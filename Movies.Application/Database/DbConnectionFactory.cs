using System.Data;
using Npgsql;

namespace Movies.Application.Database;

public interface IDbConnectionFactory
{
     Task<IDbConnection> GetConnection(CancellationToken token = default);
}


public class PgDbConnectionFactory : IDbConnectionFactory
{

     private readonly string _connectionString;

     public PgDbConnectionFactory(string connectionString)
     {
          _connectionString = connectionString;
     }
     
     public async Task<IDbConnection> GetConnection(CancellationToken token = default)
     {
          var connection = new NpgsqlConnection(_connectionString);
          await connection.OpenAsync(token);
          return connection;

     }
}