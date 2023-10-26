using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

namespace BookStore.CodeDeveloperProject.WebAPI
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Other configurations...

            services.AddDbContext<BookstoreDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configure JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://BookStore-CDP.com", // actual issuer
                        ValidAudience = "BookStore.CodeDeveloperProject.WebAPI", // actual audience
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234")) // secret key
                    };
                });

            // Add other services as needed...
            services.AddControllers();

            // Add Swagger services
            if (services.Any(x => x.ServiceType == typeof(ISwaggerProvider)))
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStoreAPI", Version = "v1" });
                    // Add any additional Swagger configuration here...
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use CORS policy
            app.UseCors("CorsPolicy");

            // Other middleware configurations...

            // Use Authentication
            app.UseAuthentication();

            // Use Authorization
            app.UseAuthorization();

            // Add Swagger middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStoreAPI v1");
                });// aplicação estiver sendo executada localmente e o Swagger estiver configurado conforme padrões,
                   // a URL para o Swagger JSON pode ser algo como: https://localhost:5001/swagger/v1/swagger.json

            }

            // Other middleware configurations...
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

