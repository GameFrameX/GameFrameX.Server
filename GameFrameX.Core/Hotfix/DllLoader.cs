using System.Reflection;
using System.Runtime.Loader;

namespace GameFrameX.Core.Hotfix;

/// <summary>
/// DLL加载器，用于动态加载和卸载DLL。
/// </summary>
internal sealed class DllLoader
{
    /// <summary>
    /// 构造函数，初始化DLL加载器并加载指定路径的DLL。
    /// </summary>
    /// <param name="dllPath">DLL文件路径。</param>
    public DllLoader(string dllPath)
    {
        Context = new HostAssemblyLoadContext();
        HotfixDll = Context.LoadFromAssemblyPath(dllPath);
    }

    /// <summary>
    /// 热更DLL。
    /// </summary>
    public Assembly HotfixDll { get; }

    /// <summary>
    /// 加载上下文。
    /// </summary>
    private HostAssemblyLoadContext Context { get; }

    /// <summary>
    /// 卸载DLL。
    /// </summary>
    /// <returns>弱引用，用于检查上下文是否已释放。</returns>
    public WeakReference Unload()
    {
        Context.Unload();
        return new WeakReference(Context);
    }

    /// <summary>
    /// 主机程序集加载上下文，用于管理DLL的加载和卸载。
    /// </summary>
    private class HostAssemblyLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// 构造函数，初始化加载上下文。
        /// </summary>
        public HostAssemblyLoadContext() : base(true)
        {
        }

        /// <summary>
        /// 覆盖默认的程序集加载行为。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <returns>加载的程序集，如果没有找到则返回null。</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}