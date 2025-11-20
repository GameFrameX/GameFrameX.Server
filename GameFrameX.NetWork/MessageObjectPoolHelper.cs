// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.NetWork.Abstractions;
using Microsoft.Extensions.ObjectPool;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork;

/// <summary>
/// Static message object pool helper class / 静态消息对象池帮助类
/// Provides centralized management for object pools using Microsoft.Extensions.ObjectPool / 使用 Microsoft.Extensions.ObjectPool 提供对象池的集中管理
/// </summary>
/// <remarks>
/// Core features / 核心功能：
/// - Thread-safe object pool management / 线程安全的对象池管理
/// - Generic type support for different object types / 支持不同对象类型的泛型支持
/// - Automatic pool creation and caching / 自动池创建和缓存
/// - Automatic object reset to default values / 自动对象重置为默认值
/// - High-performance object reuse / 高性能对象重用
/// 
/// Usage scenarios / 使用场景：
/// - Message object pooling in high-frequency scenarios / 高频场景下的消息对象池化
/// - Reducing GC pressure through object reuse / 通过对象重用减少 GC 压力
/// - Performance optimization for network communication / 网络通信的性能优化
/// </remarks>
public static class MessageObjectPoolHelper
{
    private static readonly ConcurrentDictionary<Type, object> ObjectPools = new();
    private static readonly ConcurrentDictionary<Type, Action<object>> ReturnActions = new();
    private static readonly ConcurrentDictionary<Type, Func<object>> GetActions = new();
    private static readonly ObjectPoolProvider PoolProvider = new DefaultObjectPoolProvider();

    /// <summary>
    /// Custom pooled object policy that resets objects to default values / 自定义池化对象策略，将对象重置为默认值
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool / 池中对象的类型</typeparam>
    private class ResetObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, IMessageObject, new()
    {
        public T Create()
        {
            return new T();
        }

        public bool Return(T obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Reset object to default values / 将对象重置为默认值
            obj.Clear();
            return true;
        }
    }

    /// <summary>
    /// Gets or creates an object pool for the specified type / 获取或创建指定类型的对象池
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool / 池中对象的类型</typeparam>
    /// <returns>The object pool for the specified type / 指定类型的对象池</returns>
    public static ObjectPool<T> GetPool<T>() where T : class, IMessageObject, new()
    {
        var type = typeof(T);
        return (ObjectPool<T>)ObjectPools.GetOrAdd(type, _ => PoolProvider.Create(new ResetObjectPolicy<T>()));
    }

    /// <summary>
    /// Gets an object from the pool / 从池中获取对象
    /// </summary>
    /// <typeparam name="T">The type of object to get / 要获取的对象类型</typeparam>
    /// <returns>An object from the pool / 池中的对象</returns>
    public static T Get<T>() where T : class, IMessageObject, new()
    {
        return GetPool<T>().Get();
    }

    /// <summary>
    /// Gets an object from the pool by type / 根据类型从池中获取对象
    /// </summary>
    /// <param name="type">The type of object to get, must implement IMessageObject and have a parameterless constructor / 要获取的对象类型，必须实现 IMessageObject 并有无参构造函数</param>
    /// <returns>An object from the pool / 池中的对象</returns>
    /// <exception cref="ArgumentNullException">Thrown when type is null / 当 type 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">Thrown when type does not implement IMessageObject or does not have a parameterless constructor / 当 type 未实现 IMessageObject 或没有无参构造函数时抛出</exception>
    /// <remarks>
    /// This method uses cached delegates to minimize reflection overhead / 此方法使用缓存的委托来最小化反射开销
    /// Performance note: First call for each type has reflection overhead, subsequent calls use cached delegates / 性能说明：每种类型的首次调用有反射开销，后续调用使用缓存的委托
    /// </remarks>
    public static IMessageObject Get(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        // Validate that the type implements IMessageObject / 验证类型是否实现了 IMessageObject
        if (!typeof(IMessageObject).IsAssignableFrom(type))
        {
            throw new ArgumentException(LocalizationService.GetString(
                                            GameFrameX.Localization.Keys.NetWork.TypeMustImplementInterface,
                                            type.Name,
                                            nameof(IMessageObject)), nameof(type));
        }

        // Validate that the type is a class / 验证类型是否为类
        if (!type.IsClass)
        {
            throw new ArgumentException(LocalizationService.GetString(
                                            GameFrameX.Localization.Keys.NetWork.TypeMustBeClass,
                                            type.Name), nameof(type));
        }

        // Get or create cached get action for this type / 获取或创建此类型的缓存获取操作
        Func<object> ValueFactory(Type t)
        {
            // Check if the type has a parameterless constructor / 检查类型是否有无参构造函数
            var constructor = t.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new ArgumentException(LocalizationService.GetString(
                                                GameFrameX.Localization.Keys.NetWork.TypeMustHaveParameterlessConstructor,
                                                t.Name), nameof(type));
            }

            // Get the pool for the specified type / 获取指定类型的池
            var pool = GetPool(t);

            // Create a cached delegate for the Get method / 为 Get 方法创建缓存的委托
            var poolType = typeof(ObjectPool<>).MakeGenericType(t);
            var getMethod = poolType.GetMethod(nameof(ObjectPool<object>.Get))!;

            return (Func<object>)(() => getMethod.Invoke(pool, null)!);
        }

