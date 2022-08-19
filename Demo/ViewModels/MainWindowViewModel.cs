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
    }

    private NavigationParameters onNavigationg()
    {
        System.Diagnostics.Debug.WriteLine("navigating");
        return new NavigationParameters();
    }

    public MainWindowViewModel(IRegionManager regionManager)
    {
        this.RootRegionNavigateCommand = new(regionManager, RegionNames.RootRegion, this.onNavigationg, this.onNavigated);
    }
}
