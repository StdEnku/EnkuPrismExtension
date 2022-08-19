namespace EnkuPrismExtension.Commands;

using System;
using System.Windows.Input;
using Prism.Regions;

/// <summary>
/// Regionの外側からNavigateを行うためのコマンド
/// </summary>
public class RegionNavigateCommand : ICommand
{
    private readonly string _regionName;
    private readonly IRegionManager _regionManager;
    private Func<NavigationParameters>? _beforeNavigateAction = null;
    private Action<NavigationResult>? _afterNavigateAction = null;

    /// <summary>
    /// コマンドを実行するかどうかに影響するような変更があった場合に発生します。
    /// </summary>
    /// <remarks>
    /// 本コマンドではCanExecuteメソッドの結果が常にTrueなので使用しない
    /// </remarks>
#pragma warning disable 0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 0067

    /// <summary>
    /// コマンドが実行可能かチェックするためのメソッド
    /// </remarks>
    /// <param name="parameter">コマンドパラメータ、本コマンドでは無視される</param>
    /// <returns>チェック結果、本コマンドでは常にtrueを返す</returns>
    public bool CanExecute(object? parameter)
        => true;

    /// <summary>
    /// コマンド実行時にNavigateを行うメソッド
    /// </summary>
    /// <param name="parameter">遷移先のView名</param>
    public void Execute(object? parameter)
    {
        if (parameter is string nextViewName)
        {
            var region = this._regionManager.Regions[this._regionName];

            // 現在のView名と新しいView名が一致する場合画面遷移は行われない
            var currentViewName = region.NavigationService.Journal.CurrentEntry?.Uri.ToString();
            if (nextViewName.Equals(currentViewName))
                return;

            // 画面遷移時のパラメータを取得or作成
            var navigationParams = this._beforeNavigateAction is null ? 
                                   new NavigationParameters() : 
                                   this._beforeNavigateAction();

            // RequestNavigate実行
            if (this._afterNavigateAction is not null)
            {
                region.RequestNavigate(nextViewName, this._afterNavigateAction, navigationParams);
            }
            else
            {
                region.RequestNavigate(nextViewName, navigationParams);
            }
        }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="regionManager">IRegionManagerオブジェクト</param>
    /// <param name="regionName">対象のRegion名</param>
    /// <param name="beforeNavigateAction">Navigate直前に実行される処理</param>
    /// <param name="afterNavigateAction">Navigate直後に実行される処理</param>
    public RegionNavigateCommand(IRegionManager regionManager, 
                                 string regionName,
                                 Func<NavigationParameters>? beforeNavigateAction = null,
                                 Action<NavigationResult>? afterNavigateAction = null)
    {
        this._regionName = regionName;
        this._regionManager = regionManager;
        this._beforeNavigateAction = beforeNavigateAction;
        this._afterNavigateAction = afterNavigateAction;
    }
}