using Microsoft.EntityFrameworkCore;
using AIDMS.Entities;
using AIDMS.Repositories;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

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
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add Repositories
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Google Vision
            builder.Services.Configure<GoogleCloudVisionOptions>(builder.Configuration.GetSection("GoogleCloudVision"));
            builder.Services.AddSingleton<IGoogleCloudVisionService, GoogleCloudVisionService>();

            // swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AIDMS API", Version = "v1" });
                c.OperationFilter<FileUploadOperationFilter>();
            });

            // Configure Database Context
            builder.Services.AddDbContext<AIDMSContextClass>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocalCS"));
            });

            // Configure CORS
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
