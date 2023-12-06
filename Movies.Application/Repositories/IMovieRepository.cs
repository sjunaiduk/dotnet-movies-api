﻿using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
        Task<Movie?> GetByIdAsync(Guid id);

        Task<Movie?> GetBySlugAsync(string slug);


        Task<bool> DeleteByIdAsync(Guid id);

        Task<IEnumerable<Movie>> GetAllAsync();

        Task<bool> UpdateAsync(Movie movie);

        Task<bool> CreateAsync(Movie movie);


    }
}
