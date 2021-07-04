using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Runtime.InteropServices;

namespace WeatherStation.Panel
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                       
                .LogToTrace();
                //.UseX11();
                //Add
                //.With(new X11PlatformOptions { UseGpu=false} )            
                //.With(new AvaloniaNativePlatformOptions { UseGpu = false })
                //.With(new Win32PlatformOptions { UseDeferredRendering = false });            
    }
}