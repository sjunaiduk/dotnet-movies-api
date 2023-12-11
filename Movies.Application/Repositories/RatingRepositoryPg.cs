using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    internal class RatingRepositoryPg : IRatingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RatingRepositoryPg(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreateRatingAsync(Guid movieId, Guid? userId, int rating, CancellationToken token = default)
        {
            using var connection = await _connectionFactory.GetConnection();
            var created = await connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO RATINGS (user_id, movie_id, rating)
                VALUES (@UserId, @MovieId, @Rating)
                ON CONFLICT (user_id, movie_id) 
                DO UPDATE SET rating=@Rating                
                """, new { movieId, userId, rating }, cancellationToken: token));
            return created > 0;
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default)
        {
            using var connection = await _connectionFactory.GetConnection();
            var created = await connection.ExecuteAsync(new CommandDefinition(
                 """
                DELETE FROM ratings
                WHERE 
                movie_id = @MovieId AND
                user_id = @UserId
                """, new { movieId, userId }, cancellationToken: token));
            return created > 0;

        }

        public async Task<MovieRating?> GetRatingAsync(Guid movieId, CancellationToken token = default)
        {
            using var connection = await _connectionFactory.GetConnection();
            var rating = await connection.QuerySingleOrDefaultAsync<MovieRating>(new CommandDefinition(
                """
                  SELECT m.*, 
                  ROUND(AVG(r.rating),1) as rating
                  FROM MOVIES m
                  LEFT JOIN ratings r on m.id = r.movie_id
                  WHERE m.id=@MovieId
                  GROUP BY m.id
                """, new { movieId }, cancellationToken: token));
            return rating;
        }

        public async Task<IEnumerable<MovieRating>> GetUserRatings(Guid? userId, CancellationToken token = default)
        {
            using var connection = await _connectionFactory.GetConnection();
            var ratings = await connection.QueryAsync<MovieRating>(new CommandDefinition(
                """
                SELECT m.*, 
                       avgRatings.avgRating as rating,
                       r.rating as userRating,
                       r.user_id
                FROM MOVIES m
                LEFT JOIN (
                    SELECT movie_id, ROUND(AVG(rating),1) AS avgRating
                    FROM ratings
                    GROUP BY movie_id
                ) as avgRatings ON m.id = avgRatings.movie_id
                LEFT JOIN ratings r ON m.id = r.movie_id
                WHERE r.user_id = @UserId;
                
                """, new { userId }, cancellationToken: token));
            return ratings;
        }
    }
}
