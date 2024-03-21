using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class YtDownloaderView : UserControl
{
    public YtDownloaderView()
    {
        InitializeComponent();
        this.DataContext = new YtDownloaderViewModel();
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}