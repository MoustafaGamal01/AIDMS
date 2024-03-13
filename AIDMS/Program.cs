using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using AIDMS.Models;

namespace AIDMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Test github
            // Test G 1.1
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Configure EF Core to use SQL Server with the connection string named "LocalCS"
            builder.Services.AddDbContext<AIDMSContextClass>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocalCS"));
            });

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
