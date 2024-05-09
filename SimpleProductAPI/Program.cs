
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleProductAPI.Context;
using SimpleProductAPI.Repositories;

namespace SimpleProductAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration.AddJsonFile("appsettings.json",
                                                        optional: true,
                                                        reloadOnChange: false)
                                                        .Build();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbConnection(configuration);
            builder.Services.AddAutoMapper(typeof(MapperProfile));

            var app = builder.Build();
            app.Services.ApplyMigrations();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void AddDbConnection(this IServiceCollection services,
                                               IConfiguration configuration)
        {
            var databaseName = configuration["ConnectionStrings:DatabaseName"];
            var connectionString = configuration["ConnectionStrings:Connection"].Replace("{DatabaseNameHere}", $"Database={databaseName}");

            CheckAndCreateDatabase(configuration);

            services.AddDbContext<IDataContext, DataContext>(dbContextOptions => dbContextOptions
                                .UseSqlServer(connectionString));

            services.AddScoped<IProductRepository, ProductRepository>();
        }

        private static void ApplyMigrations(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDataContext>();

            var dbFacade = context.GetDatabaseFacade();
            dbFacade.OpenConnection();

            var pendingMigrations = dbFacade.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                dbFacade.Migrate();
            }
        }

        public static void CheckAndCreateDatabase(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:Connection"].Replace("{DatabaseNameHere};", "");
            var databaseName = configuration["ConnectionStrings:DatabaseName"];

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = $"SELECT name FROM sys.databases WHERE name = '{databaseName}'";
                using (var command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return;
                    }
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = $"CREATE DATABASE {databaseName}";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
