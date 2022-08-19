namespace EnkuPrismExtension.Test.ViewModels;

using Xunit;
using EnkuPrismExtension.ViewModels;
using Moq;
using Prism.Regions;

public class ViewModelBaseTest
{
    private Mock<IRegionNavigationService> _regionNavigationService;
    private Mock<IRegionNavigationJournal> _regionNavigationJournal;
    private NavigationContext _navigationContext;

    public ViewModelBaseTest()
    {
        this._regionNavigationService = new();
        this._regionNavigationJournal = new();
        this._regionNavigationService.Setup(rns => rns.Journal).Returns(this._regionNavigationJournal.Object);
        this._navigationContext = new(this._regionNavigationService.Object, null);
    }

    #region KeepAlive
    [Fact]
    public void KeepAlive_ゲッターの戻り値チェック_Trueを返す()
    {
        // テスト対象の用意
        var viewModelBase = new ViewModelBase();

        // 検証
        var result = viewModelBase.KeepAlive;
        Assert.True(result);
    }
    #endregion

    #region IsNavigationTarget
    [Fact]
    public void IsNavigationTarget_メソッドの戻り値チェック_Trueを返す()
    {
        // テスト対象の用意
        var viewModelBase = new ViewModelBase();

        // 検証
        var result = viewModelBase.IsNavigationTarget(this._navigationContext);
        Assert.True(result);
    }
    #endregion

    #region PersistInHistory
    [Fact]
    public void PersistInHistory_メソッドの戻り値チェック_Trueを返す()
    {
        // テスト対象の用意
        var viewModelBase = new ViewModelBase();

        //検証
        var result = viewModelBase.PersistInHistory();
        Assert.True(result);
    }
    #endregion

    #region NavigateCommand
    [Fact]
    public void NavigateCommand_OnNavigatedToメソッドが呼ばれる前にCanExecuteメソッド実行_Falseを返す()
    {
        // テスト対象やダミーデータの用意
        const string DUMMY_VIEW_NAME = nameof(DUMMY_VIEW_NAME);
        var viewModelBase = new ViewModelBase();

        // 検証
        var result = viewModelBase.NavigateCommand.CanExecute(DUMMY_VIEW_NAME);
        Assert.False(result);
    }

    [Fact]
    public void NavigateCommand_OnNavigatedToメソッドが呼ばれた後にCanExecuteメソッド実行_Trueを返す()
    {
        // テスト対象やダミーデータの用意
        const string DUMMY_VIEW_NAME = nameof(DUMMY_VIEW_NAME);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // 検証
        var result = viewModelBase.NavigateCommand.CanExecute(DUMMY_VIEW_NAME);
        Assert.True(result);
    }

    [Fact]
    public void NavigateCommand_コマンド実行時_RegionNavigationServiceのRequestNavigateメソッドが実行される()
    {
        // テスト対象やダミーデータの用意
        const string DUMMY_VIEW_NAME = nameof(DUMMY_VIEW_NAME);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);
        viewModelBase.NavigateCommand.Execute(DUMMY_VIEW_NAME);

