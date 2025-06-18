using Avalonia;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Threading;
using BarrichCSSystem.Utils;
using Xilium.CefGlue;
using Xilium.CefGlue.Common;

namespace BarrichCSSystem;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            Console.WriteLine("Process is exiting, cleaning up resources...", e.ToString());
            Cleanup();
        };

        try
        {
            RegisterExceptionHandlers();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("top level exception: " + ex.Message);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .AfterSetup(_ =>
            {
                var cachePath = FileUtils.GetCefCachePath();
                FileUtils.EnsureDirectoryExists(cachePath);
                
                Debug.WriteLine($"\nCefGlue cache path: {cachePath}\n");
                
                CefRuntimeLoader.Initialize(new CefSettings()
                {
                    CachePath = cachePath,
                    WindowlessRenderingEnabled = false,
                });
            })
            .LogToTrace();
    
    private static void Cleanup()
    {
        CefRuntime.Shutdown();
    }
    
    private static void RegisterExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            Console.WriteLine($"{sender} 引发了未经处理的异常\n{args.ExceptionObject}");
        // Dispatcher.UIThread.UnhandledException += (sender, args) =>
        // {
        //     Console.WriteLine($"{sender} 引发了未经处理的异常\n{args.Exception}");
        //     args.Handled = true;
        // };
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Console.WriteLine($"{sender ?? "[未捕获来源]"} 引发了未经处理的异常\n{args.Exception}");
            args.SetObserved();
        };
    }
}