        var getAction = GetActions.GetOrAdd(type, ValueFactory);

        // Use the cached delegate to get the object / 使用缓存的委托获取对象
        return (IMessageObject)getAction();
    }

    private static object GetPool(Type type)
    {
        object ValueFactory(Type _)
        {
            var policyType = typeof(ResetObjectPolicy<>).MakeGenericType(type);
            var policy = Activator.CreateInstance(policyType);

            // Find the generic Create method / 查找泛型Create方法
            var createMethod = typeof(ObjectPoolProvider).GetMethods()
                                                         .FirstOrDefault(m => m.Name == nameof(ObjectPoolProvider.Create)
                                                                              && m.IsGenericMethodDefinition
                                                                              && m.GetParameters().Length == 1
                                                                              && m.GetParameters()[0].ParameterType.IsGenericType
                                                                              && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IPooledObjectPolicy<>));

            if (createMethod == null)
            {
                throw new InvalidOperationException(LocalizationService.GetString(
                                                        GameFrameX.Localization.Keys.NetWork.CannotFindCreateMethod));
            }

            var genericCreateMethod = createMethod.MakeGenericMethod(type);
            var result = genericCreateMethod.Invoke(PoolProvider, new[] { policy });

            if (result == null)
            {
                throw new InvalidOperationException(LocalizationService.GetString(
                                                        GameFrameX.Localization.Keys.NetWork.FailedToCreateObjectPool,
                                                        type.Name));
            }

            return result;
        }

        var pool = ObjectPools.GetOrAdd(type, ValueFactory);
        return pool;
    }

    /// <summary>
    /// Returns an object to the pool / 将对象返回到池中
    /// </summary>
    /// <typeparam name="T">The type of object to return / 要返回的对象类型</typeparam>
    /// <param name="obj">The object to return to the pool / 要返回到池中的对象</param>
    /// <exception cref="ArgumentNullException">Thrown when obj is null / 当 obj 为 null 时抛出</exception>
    public static void Return<T>(T obj) where T : class, IMessageObject, new()
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        GetPool<T>().Return(obj);
    }

    /// <summary>
    /// Returns an object to the pool without specifying generic type parameter / 将对象返回到池中而无需指定泛型类型参数
    /// </summary>
    /// <param name="obj">The object to return to the pool / 要返回到池中的对象</param>
    /// <exception cref="ArgumentNullException">Thrown when obj is null / 当 obj 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">Thrown when obj does not have a parameterless constructor / 当 obj 没有无参构造函数时抛出</exception>
    /// <remarks>
    /// This method uses cached delegates to minimize reflection overhead / 此方法使用缓存的委托来最小化反射开销
    /// It supports returning objects of derived types to their base type pools / 它支持将派生类型的对象返回到其基类型池中
    /// Performance note: First call for each type has reflection overhead, subsequent calls use cached delegates / 性能说明：每种类型的首次调用有反射开销，后续调用使用缓存的委托
    /// </remarks>
    public static void Return(IMessageObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var objType = obj.GetType();

        // Get or create cached return action for this type / 获取或创建此类型的缓存返回操作
        var returnAction = ReturnActions.GetOrAdd(objType, type =>
        {
            // Check if the type has a parameterless constructor / 检查类型是否有无参构造函数
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new ArgumentException(LocalizationService.GetString(
                                                GameFrameX.Localization.Keys.NetWork.TypeMustHaveParameterlessConstructor,
                                                type.Name), nameof(obj));
            }

            // Get the pool for the object's actual type / 获取对象实际类型的池
            var pool = GetPool(type);

            // Create a cached delegate for the Return method / 为 Return 方法创建缓存的委托
            var poolType = typeof(ObjectPool<>).MakeGenericType(type);
            var returnMethod = poolType.GetMethod(nameof(ObjectPool<object>.Return))!;

            return (Action<object>)(o => returnMethod.Invoke(pool, new[] { o }));
        });

        // Use the cached delegate to return the object / 使用缓存的委托返回对象
        returnAction(obj);
    }

    /// <summary>
    /// Clears all cached object pools and return actions / 清除所有缓存的对象池和返回操作
    /// </summary>
    /// <remarks>
    /// This method should be used with caution as it will remove all cached pools and delegates / 此方法应谨慎使用，因为它将删除所有缓存的池和委托
    /// Any existing references to pools will become invalid / 对池的任何现有引用都将变为无效
    /// </remarks>
    public static void ClearPools()
    {
        ObjectPools.Clear();
        ReturnActions.Clear();
        GetActions.Clear();
    }

    /// <summary>
    /// Gets the number of cached object pools / 获取缓存的对象池数量
    /// </summary>
    /// <returns>The number of cached pools / 缓存的池数量</returns>
    public static int PoolCount
    {
        get { return ObjectPools.Count; }
    }
}