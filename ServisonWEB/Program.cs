using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;

namespace Default
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appBasePath = System.IO.Directory.GetCurrentDirectory();
            GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
            var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
