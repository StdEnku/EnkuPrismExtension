namespace EnkuPrismExtension.ViewModels;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;

/// <summary>
/// Region内で表示されるView用の基底ViewModelクラス
/// </summary>
public class ViewModelBase : BindableBase, INavigationAware, IRegionMemberLifetime, IJournalAware
{
    // フィールド定義
    protected IRegionNavigationService? RegionNavigationService;

    // 各インターフェースの実装
    #region IRegionMemberLifetimeの実装
    /// <summary>
    /// 遷移後も本ViewModelに対応するViewのインスタンスを破棄しないか
    /// </summary>
    /// <remarks>
    /// trueなら遷移後本ViewModelに対応するViewのインスタンスは破棄されない
    /// falseなら遷移後本ViewModelに対応するViewのインスタンスは破棄される
    /// </remarks>
    public virtual bool KeepAlive => true;
    #endregion

    #region InavigationAwareの実装
    /// <summary>
    /// 再遷移後本ViewModelに対応するViewのインスタンスを再利用するか
    /// </summary>
    /// <param name="navigationContext"></param>
    /// <returns>
    /// trueなら再遷移後本ViewModelに対応するViewの旧インスタンスは再利用される
    /// falseなら再選以後本ViewModelに対応するViewの新たなインスタンスが作成される
    /// </returns>
    public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        => true;

    /// <summary>
    /// 他の画面から本ViewModelに対応するViewへ遷移した直後実行される処理
    /// </summary>
    /// <param name="navigationContext">画面遷移の情報を保持するオブジェクト</param>
    public virtual void OnNavigatedTo(NavigationContext navigationContext)
    {
        this.RegionNavigationService = navigationContext.NavigationService;
        this.RegionNavigationService.Navigated += this.onNavigatedInsideRegion;
    }

    /// <summary>
    /// 本ViewModelに対応するViewから他の画面に遷移する直前に実行される処理
    /// </summary>
    /// <param name="navigationContext">画面遷移の情報を保持するオブジェクト</param>
    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    {
        if (this.RegionNavigationService is not null)
        {
            this.RegionNavigationService.Navigated -= this.onNavigatedInsideRegion;
            this.RegionNavigationService = null;
        }
    }

    // 他Viewからの画面遷移完了後にコマンド実行可能かチェックするメソッド
    private void onNavigatedInsideRegion(object? sender, EventArgs e)
    {
        this.NavigateCommand.RaiseCanExecuteChanged();
        this.GoBackCommand.RaiseCanExecuteChanged();
        this.GoForwardCommand.RaiseCanExecuteChanged();
    }
    #endregion

    #region IJounalAwareの実装
    /// <summary>
    /// NavigationJournalに対応するViewを記録するかどうか
    /// </summary>
    /// <returns>trueなら記録する</returns>
    public virtual bool PersistInHistory() => true;
    #endregion

    // Regionの内側からの画面遷移用コマンド
    #region NavigateCommand
    /// <summary>
    /// 本ViewModelに対応するViewを表示しているRegionでの画面遷移用コマンド
    /// </summary>
    public DelegateCommand<string> NavigateCommand { get; private set; }

    private void navigate(string nextViewName)
    {
        var navigateParams = this.BeforeNavigate(nextViewName);

        Debug.Assert(this.RegionNavigationService is not null);

        // 現在のView名と新しいView名が一致する場合画面遷移は行われない
        var currentViewName = this.RegionNavigationService.Journal.CurrentEntry?.Uri.ToString();
        if (nextViewName.Equals(currentViewName))
            return;

        this.RegionNavigationService.RequestNavigate(nextViewName, navigateParams);
    }

    private bool canNavigate(string nextViewName)
        => this.RegionNavigationService is not null;
    #endregion

    #region GoBackCommand
    /// <summary>
    /// 本ViewModelに対応するViewを表示しているRegionでのGoBack用コマンド
    /// </summary>
    public DelegateCommand GoBackCommand { get; private set; }

    private void goBack()
    {
        this.BeforeGoBack();
        Debug.Assert(this.RegionNavigationService is not null);
        Debug.Assert(this.RegionNavigationService.Journal is not null);
        this.RegionNavigationService.Journal.GoBack();
    }

    private bool canGoBack()
    {
        var regionNavigationService = this.RegionNavigationService;
        if (regionNavigationService is null)
        {
            return false;
        }

        return regionNavigationService.Journal.CanGoBack;
    }
    #endregion

    #region GoForwardCommand
    /// <summary>
    /// 本ViewModelに対応するViewを表示しているRegionでのGoForward用コマンド
    /// </summary>
    public DelegateCommand GoForwardCommand { get; private set; }

    private void goForward()
    {
        this.BeforeGoForward();
        Debug.Assert(this.RegionNavigationService is not null);
        Debug.Assert(this.RegionNavigationService.Journal is not null);
        this.RegionNavigationService.Journal.GoForward();
    }

    private bool canGoForward()
    {
        var regionNavigationService = this.RegionNavigationService;
        if (regionNavigationService is null)
        {
            return false;
        }

        return regionNavigationService.Journal.CanGoForward;
    }
    #endregion

    // Regionの内側からの画面遷移の直前に実行されるメソッド
    #region NavigateCommandによって画面遷移が行われる直前に実行されるメソッド
    /// <summary>
    /// NavigateCommandによって画面遷移が行われる直前に実行されるメソッド
    /// </summary>
    /// <param name="nextViewName">遷移後表示するView名</param>
    /// <returns>遷移先に渡すパラメータ群</returns>
    protected virtual NavigationParameters? BeforeNavigate(string nextViewName)
    {
        return null;
    }
    #endregion

    #region GoBackCommandによって画面遷移が行われる直前に実行されるメソッド
    /// <summary>
    /// GoBackCommandによって画面遷移が行われる直前に実行されるメソッド
    /// 継承先でオーバーライドして使うことを想定している
    /// </summary>
    protected virtual void BeforeGoBack()
    {

    }
    #endregion

    #region GoForwardCommandによって画面遷移が行われる直前に実行されるメソッド
    /// <summary>
    /// GoForwardCommandによって画面遷移が行われる直前に実行されるメソッド
    /// 継承先でオーバーライドして使うことを想定している
    /// </summary>
    protected virtual void BeforeGoForward()
    {

    }
    #endregion

    // コンストラクタ
    #region コンストラクタ
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="regionManager">DIコンテナから注入されるRegionManager</param>
    public ViewModelBase()
    {
        this.NavigateCommand = new DelegateCommand<string>(this.navigate, this.canNavigate);
        this.GoBackCommand = new DelegateCommand(this.goBack, this.canGoBack);
        this.GoForwardCommand = new DelegateCommand(this.goForward, this.canGoForward);
    }
    #endregion
}