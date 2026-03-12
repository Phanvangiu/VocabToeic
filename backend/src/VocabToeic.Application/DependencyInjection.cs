using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VocabToeic.Application.Common.Behaviors;

namespace VocabToeic.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(
      this IServiceCollection services)
  {
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

    services.AddAutoMapper(typeof(DependencyInjection).Assembly);

    // Đăng ký FluentValidation tự động scan toàn bộ Assembly
    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

    // Đăng ký ValidationBehavior vào MediatR Pipeline
    services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));

    return services;
  }
}