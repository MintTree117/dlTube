using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views.Mobile;

public partial class YoutubeSearchViewMobile : UserControl
{
    public YoutubeSearchViewMobile()
    {
        InitializeComponent();
        this.DataContext = new YoutubeSearchViewModel();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}