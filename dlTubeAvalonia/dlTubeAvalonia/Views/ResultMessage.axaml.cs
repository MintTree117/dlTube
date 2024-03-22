using AngleSharp.Browser;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace dlTubeAvalonia.Views;

public partial class ResultMessage : UserControl
{
    public bool HideOnClose { get; set; }
    public string Text
    {
        get => GetValue( TextProperty );
        set => SetValue( TextProperty, value );
    }
    
    // Define the Text property using Avalonia's property system
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ResultMessage, string>( nameof( Text ), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay );
    public static readonly StyledProperty<ICommand> CloseCommandProperty = AvaloniaProperty.Register<ResultMessage, ICommand>( nameof( CloseCommand ) );
    public ICommand CloseCommand
    {
        get => GetValue( CloseCommandProperty );
        set => SetValue( CloseCommandProperty, value );
    }
    
    // Constructor
    public ResultMessage()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }

    void Close( object? sender, RoutedEventArgs args )
    {
        IsVisible = false;
    }
}