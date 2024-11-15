﻿namespace ECGPlatform;

public partial class App
{
    public new static App Current => (App)Application.Current;

    public App()
    {
        ProcessManager.GetProcessLock();
    }

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

        services.AddSingleton<IndexFileService>();

        services.AddSingleton<ISettingManager, SettingManager>();

        services.AddSingleton<ILanguageManager, LanguageManager>();
        services.AddSingleton<IThemeManager, ThemeManager>();

        services.AddSingleton<IHttpManager, HttpManager>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(sp =>
        {
            var mainWindowViewModel = sp.GetService<MainWindowViewModel>()!;
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            mainWindowViewModel.BindingWindow = mainWindow;
            return mainWindow;
        });

        services.AddTransient<LocalDataPageViewModel>();
        services.AddTransient<LocalDataPage>(sp => new LocalDataPage
        {
            DataContext = sp.GetService<LocalDataPageViewModel>()
        });
        
        services.AddTransient<RemotePageViewModel>();
        services.AddTransient<RemotePage>(sp => new RemotePage
        {
            DataContext = sp.GetService<RemotePageViewModel>()
        });
        
        services.AddTransient<TemplatePageViewModel>();
        services.AddTransient<TemplatePage>(sp => new TemplatePage()
        {
            DataContext = sp.GetService<TemplatePageViewModel>()
        });

        services.AddTransient<SettingPageViewModel>();
        services.AddTransient<SettingPage>(sp => new SettingPage
        {
            DataContext = sp.GetService<SettingPageViewModel>()
        });

        services.AddTransient<ShowECGWindowViewModel>();
        services.AddTransient<ShowECGWindow>(sp =>
        {
            var showECGWindowViewModel = sp.GetService<ShowECGWindowViewModel>()!;
            var showECGWindow = new ShowECGWindow
            {
                DataContext = showECGWindowViewModel
            };
            showECGWindowViewModel.BindingWindow = showECGWindow;
            return showECGWindow;
        });
        
        services.AddTransient<LoginWindowViewModel>();
        services.AddTransient<LoginWindow>(sp =>
        {
            var loginWindowViewModel = sp.GetService<LoginWindowViewModel>()!;
            var loginWindow = new LoginWindow
            {
                DataContext = loginWindowViewModel
            };
            loginWindowViewModel.BindingWindow = loginWindow;
            return loginWindow;
        });
        

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var logger = Services.GetService<ILogger>();
        logger?.Information("Application Start Up.");
        _ = Services.GetService<ILanguageManager>();
        _ = Services.GetService<IThemeManager>();

        // var loginWindow = Services.GetService<LoginWindow>();
        // loginWindow!.Show();
        var mainWindow = Services.GetService<MainWindow>();
        mainWindow!.Show();
    }
}