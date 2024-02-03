namespace ECGPlatform;

public partial class App : Application
{
    public static new App Current => (App)Application.Current;

    public IServiceProvider Services { get; private set; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogger>(_ =>
            new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log.txt").CreateLogger()
        );

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>(sp => new MainWindow
        {
            DataContext = sp.GetService<MainWindowViewModel>()
        });

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
		var logger = Services.GetService<ILogger>();
        logger?.Information("Application Start Up.");
       
		var mainWindow = Services.GetService<MainWindow>();
        mainWindow!.Show();
    }
}