        // 検証
        this._regionNavigationService.Verify(rns => rns.RequestNavigate(new Uri(DUMMY_VIEW_NAME, UriKind.Relative), It.IsAny<Action<NavigationResult>>(), (NavigationParameters?)null));
    }

    [Fact]
    public void NavigateCommand_他Viewからの画面遷移直後_コマンドが実行可能かチェックされる()
    {
        // テスト対象の用意
        var result = false;
        var viewModelBase = new ViewModelBase();

        // CommandのCanExecuteChangedイベントにフックするイベントの用意
        EventHandler? onCanExecuteChanged = null;
        onCanExecuteChanged = (sender, e) =>
        {
            // CanExecuteChangedイベントが発生したらresultがtrueになる
            result = true;
            viewModelBase.NavigateCommand.CanExecuteChanged -= onCanExecuteChanged;
        };
        viewModelBase.NavigateCommand.CanExecuteChanged += onCanExecuteChanged;
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // NavigationContextのNavigatedイベントを発生させる
        this._regionNavigationService.Raise(rns => rns.Navigated += null, new RegionNavigationEventArgs(this._navigationContext));
        
        // 検証
        Assert.True(result);
    }
    #endregion

    #region GoBackCommand
    [Fact]
    public void GoBackCommand_OnNavigatedToメソッドが呼ばれる前にCanExecuteメソッド実行_Falseを返す()
    {
        // テスト対象の用意
        var viewModelBase = new ViewModelBase();
        var result = viewModelBase.GoBackCommand.CanExecute();

        // 検証
        Assert.False(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GoBackCommand_OnNavigatedToメソッドが呼ばれた後にCanExecuteメソッド実行_RegionNavigationJournalのCanGoBackプロパティと同じ値が返される(bool canGoBack)
    {
        // テスト対象の用意
        this._regionNavigationJournal.Setup(rnj => rnj.CanGoBack).Returns(canGoBack);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // 検証
        var result = viewModelBase.GoBackCommand.CanExecute();
        Assert.Equal(result, canGoBack);
    }

    [Fact]
    public void GoBackCommand_コマンド実行時_RegionNavigationJournalのGoBackメソッドが実行される()
    {
        // テスト対象の用意
        this._regionNavigationJournal.Setup(rnj => rnj.CanGoBack).Returns(true);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);
        viewModelBase.GoBackCommand.Execute();

        // 検証
        this._regionNavigationJournal.Verify(rnj => rnj.GoBack());
    }

    [Fact]
    public void GoBackCommand_他Viewからの画面遷移直後_コマンドが実行可能かチェックされる()
    {
        // テスト対象の用意
        var result = false;
        var viewModelBase = new ViewModelBase();

        // CommandのCanExecuteChangedイベントにフックするイベントの用意
        EventHandler? onCanExecuteChanged = null;
        onCanExecuteChanged = (sender, e) =>
        {
            // CanExecuteChangedイベントが発生したらresultがtrueになる
            result = true;
            viewModelBase.GoBackCommand.CanExecuteChanged -= onCanExecuteChanged;
        };
        viewModelBase.GoBackCommand.CanExecuteChanged += onCanExecuteChanged;
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // NavigationContextのNavigatedイベントを発生させる
        this._regionNavigationService.Raise(rns => rns.Navigated += null, new RegionNavigationEventArgs(this._navigationContext));
        
        // 検証
        Assert.True(result);
    }
    #endregion

    #region GoForwardCommand
    [Fact]
    public void GoForwardCommand_OnNavigatedToメソッドが呼ばれる前にCanExecuteメソッド実行_Falseを返す()
    {
        // テスト対象の用意
        var viewModelBase = new ViewModelBase();
        var result = viewModelBase.GoForwardCommand.CanExecute();

        // 検証
        Assert.False(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GoForwardCommand_OnNavigatedToメソッドが呼ばれた後にCanExecuteメソッド実行_RegionNavigationJournalのCanGoBackプロパティと同じ値が返される(bool canGoForward)
    {
        // テスト対象の用意
        this._regionNavigationJournal.Setup(rnj => rnj.CanGoForward).Returns(canGoForward);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // 検証
        var result = viewModelBase.GoForwardCommand.CanExecute();
        Assert.Equal(result, canGoForward);
    }

    [Fact]
    public void GoForwardCommand_コマンド実行時_RegionNavigationJournalのGoForwardメソッドが実行される()
    {
        // テスト対象の用意
        this._regionNavigationJournal.Setup(rnj => rnj.CanGoForward).Returns(true);
        var viewModelBase = new ViewModelBase();
        viewModelBase.OnNavigatedTo(this._navigationContext);
        viewModelBase.GoForwardCommand.Execute();

        // 検証
        this._regionNavigationJournal.Verify(rnj => rnj.GoForward());
    }

    [Fact]
    public void GoForwardCommand_他Viewからの画面遷移直後_コマンドが実行可能かチェックされる()
    {
        // テスト対象の用意
        var result = false;
        var viewModelBase = new ViewModelBase();

        // CommandのCanExecuteChangedイベントにフックするイベントの用意
        EventHandler? onCanExecuteChanged = null;
        onCanExecuteChanged = (sender, e) =>
        {
            // CanExecuteChangedイベントが発生したらresultがtrueになる
            result = true;
            viewModelBase.GoForwardCommand.CanExecuteChanged -= onCanExecuteChanged;
        };
        viewModelBase.GoForwardCommand.CanExecuteChanged += onCanExecuteChanged;
        viewModelBase.OnNavigatedTo(this._navigationContext);

        // NavigationContextのNavigatedイベントを発生させる
        this._regionNavigationService.Raise(rns => rns.Navigated += null, new RegionNavigationEventArgs(this._navigationContext));
        
        // 検証
        Assert.True(result);
    }
    #endregion
}
