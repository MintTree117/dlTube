using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace dlTubeAvalonia.Views;

public partial class YoutubeView : UserControl
{
    public YoutubeView()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}