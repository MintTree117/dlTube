using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class YoutubeDownloadView : UserControl
{
    public YoutubeDownloadView()
    {
        InitializeComponent();
        this.DataContext = new YoutubeViewModel();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}