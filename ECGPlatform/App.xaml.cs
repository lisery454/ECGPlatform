namespace ECGPlatform;

public partial class App
{
    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; } = ConfigureServices();

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ProgramConstants>();

        services.AddSingleton<ILogger>(sp =>
            new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(sp.GetService<ProgramConstants>()!.LogPath)
                .CreateLogger()
        );

        services.AddSingleton<ISerializer, Serializer>();
        services.AddSingleton<IDeserializer, Deserializer>();

        services.AddSingleton<ISettingManager, SettingManager>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>(sp => new MainWindow
        {
            DataContext = sp.GetService<MainWindowViewModel>()
        });

        services.AddTransient<LocalDataPageViewModel>();
        services.AddTransient<LocalDataPage>(sp => new LocalDataPage
        {
            DataContext = sp.GetService<LocalDataPageViewModel>()
        });

        services.AddTransient<SettingPageViewModel>();
        services.AddTransient<SettingPage>(sp => new SettingPage
        {
            DataContext = sp.GetService<SettingPageViewModel>()
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