using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace dlTubeAvalonia.Views;

public sealed partial class ArchiveView : UserControl
{
    public ArchiveView()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}