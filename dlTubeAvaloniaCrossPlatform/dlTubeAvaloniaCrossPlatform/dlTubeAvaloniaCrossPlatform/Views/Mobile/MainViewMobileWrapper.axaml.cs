using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.Services;

namespace dlTubeAvaloniaCrossPlatform.Views.Mobile;

public partial class MainViewMobileWrapper : UserControl, IDisposable
{
    public MainViewMobileWrapper()
    {
        InitializeComponent();
        ViewsMessageBus.Instance.MobileViewChanged += OnChangeView;
    }
    public void Dispose()
    {
        ViewsMessageBus.Instance.MobileViewChanged -= OnChangeView;
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }

    void OnChangeView( UserControl view )
    {
        Content = view;
    }
}