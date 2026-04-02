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

using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装。
/// </summary>
/// <remarks>
/// Static utility class for game database operations, providing basic database operation encapsulation.
/// </remarks>
public static partial class GameDb
{
    /// <summary>
    /// 数据库服务实现实例。
    /// </summary>
    /// <remarks>
    /// Database service implementation instance.
    /// </remarks>
    private static IDatabaseService _dbServiceImplementation;

    /// <summary>
    /// 初始化GameDb。
    /// </summary>
    /// <remarks>
    /// Initialize the GameDb instance.
    /// </remarks>
    /// <typeparam name="T">数据库服务的具体实现类型,必须实现IDatabaseService接口且有无参构造函数 / Database service implementation type, must implement IDatabaseService interface and have a parameterless constructor</typeparam>
    /// <param name="dbOptions">数据库配置选项 / Database configuration options</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="dbOptions"/> 的 ConnectionString 或 Name 为 null 时抛出 / Thrown when ConnectionString or Name of <paramref name="dbOptions"/> is null</exception>
    /// <returns>返回数据库是否初始化成功 / Returns whether the database was initialized successfully</returns>
    public static async Task<bool> Init<T>(DbOptions dbOptions) where T : IDatabaseService, new()
    {
        ArgumentNullException.ThrowIfNull(dbOptions, nameof(dbOptions));
        ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
        ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
        var service = new T();
        var isOpened = await service.Open(dbOptions);
        if (!isOpened)
        {
            return false;
        }

        _dbServiceImplementation = service;
        return true;
    }

    /// <summary>
    /// 以指定类型获取数据库服务实例。
    /// </summary>
    /// <remarks>
    /// Get the database service instance as the specified type.
    /// </remarks>
    /// <typeparam name="T">要转换的数据库服务类型,必须实现IDatabaseService接口 / Database service type to convert to, must implement IDatabaseService interface</typeparam>
    /// <returns>转换后的数据库服务实例 / The converted database service instance</returns>
    /// <exception cref="InvalidCastException">当类型转换失败时抛出 / Thrown when type conversion fails</exception>
    public static T As<T>() where T : IDatabaseService
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return (T)_dbServiceImplementation;
    }

    /// <summary>
    /// 关闭数据库连接。
    /// </summary>
    /// <remarks>
    /// Close the database connection.
    /// </remarks>
    public static void Close()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        CloseAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步关闭数据库连接。
    /// </summary>
    /// <remarks>
    /// Asynchronously closes the database connection.
    /// </remarks>
    /// <returns>表示异步关闭操作的任务 / Task representing the asynchronous close operation</returns>
    public static Task CloseAsync()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.Close();
    }

}
