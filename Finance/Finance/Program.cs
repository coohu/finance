using Finance;
using Finance.Account.Source;
using Finance.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

Logger.GetLogger(typeof(Startup)).Error("Startup");

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => options.ServiceName = "FinanceService")
    .ConfigureWebHostDefaults(webBuilder =>
    {
        var serverUrl = ConfigHelper.Instance.XmlReadAppSetting("server_url");
        webBuilder.UseUrls(serverUrl);
        webBuilder.UseStartup<Startup>();
    })
    .Build();

if (255 == CommondHandler.Test())
{
    CommondHandler.Process("init -f");
    CommondHandler.Process("act.init finance_demo -f");
}

host.Run();
