﻿using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappings
{
    public class ValidationMappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch(ValidationException ex)
            {
                context.Response.StatusCode = 400;
                var validationFaliureResponse = new ValidationErrorResponse
                {
                    Errors = ex.Errors.Select(e => new ValidationError
                    { Error = e.ErrorMessage,
                      Property = e.PropertyName
                    })
                };

                await context.Response.WriteAsJsonAsync(validationFaliureResponse);
            }
        }
    }
}
