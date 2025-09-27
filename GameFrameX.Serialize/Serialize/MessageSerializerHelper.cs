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

using Microsoft.IO;
using ProtoBuf.Meta;

namespace GameFrameX.Serialize.Serialize
{
    /// <summary>
    /// 消息序列化帮助类
    /// </summary>
    public static class MessageSerializerHelper
    {
        private static readonly RecyclableMemoryStreamManager Manager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] Serialize<T>(T value)
        {
            using (var memoryStream = Manager.GetStream())
            {
                ProtoBuf.Serializer.Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="type"></param>
        public static void Register(Type type)
        {
            RuntimeTypeModel.Default.Add(type, false);
        }

        /// <summary>
        /// 反序列化数据对象
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object Deserialize(byte[] data, Type type)
        {
            using (var memoryStream = Manager.GetStream(data))
            {
                return ProtoBuf.Serializer.Deserialize(type, memoryStream);
            }
        }

        /// <summary>
        /// 反序列化数据对象
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data) where T : class, new()
        {
            using (var memoryStream = Manager.GetStream(data))
            {
                return ProtoBuf.Serializer.Deserialize(typeof(T), memoryStream) as T;
            }
        }
    }
}