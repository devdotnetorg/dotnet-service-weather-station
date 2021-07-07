using Avalonia;
using System;
using System.Globalization;
using System.Threading;

namespace WeatherStation.Panel
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            //pause
            if(args.Length==1)
            {
                int intTime = Convert.ToInt32(args[0]);
                Console.WriteLine($"Запуск программы через {intTime} секунд");
                Thread.Sleep(intTime*1000);
            }
            //Set Culture
            var vCulture = CultureInfo.GetCultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = vCulture;
            Thread.CurrentThread.CurrentUICulture = vCulture;
            CultureInfo.DefaultThreadCurrentCulture = vCulture;
            CultureInfo.DefaultThreadCurrentUICulture = vCulture;
            //
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions { AllowEglInitialization = true })
                .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                .With(new AvaloniaNativePlatformOptions { UseGpu = true })            
                .LogToTrace();                
    }
}


