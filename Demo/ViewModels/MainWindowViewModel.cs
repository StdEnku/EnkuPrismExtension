namespace Demo.ViewModels;

using Prism.Mvvm;
using EnkuPrismExtension.Commands;
using Prism.Regions;
using Demo.Constants;

public class MainWindowViewModel : BindableBase
{
    public RegionNavigateCommand RootRegionNavigateCommand { get; private set; }

    public MainWindowViewModel(IRegionManager regionManager)
    {
        this.RootRegionNavigateCommand = new(regionManager, RegionNames.RootRegion);
    }
}
