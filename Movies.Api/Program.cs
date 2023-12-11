using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application;
using Movies.Application.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config["Jwt:Key"]!)),
            ValidAudience = config["Jwt:Audience"],
            ValidIssuer = config["Jwt:Issuer"]
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.AdminUserPolicyName, policyBuilder =>
    {
        policyBuilder.RequireClaim(AuthConstants.AdminUserClaim, "true");
    });

    options.AddPolicy(AuthConstants.TrustedMemberPolicyName, policyBuilder =>
    {
        policyBuilder.RequireAssertion(authContext =>
        {
            return authContext.User.HasClaim(claim => 
            claim is {Type: AuthConstants.TrustedMemberClaim, Value: "true"} ||
            claim is {Type: AuthConstants.AdminUserClaim, Value: "true"});
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.UseHelloWorld();
app.MapControllers();

var initializer = app.Services.GetRequiredService<DbInitializer>();
await initializer.InitialiseDb();


app.Run();
