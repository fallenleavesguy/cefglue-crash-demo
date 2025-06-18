using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BarrichCSSystem.ViewModels;
using BarrichCSSystem.Views;

namespace BarrichCSSystem;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

public class WhatsAppBrowserViewLocator: IDataTemplate
{
    public Control? Build(object? data)
    {
        var dataContext = data as WhatsAppBrowserViewModel;
        if (dataContext is null)
            return null;

        var browser = new WhatsAppBrowserView("https://www.qq.com/", dataContext.UniqKey)
        {
            DataContext = dataContext
        };
        
        return browser;
    }

    public bool Match(object? data)
    {
        return data is WhatsAppBrowserViewModel;
    }
}