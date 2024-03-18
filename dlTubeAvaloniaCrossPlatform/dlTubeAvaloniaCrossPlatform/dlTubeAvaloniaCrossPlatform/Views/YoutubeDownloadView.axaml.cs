using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views;

public sealed partial class YoutubeDownloadView : UserControl
{
    public YoutubeDownloadView()
    {
        InitializeComponent();
        this.DataContext = new YoutubeDownloaderViewModel();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}