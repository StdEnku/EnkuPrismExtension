namespace Demo.Constants;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// View名やRegion名を作成する際のヘルパー
/// </summary>
internal static class Helpers
{
    /// <summary>
    /// "アセンブリ名の文字列/引数の文字列"を返す静的メソッド
    /// 引数にはCallerMemberName属性が付いているので何も入力せずに使用してよい
    /// </summary>
    public static string CreateName([CallerMemberName] string? baseName = null)
    {
        Debug.Assert(baseName is not null);
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var result = assemblyName + '/' + baseName;
        return result;
    }
}