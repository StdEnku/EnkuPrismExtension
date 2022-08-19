namespace Demo;

using Demo.Views;
using Prism.Ioc;
using System.Windows;
using Demo.Constants;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<Control1>(ViewNames.Control1);
        containerRegistry.RegisterForNavigation<Control2>(ViewNames.Control2);
    }
}
