using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RimLangKit.Models;
using RimLangKit.Presenters;
using RimLangKit.Services;
using Serilog;

namespace RimLangKit
{
    internal static class Program
    {
        /// <summary>
        /// DI контейнер приложения
        /// </summary>
        public static ServiceProvider? ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            // Настройка Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: Path.Combine(AppContext.BaseDirectory, "logs", "rimlangkit-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("=== Запуск приложения RimLangKit ===");

                // Настройка DI контейнера
                var services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                // Инициализация Windows Forms
                ApplicationConfiguration.Initialize();

                // Создание и запуск главной формы через DI
                var mainForm = ServiceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);

                Log.Information("=== Завершение работы приложения ===");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критическая ошибка при запуске приложения");
                MessageBox.Show(
                    $"Критическая ошибка при запуске приложения:\n\n{ex.Message}\n\nПодробности в файле логов.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
                ServiceProvider?.Dispose();
            }
        }

        /// <summary>
        /// Настройка сервисов Dependency Injection
        /// </summary>
        private static void ConfigureServices(IServiceCollection services)
        {
            // Настройка логирования
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            // Регистрация сервисов
            services.AddSingleton<IGitHubService, GitHubService>();
            services.AddSingleton<IMorpherService, MorpherService>();

            // Регистрация Models (состояние приложения)
            services.AddSingleton<ApplicationState>();

            // Регистрация Presenters (MVP паттерн)
            services.AddTransient<FileProcessingPresenter>();
            services.AddTransient<DatabasePresenter>();
            services.AddTransient<LanguageUpdatePresenter>();
            services.AddTransient<MainFormPresenter>();

            // Регистрация главной формы
            services.AddTransient<MainForm>();
        }
    }
}
