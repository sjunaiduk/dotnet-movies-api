using Movies.Application.Models;
using Dapper;
using Movies.Application.Database;
using System.Reflection.Metadata;

namespace Movies.Application.Repositories
{
    internal class MovieRepositoryPg : IMovieRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepositoryPg(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);
      
            const string sql = """
                                 SELECT m.*, STRING_AGG(g.genre, ',') as genres,
                                 ROUND(AVG(r.rating),1) as rating,
                                 myr.rating as userrating
                                
                                 FROM MOVIES m
                                 LEFT JOIN MOVIES_GENRES_MAPPING mapping on m.id = mapping.movie_id
                                 LEFT JOIN GENRES g on g.id = mapping.genre_id
                                 LEFT JOIN ratings r on m.id = r.movie_id
                                 LEFT JOIN ratings myr on m.id = myr.movie_id AND myr.user_id=@UserId
                                 WHERE m.id = @Id
                                 GROUP BY m.id, myr.rating
                               """;
            var movie = await connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql,
                new { id, userId }, cancellationToken :token));
            if (movie is null)
            {
                return null;
            }

            return new Movie
            {
                Id = id,
                Title = movie.title,
                YearOfRelease = movie.yearofrelease,
                UserRating = (int?)movie.userrating,
                Rating = (float?)movie.rating,
                Genres = string.IsNullOrEmpty(movie.genres) ?
                new List<string>()
                :
                Enumerable.ToList(movie.genres.Split(','))
            };
        }

        public async Task<Movie?> GetBySlugAsync(string slug, Guid?  userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);

            const string sql = """
                                 SELECT m.*, STRING_AGG(DISTINCT g.genre, ',') as genres,
                                 ROUND(AVG(r.rating),1) as rating,
                                 myr.rating as userrating
                                
                                 FROM MOVIES m
                                 LEFT JOIN MOVIES_GENRES_MAPPING mapping on m.id = mapping.movie_id
                                 LEFT JOIN GENRES g on g.id = mapping.genre_id
                                 LEFT JOIN ratings r on m.id = r.movie_id
                                 LEFT JOIN ratings myr on m.id = myr.movie_id AND myr.user_id=@UserId
                                 WHERE m.Slug=@Slug
                                 GROUP BY m.id, myr.rating
                               """;
            var movie = await connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql,
                new { slug, userId }, cancellationToken: token));
            if (movie is null)
            {
                return null;
            }

            return new Movie
            {
                Id = movie.id,
                Title = movie.title,
                YearOfRelease = movie.yearofrelease,
                UserRating = (int?)movie.userrating,
                Rating = (float?)movie.rating,
                Genres = string.IsNullOrEmpty(movie.genres) ?
                new List<string>()
                :
                Enumerable.ToList(movie.genres.Split(','))
            };
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);
            var transaction = connection.BeginTransaction();
            var movieMappingDeleted = await connection.ExecuteAsync(new CommandDefinition(
                """DELETE FROM MOVIES_GENRES_MAPPING WHERE movie_id=@Id""",
                new { id }, cancellationToken: token));
            if (movieMappingDeleted == 0) { return false; }

            await connection.ExecuteAsync(new CommandDefinition(
                """DELETE FROM GENRES g WHERE g.id NOT IN (SELECT genre_id FROM MOVIES_GENRES_MAPPING)""", cancellationToken: token));
            await connection.ExecuteAsync(new CommandDefinition(
                  """DELETE FROM MOVIES WHERE id=@Id""",
                  new {id}, cancellationToken: token));

            transaction.Commit();

            return true;

        }

        public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
        {
           using var connection = await _dbConnectionFactory.GetConnection(token);

            const string sql = """
                                 SELECT m.*, STRING_AGG(DISTINCT g.genre, ',') as genres,
                                 ROUND(AVG(r.rating),1) as rating,
                                 myr.rating as userrating
                                 FROM MOVIES m
                                 LEFT JOIN MOVIES_GENRES_MAPPING mapping on m.id = mapping.movie_id
                                 LEFT JOIN GENRES g on g.id = mapping.genre_id
                                 LEFT JOIN ratings r on m.id = r.movie_id
                                 LEFT JOIN ratings myr on m.id = myr.movie_id AND myr.user_id=@UserId
                                 GROUP BY m.id, myr.rating
                               """;
            var movies = await connection.QueryAsync(new CommandDefinition(sql,new {userId }, cancellationToken: token));
            return movies.Select(m => new Movie
            {
                Id = m.id,
                Title = m.title,
                YearOfRelease = m.yearofrelease,
                Rating = (float?)m.rating,
                UserRating = (int?)m.userrating,
                Genres = string.IsNullOrEmpty(m.genres) ? new List<string>() 
                    :
                    Enumerable.ToList(m.genres.Split(","))
            });
        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);
            var transaction = connection.BeginTransaction();
            var count = await connection.ExecuteAsync(new CommandDefinition(
                """
                UPDATE MOVIES
                SET 
                title=@Title,
                yearofrelease=@YearOfRelease,
                slug=@Slug
                WHERE id=@Id
                """,cancellationToken: token, parameters: movie));

            await connection.ExecuteAsync(new CommandDefinition(
                """
                DELETE FROM MOVIES_GENRES_MAPPING
                WHERE movie_id=@Id
                """, movie, cancellationToken: token));

            var genreIds = new List<int>();

            foreach (var genre in movie.Genres)
            {
                var genreId = await CreateGenreAndReturnIdAsync(genre);
                var mappingExists = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
                    """
                    SELECT COUNT(1) FROM MOVIES_GENRES_MAPPING
                    WHERE movie_id=@MovieId and genre_id=@GenreId
                    """, new { MovieId = movie.Id, genreId }, cancellationToken: token));
                if (!mappingExists)
                {
                    await connection.ExecuteAsync(new CommandDefinition(
                        """
                        INSERT INTO MOVIES_GENRES_MAPPING VALUES 
                        (@MovieId, @GenreId)
                        """, new { movieId = movie.Id, genreId },
                        cancellationToken: token
                        ));
                }
            }

            transaction.Commit();
            return count > 0;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);
            var transaction = connection.BeginTransaction();
            var count = await connection.ExecuteAsync(new CommandDefinition("""
                                          INSERT INTO MOVIES VALUES (
                                          @Id,
                                          @Slug,  
                                          @Title,
                                                                     
                                          @YearOfRelease
                                          )
                                          """, movie, cancellationToken: token));

            if (count > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    var genreId = await CreateGenreAndReturnIdAsync(genre);
                    await connection.ExecuteAsync(new CommandDefinition("""
                                                  INSERT INTO MOVIES_GENRES_MAPPING (movie_id, genre_id) VALUES (
                                                    @Id,
                                                    @GenreId
                                                  )
                                                  """, new { Id = movie.Id, genreId },
                                                  cancellationToken: token));
            
                } 

            }
      
          transaction.Commit();

            return count > 0;
        }

        public async Task<bool> MovieExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await  _dbConnectionFactory.GetConnection(token);
            var exists = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
                """
                SELECT COUNT(1) FROM MOVIES WHERE id=@Id
                """, new { id }, cancellationToken: token));
            return exists;
        }

     
        public async Task<Guid> CreateGenreAndReturnIdAsync(string genre, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.GetConnection(token);

            var transaction = connection.BeginTransaction();

            var exists = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
                """
                SELECT COUNT(1) FROM GENRES WHERE genre=@Genre
                """, new { genre }, cancellationToken: token));

            Guid id = !exists
               ? await connection.ExecuteScalarAsync<Guid>(
                   new CommandDefinition(
                       """
                        INSERT INTO GENRES VALUES (@Id, @Genre) RETURNING id
                        """, new { id = Guid.NewGuid(), genre }, cancellationToken: token))
               : await connection.ExecuteScalarAsync<Guid>(
                   new CommandDefinition(
                   """
                    SELECT id FROM GENRES WHERE genre = @Genre
                    """, new { Genre = genre }, cancellationToken: token));

            transaction.Commit();
            return id;

        }
        

    }
}
