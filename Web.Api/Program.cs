using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Api.Data;
using Web.Api.Security;
using Web.Api.Services;

var builder = WebApplication.CreateBuilder(args);


// Controllers
builder.Services.AddControllers();

// Options
builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("Jwt"));

// Repositpory
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<TokenRepository>();

// Services
builder.Services.AddSingleton<EmailTokenService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();

// Security
builder.Services.AddSingleton<JwtOption>();

// Authentication
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOption>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer( o =>
    {
        o.RequireHttpsMetadata = false; // Set to true for production
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

app.Run();
