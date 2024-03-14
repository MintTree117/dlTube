using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace dlTubeAvalonia.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}