﻿using Todo.Domain.Models;
using Todo.Application.Services;
using Microsoft.AspNetCore.Http;
using Todo.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Todo.Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Todo.Application.Notifications;
using Todo.Domain.Contracts.Repository;
using ScottBrady91.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Contracts.Services;

namespace Todo.Application.Configuration;

public static class DependecyConfig
{
    public static void ResolveDependecies(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton(_ => builder.Configuration);
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAssignmentListRepository, AssignmentListRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAssignmentListService, AssignmentListService>();

        services.AddScoped<INotificator, Notificator>();
        services.AddScoped<IPasswordHasher<User>, Argon2PasswordHasher<User>>();
    }
}