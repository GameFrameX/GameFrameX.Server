// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


namespace GameFrameX.Localization;

/// <summary>
/// 本地化键常量定义 - Database 分部类
/// </summary>
public static partial class Keys
{
    /// <summary>
    /// 数据库模块日志键名
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// MongoDB服务初始化成功，连接字符串：{0}，数据库名称：{1}
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.InitializedSuccessfully
        /// 用途: MongoDB服务成功初始化时记录
        /// 参数: {0} - 连接字符串, {1} - 数据库名称
        /// </remarks>
        public const string MongoDbInitializedSuccessfully = "Database.MongoDb.InitializedSuccessfully";

        /// <summary>
        /// MongoDB服务初始化失败，连接字符串：{0}，数据库名称：{1}
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.InitializationFailed
        /// 用途: MongoDB服务初始化失败时记录
        /// 参数: {0} - 连接字符串, {1} - 数据库名称
        /// </remarks>
        public const string MongoDbInitializationFailed = "Database.MongoDb.InitializationFailed";

        /// <summary>
        /// MongoDbService 未初始化，Open() 未成功完成。
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.ServiceUnavailable
        /// 用途: 当 MongoDbService 尚未初始化就执行数据库操作时抛出
        /// </remarks>
        public const string MongoDbServiceUnavailable = "Database.MongoDb.ServiceUnavailable";

        /// <summary>
        /// ExecuteInTransactionAsync 所有重试均失败，异常未知。
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.ExecuteInTransactionFailed
        /// 用途: 事务执行重试全部失败后抛出
        /// </remarks>
        public const string MongoDbExecuteInTransactionFailed = "Database.MongoDb.ExecuteInTransactionFailed";

        /// <summary>
        /// CommitTransaction 所有重试均失败，异常未知。
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.CommitTransactionFailed
        /// 用途: 事务提交重试全部失败后抛出
        /// </remarks>
        public const string MongoDbCommitTransactionFailed = "Database.MongoDb.CommitTransactionFailed";

        /// <summary>
        /// MongoDbService.{0} 所有重试均失败，异常未知。
        /// </summary>
        /// <remarks>
        /// 键名: Database.MongoDb.OperationRetryFailed
        /// 用途: 通用操作重试全部失败后抛出
        /// 参数: {0} - 操作名称
        /// </remarks>
        public const string MongoDbOperationRetryFailed = "Database.MongoDb.OperationRetryFailed";
    }
}