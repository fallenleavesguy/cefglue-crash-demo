using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarrichCSSystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private PageViewModel? _currentPage;
    
    [ObservableProperty]
    private WhatsAppBrowserViewModel? _currentWhatsAppPage;

    [ObservableProperty] private ObservableCollection<WhatsAppBrowserViewModel> _whatsAppsPages = [];

    [RelayCommand]
    private void CreateWhatsAppPage(string? uniKey = null)
    {
        var vm = new WhatsAppBrowserViewModel
        {
            UniqKey = string.IsNullOrEmpty(uniKey) ? Guid.NewGuid().ToString() : uniKey,
        };
        WhatsAppsPages.Add(vm);

        this.SwitchWhatsAppPage(vm);
    }
    
    [RelayCommand]
    private void SwitchWhatsAppPage(WhatsAppBrowserViewModel vm)
    {
        CurrentWhatsAppPage = vm;
    }

    [RelayCommand]
    private void CloseWhatsAppAccountWindow(WhatsAppBrowserViewModel vm)
    {
        // 从列表中移除页面
        WhatsAppsPages.Remove(vm);
    }
}