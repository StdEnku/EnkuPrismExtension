namespace EnkuPrismExtension.Commands;

using Prism.Regions;
using System;
using System.Windows.Input;

/// <summary>
/// Regionの外側からGoBackを行うためのコマンド
/// </summary>
public class RegionGoBackCommand : ICommand
{
    private readonly string _regionName;
    private readonly IRegionManager _regionManager;
    private Action? _beforeGoBackAction = null;
    private Action? _afterGoBackAction = null;

    /// <summary>
    /// コマンドを実行するかどうかに影響するような変更があった場合に発生します。
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// コマンドが実行可能かチェックする
    /// </summary>
    public void CheckCanExecute()
    {
        if (this.CanExecuteChanged is not null)
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// コマンドが実行可能かチェックするためのメソッド
    /// </remarks>
    /// <param name="parameter">コマンドパラメータ、本コマンドでは無視される</param>
    /// <returns>チェック結果</returns>
    public bool CanExecute(object? parameter)
    {
        var isRootRegionExist = this._regionManager.Regions.ContainsRegionWithName(this._regionName);
        if (!isRootRegionExist)
        {
            return false;
        }

        var navigationService = this._regionManager.Regions[this._regionName].NavigationService;
        var journal = navigationService.Journal;
        return journal.CanGoBack;
    }

    /// <summary>
    /// コマンド実行時にGoBackを行うメソッド
    /// </summary>
    /// <param name="parameter">本コマンドでは使用しないため無視される</param>
    public void Execute(object? parameter)
    {
        var navigationService = this._regionManager.Regions[this._regionName].NavigationService;

        if (this._beforeGoBackAction is not null)
        {
            this._beforeGoBackAction();
        }

        if (this._afterGoBackAction is not null)
        {
            EventHandler<RegionNavigationEventArgs>? onGoForwarded = null;
            onGoForwarded = (s, e) =>
            {
                this._afterGoBackAction();
                navigationService.Navigated -= onGoForwarded;
            };
            navigationService.Navigated += onGoForwarded;
        }

        var journal = navigationService.Journal;
        journal.GoBack();
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="regionManager">IRegionManagerオブジェクト</param>
    /// <param name="regionName">対象のRegion名</param>
    /// <param name="beforeGoBackAction">GoBack直前に実行される処理</param>
    /// <param name="afterGoBackAction">GoBack直後に実行される処理</param>
    public RegionGoBackCommand(IRegionManager regionManager, 
                               string regionName, 
                               Action? beforeGoBackAction = null,
                               Action? afterGoBackAction = null)
    {
        this._regionName = regionName;
        this._regionManager = regionManager;
        this._beforeGoBackAction = beforeGoBackAction;
        this._afterGoBackAction = afterGoBackAction;
    }
}