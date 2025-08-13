using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScoreManager.DataBase;
using ScoreManager.Interfaces;
using ScoreManager.Services;
using ScoreManager.Tools;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                
                logging.AddConsole();

                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContextFactory<ScoreManagerDbContext>(options =>
                {
                    const string connectionString = "Host=localhost;Database=score_manager_db;Username=postgres;Password=mypassword";
                    options.UseNpgsql(connectionString);
                });

                services.AddSingleton<IDbExecuter, ScoreManagerDbExecuter>();
                services.AddScoped<IStudentService, StudentService>();
                services.AddScoped<IScoreService, ScoreService>();
                services.AddScoped<IReportingService, ReportingService>();
                
                services.AddSingleton<StudentReportPrinter>();
                services.AddScoped<DatabaseSeeder>();

            }).Build();

        await RunApplicationLogic(host.Services);
    }

    private static async Task RunApplicationLogic(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var seeder = services.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedDataAsync();

                var reportingService = services.GetRequiredService<IReportingService>();
                var printer = services.GetRequiredService<StudentReportPrinter>();

                var topStudents = await reportingService.GetTopStudentsByAverageScoreAsync(10);

                printer.PrintTopStudents(topStudents);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unhandled error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}