namespace Demo.ViewModels;

using Prism.Mvvm;
using EnkuPrismExtension.Commands;
using Prism.Regions;
using Demo.Constants;

public class MainWindowViewModel : BindableBase
{
    public RegionNavigateCommand RootRegionNavigateCommand { get; private set; }

    private void onNavigated(NavigationResult navigationResult)
    {
        System.Diagnostics.Debug.WriteLine("navigated");
        this.RootRegionGoBackCommand.CheckCanExecute();
        this.RootRegionGoForwardCommand.CheckCanExecute();
    }

    private NavigationParameters onNavigationg()
    {
        System.Diagnostics.Debug.WriteLine("navigating");
        return new NavigationParameters();
    }

    public RegionGoBackCommand RootRegionGoBackCommand { get; private set; }
    private void onGoBacking()
    {
        System.Diagnostics.Debug.WriteLine("backing");
    }

    private void onGoBacked()
    {
        System.Diagnostics.Debug.WriteLine("backed");
        this.RootRegionGoBackCommand.CheckCanExecute();
        this.RootRegionGoForwardCommand.CheckCanExecute();
    }

    public RegionGoForwardCommand RootRegionGoForwardCommand { get; private set; }
    private void onGoForwarding()
    {
        System.Diagnostics.Debug.WriteLine("forwarding");
    }

    private void onGoForwarded()
    {
        System.Diagnostics.Debug.WriteLine("forwarded");
        this.RootRegionGoBackCommand.CheckCanExecute();
        this.RootRegionGoForwardCommand.CheckCanExecute();
    }


    public MainWindowViewModel(IRegionManager regionManager)
    {
        this.RootRegionNavigateCommand = new(regionManager, RegionNames.RootRegion, this.onNavigationg, this.onNavigated);
        this.RootRegionGoBackCommand = new(regionManager, RegionNames.RootRegion, this.onGoBacking, this.onGoBacked);
        this.RootRegionGoForwardCommand = new(regionManager, RegionNames.RootRegion, this.onGoForwarding, this.onGoForwarded);
    }
}
