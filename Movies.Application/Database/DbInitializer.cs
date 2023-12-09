using System.Data;
using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitialiseDb()
    {
        var connection = await _dbConnectionFactory.GetConnection();
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS MOVIES (
                                          id UUID PRIMARY KEY,
                                          slug TEXT NOT NULL,
                                          title TEXT NOT NULL,
                                          yearofrelease integer NOT NULL);
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS GENRES (
                                          id UUID PRIMARY KEY,
                                          genre TEXT NOT NULL);
                                      """);
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS MOVIES_GENRES_MAPPING (
                                          movie_id UUID REFERENCES MOVIES(id),
                                          genre_id UUID REFERENCES GENRES(id),
                                          PRIMARY KEY (movie_id, genre_id)
                                      )
                                      """);
        
        await connection.ExecuteAsync("""
                                        create unique index concurrently
                                        if not exists movies_slug_indx
                                        on movies
                                        using btree(slug)
                                      """);

        await connection.ExecuteAsync("""
                create table if not exists ratings (
                user_id uuid,
                movie_id uuid references movies(id),
                rating int,
                primary key (user_id, movie_id)
                )
            """);

    }
}