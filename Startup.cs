using that2dollar.Services;
using that2dollar.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace that2dollar
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddHttpClient();


            services.AddDbContext<ToUsdContext>(options => {
                /// opt.UseInMemoryDatabase(databaseName: "database_name");

            }, ServiceLifetime.Transient);

            services.AddScoped<IHttpSpooler, HttpSpooler>();
            services.AddScoped<IRatesService, RatesService>();


            services.AddControllers().AddJsonOptions(option =>
            option.JsonSerializerOptions.PropertyNamingPolicy
                = JsonNamingPolicy.CamelCase
                 );
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Currencies to USD rates service",
                    Description = "Microservices NetCore 5, Typescript, Postgress , Yahoo business Services",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Dergachev Andrey",
                        Email = "andrey1yalta@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/andrey-dergachev-b3053010/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable Cors
            app.UseCors("MyPolicy");

            if (true || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "that2dollar v1"));
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //    app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
