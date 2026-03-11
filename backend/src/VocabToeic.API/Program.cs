using Microsoft.OpenApi.Models;
using VocabToeic.Application;
using VocabToeic.Infrastructure;
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger ───────────────────────────────────────
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "VocabToeic API",
    Version = "v1",
    Description = "API hệ thống học từ vựng và luyện thi TOEIC Reading"
  });

  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Nhập JWT token theo format: Bearer {token}"
  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
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
app.UseAuthorization();
app.MapControllers();

app.Run();