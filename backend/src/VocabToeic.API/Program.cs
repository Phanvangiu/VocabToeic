using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VocabToeic.API.Middlewares;
using VocabToeic.Application;
using VocabToeic.Infrastructure;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── JWT Authentication ─────────────────────────────
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(secretKey)),
    ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
  };

  // Check Redis blacklist on every authenticated request
  options.Events = new JwtBearerEvents
  {
    OnTokenValidated = async context =>
    {
      var redisService = context.HttpContext.RequestServices
              .GetRequiredService<VocabToeic.Application.Common.Interfaces.IRedisService>();

      var jti = context.Principal?.Claims
              .FirstOrDefault(c => c.Type == "jti")?.Value;

      if (jti != null && await redisService.ExistsAsync($"blacklist:{jti}"))
      {
        // Token is blacklisted — reject
        context.Fail("Token has been revoked.");
      }
    }
  };
});

// ── Swagger ───────────────────────────────────────
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
  {
    Title = "VocabToeic API",
    Version = "v1",
    Description = "API hệ thống học từ vựng và luyện thi TOEIC Reading"
  });

  options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Description = "Nhập JWT token theo format: Bearer {token}"
  });

  options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

// ── CORS ──────────────────────────────────────────
builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", policy =>
      policy.WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VocabToeic API v1");
    options.RoutePrefix = "swagger";
    options.DisplayRequestDuration();
  });
}

app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseAuthentication(); // ← Phải trước UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();