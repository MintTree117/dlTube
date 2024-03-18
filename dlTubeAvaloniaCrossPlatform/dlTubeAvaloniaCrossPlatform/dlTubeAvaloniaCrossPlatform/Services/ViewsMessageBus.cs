using System;
using Avalonia.Controls;

namespace dlTubeAvaloniaCrossPlatform.Services;

public class ViewsMessageBus
{
    static ViewsMessageBus? _instance;

    public static ViewsMessageBus Instance
    {
        get
        {
            _instance ??= new ViewsMessageBus();
            return _instance;
        }
    }

    public event Action<UserControl>? MobileViewChanged;
    public void InvokeMobileViewChanged( UserControl view )
    {
        MobileViewChanged?.Invoke( view );
    }
}