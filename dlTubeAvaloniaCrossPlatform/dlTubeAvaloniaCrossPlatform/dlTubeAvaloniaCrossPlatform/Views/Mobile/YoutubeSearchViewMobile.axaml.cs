using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace dlTubeAvaloniaCrossPlatform.Views.Mobile;

public partial class YoutubeSearchViewMobile : UserControl
{
    public YoutubeSearchViewMobile()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}