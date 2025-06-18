using CommunityToolkit.Mvvm.ComponentModel;

namespace BarrichCSSystem.ViewModels;

public partial class WhatsAppBrowserViewModel: ObservableObject
{
    public required string UniqKey { get; set; } = string.Empty;
}