namespace Movies.Api.Auth
{
    public static class AuthExtensions
    {
        public static Guid? GetUser(this HttpContext context)
        {
            var user = context.User.Claims.SingleOrDefault(c => c.Type == "userid");

            if (user is null)
            {
                return null;
            }

            return Guid.Parse(user.Value);
        }
    }
}
