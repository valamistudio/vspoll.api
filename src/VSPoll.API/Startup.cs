﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using EvolveDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using VSPoll.API.Extensions;
using VSPoll.API.Persistence.Contexts;

namespace VSPoll.API;

[ExcludeFromCodeCoverage]
public class Startup
{
    public Startup(IConfiguration configuration)
        => Configuration = configuration;

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServices().AddRepositories().AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        SetupDatabase(services);
        ConfigureSwagger(services);
    }

    protected virtual void SetupDatabase(IServiceCollection services)
        => services.AddDbContext<PollContext>(options => options
            .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
            .UseLazyLoadingProxies()
            .UseSnakeCaseNamingConvention());

    private static void ConfigureSwagger(IServiceCollection services)
        => services.AddSwaggerGen(x => x.SwaggerDoc("v1", new()
        {
            Title = "VSPoll API",
            Version = "v1",
        }));

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            ConfigureSwaggerEndpoint(app);
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
        ApplyMigrations(app);
    }

    private static void ConfigureSwaggerEndpoint(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/Swagger/v1/swagger.json", "VSPoll API");
            x.RoutePrefix = string.Empty;
        });
    }

    protected virtual void ApplyMigrations(IApplicationBuilder app)
    {
        using NpgsqlConnection sqlConnection = new(Configuration.GetConnectionString("DefaultConnection"));
        Evolve evolve = new(sqlConnection)
        {
            IsEraseDisabled = true,
            Locations = new[] { "Migrations" },
        };
        evolve.Migrate();
    }
}
