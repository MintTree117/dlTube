using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class DownloadView : UserControl
{
    public DownloadView()
    {
        InitializeComponent();
        this.DataContext = new DownloaderViewModel();
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}