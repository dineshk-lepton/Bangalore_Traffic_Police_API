using Bangalore_Traffic_API;
using Bangalore_Traffic_API.BAL;
using Bangalore_Traffic_API.DAL;
using Bangalore_Traffic_API.Model;
using Serilog;


        IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IDBAccess, DBAccess>();
        services.AddSingleton<IBangaloreAPIHit, BangaloreAPIHit>();
        services.AddHostedService<Worker>();
        services.AddHttpClient();
    }).UseSerilog()
    .Build();


        var configSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var logPath = configSettings["Logging:LogPath"];
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var date = DateTime.Now.ToString("dd/MM/yyyy").Replace('/', '_');
            string logFilepath = Path.Combine(logPath, date + ".txt");

             Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(logFilepath)
            .CreateLogger();


        host.Run();
   