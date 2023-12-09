namespace Movies.Api
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;

        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.QueryString.Value == "?name=Junaid")
            {
                await context.Response.WriteAsJsonAsync("Hello World");
                return;
            }

            await _next(context);
        }
    }

    public static class TestMiddlewareExtensions
    {
        public static void UseHelloWorld(this IApplicationBuilder app)
        {
            app.UseMiddleware<TestMiddleware>();
        }
    }
}
