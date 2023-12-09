using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories;
using Movies.Application.Database;
using Movies.Application.Services;
using FluentValidation;

namespace Movies.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IMovieRepository, MovieRepositoryPg>();
            services.AddSingleton<IMovieService, MovieService>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new PgDbConnectionFactory(connectionString));

            services.AddSingleton<DbInitializer>();

            return services;
        }

       
    
    }
}
