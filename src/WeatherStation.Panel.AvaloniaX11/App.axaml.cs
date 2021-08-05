// The MIT License(MIT)
//
// Copyright(c) 2021 Serdyukov Anton <anton@devdotnet.org>
// DevDotNet.Org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace WeatherStation.Panel.AvaloniaX11
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
#if RELEASE
                //Set start position
                desktop.MainWindow.Position = new PixelPoint(0, 0);                
#endif
                //Set fullscreen
                try
                {
                    var size = GetDisplaySize();
                    desktop.MainWindow.Width = size.Width;
                    desktop.MainWindow.Height = size.Height;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Ошибка при вызове утилиты xrandr - {ex.Message}. {ex}." +
                        $" Если возникает в образе с Xfce4, то можно игнорировать.");
                }
#if RELEASE
                //For xfce4
                desktop.MainWindow.WindowState = WindowState.FullScreen;
                desktop.MainWindow.Topmost = true;
#endif
            }
            base.OnFrameworkInitializationCompleted();
        }
        public Size GetDisplaySize()
        {
            // Use xrandr to get size of screen located at offset (0,0).
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "xrandr";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            var match = System.Text.RegularExpressions.Regex.Match(output, @"(\d+)x(\d+)\+0\+0");
            var w = match.Groups[1].Value;
            var h = match.Groups[2].Value;
            Size r = new Size(int.Parse(w), int.Parse(h));
            Console.WriteLine("Display Size is {0} x {1}", w, h);
            return r;
        }
    }
}
