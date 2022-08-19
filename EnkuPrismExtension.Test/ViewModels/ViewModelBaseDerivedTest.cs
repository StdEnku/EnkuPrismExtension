namespace EnkuPrismExtension.Test.ViewModels;

using Xunit;
using EnkuPrismExtension.ViewModels;
using Moq;
using Prism.Regions;

#region テストで使用するダミーのViewModelBase派生クラス
class Derived : ViewModelBase
{
    // BeforeGoBackメソッドが実行されたらtrueとなるフィールド
    public bool isExecutedBeforeGoBack = false;

    // BeforeGoForwardメソッドが実行されたらtrueとなるフィールド
    public bool isExecutedBeforeGoForward = false;

    // ダミーのNavigationParameters
    public static NavigationParameters NavigationParameters;

    static Derived()
    {
        NavigationParameters = new NavigationParameters();
        NavigationParameters.Add("Key1", "Value1");
        NavigationParameters.Add("Key2", "Value2");
        NavigationParameters.Add("Key3", "Value3");
    }

    protected override void BeforeGoBack()
    {
        this.isExecutedBeforeGoBack = true;
    }

    protected override void BeforeGoForward()
    {
        this.isExecutedBeforeGoForward = true;
    }

    protected override NavigationParameters? BeforeNavigate(string nextViewName)
    {
        return NavigationParameters;
    }
}
#endregion

public class ViewModelBaseDerivedTest
{
    private Mock<IRegionNavigationService> _regionNavigationService;
    private Mock<IRegionNavigationJournal> _regionNavigationJournal;
    private NavigationContext _navigationContext;

    public ViewModelBaseDerivedTest()
    {
        this._regionNavigationService = new();
        this._regionNavigationJournal = new();
        this._regionNavigationService.Setup(rns => rns.Journal).Returns(this._regionNavigationJournal.Object);
        this._navigationContext = new(this._regionNavigationService.Object, null);
    }

    [Fact]
    public void BeforeGoBack_GoBackCommand実行後_BeforeGoBackメソッドが実行されている()
    {
        // テスト対象の用意
        var target = new Derived();
        target.OnNavigatedTo(this._navigationContext);

        // GoBackCommandの実行
        target.GoBackCommand.Execute();

        // 結果確認
        var result = target.isExecutedBeforeGoBack;
        Assert.True(result);
    }

    [Fact]
    public void BeforeGoForward_GoForwardCommand実行後_BeforeGoForwardメソッドが実行されている()
    {
        // テスト対象の用意
        var target = new Derived();
        target.OnNavigatedTo(this._navigationContext);

        // GoForwardCommandの実行
        target.GoForwardCommand.Execute();

        // 結果確認
        var result = target.isExecutedBeforeGoForward;
        Assert.True(result);
    }

    [Fact]
    public void BeforeNavigate_NavigateCommand実行後_RegionNavigationServiceのRequestNavigateが正しく呼び出されているか()
    {
        // テスト対象の用意
        const string NEXT_VIEW_NAME = "next view";
        var target = new Derived();
        target.OnNavigatedTo(this._navigationContext);

        // GoBackCommandの実行
        target.NavigateCommand.Execute(NEXT_VIEW_NAME);

        // 結果確認
        this._regionNavigationService.Verify(rns => rns.RequestNavigate(new Uri(NEXT_VIEW_NAME, UriKind.Relative), It.IsAny<Action<NavigationResult>>(), Derived.NavigationParameters));
    }
}