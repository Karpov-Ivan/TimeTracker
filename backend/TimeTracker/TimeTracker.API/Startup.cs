using System;
using System.Reflection;
using TimeTracker.DataBase;
using GitLabServices;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TimeTracker.Adapter;
using TimeTracker.Adapter.Mapper;
using TimeTracker.Interfaces;
using TimeTracker.Interfaces.Services;
using TimeTracker.Mapper;

namespace TimeTracker
{
    public class Startup
    {
        private readonly string _corsPolicy = "TimeTrackerQoollo";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("TimeTracerDB");
            services.AddDbContext<Context>(options =>
                options.UseNpgsql(connectionString)
            );
            services.AddMvc();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IGitLabService, GitLabService>();
            services.AddTransient<MapperDto>();

            services.AddControllers();
            services.AddHealthChecks();

            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy, builder =>
                {
                    builder.WithOrigins("http://localhost:5173",
                                        "http://127.0.0.1:5173",
                                        "http://localhost",
                                        "http://127.0.0.1",
                                        "http://0.0.0.0:5173",
                                        "http://185.31.160.57:5550/")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed((host) => true)
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TimeTracker", Version = "v1" });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            });

            services.AddLogging();
            services.AddAutoMapper(c => c.AddProfile<MappingDatabaseProfile>(), typeof(Startup));
            services.AddAutoMapper(c => c.AddProfile<Mapper.MapperDto>(), typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeTracker backend v1"));
            }

            app.UseCors(_corsPolicy);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
