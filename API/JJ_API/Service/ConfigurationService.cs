using Newtonsoft.Json;

namespace JJ_API.Service
{
    public class ConfigurationService
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
           var settings = PropertiesSingletonBase.Load();
             PropertiesSingletonBase.Save(settings);
            config.AddJsonFile(JsonConvert.SerializeObject(settings, Formatting.Indented), optional: false, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}
