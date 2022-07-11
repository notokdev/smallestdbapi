using Microsoft.EntityFrameworkCore;
using SmallestDbApi;
using System.Linq;

internal class Program
{
    const int DELAY = 5;

    private static void Main(string[] args)
    {
        var success = false;
        do
        {
            try
            {
                success = RunApp(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"App failed to start, waiting {DELAY} seconds to try again...");
                Console.WriteLine($"Error: {ex.Message}");
                Thread.Sleep(DELAY * 1000);
            }
        } while (!success);
    }

    public static bool RunApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options => options
            .UseMySql(connString, ServerVersion.AutoDetect(connString)));

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetService<AppDbContext>();
            context?.Database.Migrate();
            if (!context.WeatherForecasts.Any())
            {
                var summaries = new[]
                    {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

                context.WeatherForecasts.AddRange(Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                }));
                context.SaveChanges();
            }
        }

        app.Run();

        return true;
    }
}