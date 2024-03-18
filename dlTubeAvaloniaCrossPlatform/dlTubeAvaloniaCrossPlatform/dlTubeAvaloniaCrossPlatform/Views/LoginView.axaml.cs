using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace dlTubeAvaloniaCrossPlatform.Views;

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