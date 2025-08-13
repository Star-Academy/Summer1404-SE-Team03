using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScoreManager.DataBase;
using ScoreManager.Interfaces;
using ScoreManager.Services;
using ScoreManager.Tools; // Add this using for the printer

public class Program
{
    public static async Task Main(string[] args)
    {
        // Use Host.CreateDefaultBuilder for standardized setup (logging, config, DI)
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // --- Register the DbContextFactory ---
                // The factory is registered as a singleton, but it creates short-lived DbContext instances.
                services.AddDbContextFactory<ScoreManagerDbContext>(options =>
                {
                    const string connectionString = "Host=localhost;Database=score_manager_db;Username=postgres;Password=mypassword";
                    options.UseNpgsql(connectionString);
                });

                // --- Register Application Services ---
                
                // Register the DB Executer as a singleton as it is stateless and thread-safe.
                services.AddSingleton<IDbExecuter, ScoreManagerDbExecuter>();

                // Services with potential state or dependencies on scoped services (like DbContext)
                // should be scoped.
                services.AddScoped<IStudentService, StudentService>();
                services.AddScoped<IScoreService, ScoreService>();

                // *** REGISTER THE NEW REPORTING SERVICE ***
                services.AddScoped<IReportingService, ReportingService>();

                // *** REGISTER THE NEW PRINTER CLASS ***
                // It's stateless, so Singleton is appropriate.
                services.AddSingleton<StudentReportPrinter>();

            }).Build();

        // The application's main logic is executed here.
        await RunApplicationLogic(host.Services);
    }

    private static async Task RunApplicationLogic(IServiceProvider serviceProvider)
    {
        // Use a service scope to resolve scoped services. This is best practice.
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                // Resolve the new services from the DI container
                var reportingService = services.GetRequiredService<IReportingService>();
                var printer = services.GetRequiredService<StudentReportPrinter>();

                Console.WriteLine("Fetching top 10 students by average score...");

                // 1. Call the service to get the data
                var topStudents = await reportingService.GetTopStudentsByAverageScoreAsync(10);

                // 2. Pass the data to the printer to display it
                printer.PrintTopStudents(topStudents);

                // You can still use other services here if needed
                // var studentService = services.GetRequiredService<IStudentService>();
                // var allStudents = await studentService.GetAllStudentsAsync();
                // Console.WriteLine($"\nTotal students in system: {allStudents.Count()}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}