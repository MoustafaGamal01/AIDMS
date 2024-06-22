using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AIDMS.Entities;
using AIDMS.Repositories;
using AIDMS.Security_Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AIDMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddEndpointsApiExplorer();

            // Add Repositories
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IUniversityListNIdsRepository, UniversityListNIdsRepository>();
            builder.Services.AddScoped<IPDFManagementRepository, PDFManagementRepository>();

            // Google Vision Configuration
            builder.Services.Configure<GoogleCloudVisionOptions>(builder.Configuration.GetSection("GoogleCloudVision"));
            builder.Services.AddSingleton<IGoogleCloudVisionRepository, GoogleCloudVisionRepository>();

            // Google Cloud Configuration
            builder.Services.Configure<GoogleCloudStorageOptions>(builder.Configuration.GetSection("TempGoogleCloudStorage"));
            builder.Services.AddScoped<IGoogleCloudStorageRepository>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<GoogleCloudStorageOptions>>().Value;
                return new GoogleCloudStorageRepository(options.BucketName);
            });

            // Configure Database Context
            builder.Services.AddDbContext<AIDMSContextClass>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteCS"));
            });

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AIDMSContextClass>();

            // Configure JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtOptions>();
            builder.Services.AddSingleton(jwtSettings);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

            // Configure CORS
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Configure Swagger with OAuth2 (JWT Bearer)
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AIDMS API", Version = "v1" });

                // Define the JWT Bearer security scheme
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };
                c.AddSecurityDefinition("Bearer", securityScheme);

                // Add JWT Bearer requirement to all endpoints
                var securityRequirement = new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(securityRequirement);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIDMS API v1");

                    // Enable JWT authorization in Swagger UI
                    c.OAuthClientId("swagger-client-id");
                    c.OAuthClientSecret(""); // Normally you would set this securely
                    c.OAuthAppName("Swagger UI");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
