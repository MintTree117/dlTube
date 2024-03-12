using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class YoutubeView : UserControl
{
    public YoutubeView()
    {
        InitializeComponent();
        this.DataContext = new YoutubeViewModel();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}