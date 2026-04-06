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

using GameFrameX.Foundation.Orm.Entity.Filter;

namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 缓存数据对象接口。
/// </summary>
/// <remarks>
/// Cache data object interface.
/// </remarks>
public interface ICacheState : ISafeDeletedFilter, ISafeCreatedFilter, ISafeUpdateFilter
{
    /// <summary>
    /// 唯一ID。
    /// </summary>
    /// <remarks>
    /// Unique identifier.
    /// </remarks>
    /// <value>唯一ID / Unique ID</value>
    long Id { get; set; }

    /// <summary>
    /// 是否修改。
    /// </summary>
    /// <remarks>
    /// Whether the object has been modified.
    /// </remarks>
    /// <returns>如果已修改则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if modified; otherwise <c>false</c></returns>
    bool IsModify();

    /// <summary>
    /// 用于在对象从数据库加载后进行一些特定的处理，如初始化数据或设置状态。
    /// </summary>
    /// <remarks>
    /// Used to perform specific processing after the object is loaded from the database, such as initializing data or setting state.
    /// </remarks>
    /// <param name="isNew">是否是新对象 / Whether it is a new object</param>
    void LoadFromDbPostHandler(bool isNew);

    /// <summary>
    /// 在对象保存到数据库后调用的方法，可以进行一些后续处理。
    /// </summary>
    /// <remarks>
    /// Method called after the object is saved to the database, can perform some post-processing.
    /// </remarks>
    void SaveToDbPostHandler();

    /// <summary>
    /// 将对象序列化转换为字节数组。
    /// </summary>
    /// <remarks>
    /// Serialize the object to a byte array.
    /// </remarks>
    /// <returns>序列化后的字节数组 / Serialized byte array</returns>
    byte[] ToBytes();
}