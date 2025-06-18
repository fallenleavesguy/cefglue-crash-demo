using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using BarrichCSSystem.Utils;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Handlers;

namespace BarrichCSSystem.Views
{
    public class WhatsAppBrowserView : UserControl
    {
        private readonly Decorator _browserWrapper;
        
        private readonly AvaloniaCefBrowser _browser;

        public WhatsAppBrowserView(string url, string uniqKey)
        {
            this._browserWrapper = new Decorator();
            this.Content = this._browserWrapper;
            
            var relativeCachePath = FileUtils.GetCefRelativeCachePath(uniqKey);

            _browser = new AvaloniaCefBrowser(() => {
                var cachePath = FileUtils.GetCefCachePath(relativeCachePath);
                
                FileUtils.EnsureDirectoryExists(cachePath);
                
                return CefRequestContext.CreateContext(new CefRequestContextSettings
                {
                    PersistSessionCookies = true,
                    PersistUserPreferences = true,
                    CachePath = cachePath,
                }, null);
            });
            _browser.Address = url;
            _browser.ContextMenuHandler = new CustomContextMenuHandler()
            {
                OpenDevToolsHandler = this.OpenDevTools,
            };
            // browser.TitleChanged += OnBrowserTitleChanged;
            // _browser.LifeSpanHandler = new BrowserLifeSpanHandler();
            _browserWrapper.Child = _browser;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            // 关闭cef浏览器
            this.Dispose();
        }

        static Task<object> AsyncCallNativeMethod(Func<object> nativeMethod)
        {
            return Task.Run(() =>
            {
                var result = nativeMethod.Invoke();
                if (result is Task task)
                {
                    if (task.GetType().IsGenericType)
                    {
                        return ((dynamic) task).Result;
                    }

                    return task;
                }

                return result;
            });
        }

        public event Action<string>? TitleChanged;

        private void OnBrowserTitleChanged(object sender, string title)
        {
            TitleChanged?.Invoke(title);
        }

        private void OnAddressTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _browser.Address = ((TextBox)sender).Text;
            }
        }

        public async Task EvaluateJavascript()
        {
            var result = new StringWriter();

            result.Write(await _browser.EvaluateJavaScript<string>("return \"Hello World!\""));

            result.Write("; " + await _browser.EvaluateJavaScript<int>("return 1+1"));

            result.Write("; " + await _browser.EvaluateJavaScript<bool>("return false"));

            result.Write ("; " + await _browser.EvaluateJavaScript<double>("return 1.5+1.5"));

            result.Write("; " + await _browser.EvaluateJavaScript<double>("return 3+1.5"));

            result.Write("; " + await _browser.EvaluateJavaScript<DateTime>("return new Date()"));

            result.Write("; " + string.Join(", ", await _browser.EvaluateJavaScript<object[]>("return [1, 2, 3]")));

            result.Write("; " + string.Join(", ", (await _browser.EvaluateJavaScript<ExpandoObject>("return (function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));

            _browser.ExecuteJavaScript($"alert(\"{result.ToString().Replace("\r\n", " | ").Replace("\"", "\\\"")}\")");
        }
        
        // public void BindJavascriptObject()
        // {
        //     const string TestObject = "dotNetObject";
        //
        //     var obj = new BindingTestClass();
        //     _browser.RegisterJavascriptObject(obj, TestObject, AsyncCallNativeMethod);
        //
        //     var methods = obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
        //                                .Where(m => m.GetParameters().Length == 0)
        //                                .Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1));
        //
        //     var script = "(function () {" +
        //         "let calls = [];" +
        //         string.Join("", methods.Select(m => $"calls.push({{ name: '{m}', promise: {TestObject}.{m}() }});")) +
        //         $"calls.push({{ name: 'asyncGetObjectWithParams', promise: {TestObject}.asyncGetObjectWithParams('a string') }});" +
        //         $"calls.push({{ name: 'getObjectWithParams', promise: {TestObject}.getObjectWithParams(5, 'a string', {{ Name: 'obj name', Value: 10 }}, [ 1, 2 ]) }});" +
        //         "calls.forEach(c => c.promise.then(r => console.log(c.name + ': ' + JSON.stringify(r))).catch(e => console.log(e)));" +
        //         "})()";
        //
        //     _browser.ExecuteJavaScript(script);
        // }

        public void ExecuteJavascript(string script)
        {
            _browser.ExecuteJavaScript(script); 
        }

        public void OpenDevTools()
        {
            _browser.ShowDeveloperTools();
        }

        public void Dispose()
        {
            _browser.Dispose();
        }

        private class BrowserLifeSpanHandler : LifeSpanHandler
        {
            protected override bool OnBeforePopup(
                CefBrowser browser,
                CefFrame frame,
                string targetUrl,
                string targetFrameName,
                CefWindowOpenDisposition targetDisposition,
                bool userGesture,
                CefPopupFeatures popupFeatures,
                CefWindowInfo windowInfo,
                ref CefClient client,
                CefBrowserSettings settings,
                ref CefDictionaryValue extraInfo,
                ref bool noJavascriptAccess)
            {
                var bounds = windowInfo.Bounds;
                Dispatcher.UIThread.Post(() =>
                {
                    var window = new Window();
                    var popupBrowser = new AvaloniaCefBrowser();
                    popupBrowser.Address = targetUrl;
                    window.Content = popupBrowser;
                    window.Position = new PixelPoint(bounds.X, bounds.Y);
                    window.Height = bounds.Height;
                    window.Width = bounds.Width;
                    window.Title = targetUrl;
                    window.Show();
                });
                return true;
            }
        }
    }
    
    internal class CustomContextMenuHandler : ContextMenuHandler
    {
        public delegate void OpenDevToolsDelegate();
        public OpenDevToolsDelegate? OpenDevToolsHandler;
        private const int DeveloperConsoleMenuItemId = (int)CefMenuId.UserFirst + 1;

        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            base.OnBeforeContextMenu(browser, frame, state, model);
            model.Clear();
            
            model.AddItem((int)CefMenuId.Copy, "复制");
            model.AddItem((int)CefMenuId.Paste, "粘贴");
            model.AddItem((int)CefMenuId.Reload, "刷新");
            model.AddItem(DeveloperConsoleMenuItemId, "控制台");
        }

        protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId,
            CefEventFlags eventFlags)
        {
            if (commandId == DeveloperConsoleMenuItemId)
            {
                // invoke delegate to Open the developer console
                OpenDevToolsHandler?.Invoke();
                return true;
            }
            
            return base.OnContextMenuCommand(browser, frame, state, commandId, eventFlags);
        }
    }
}