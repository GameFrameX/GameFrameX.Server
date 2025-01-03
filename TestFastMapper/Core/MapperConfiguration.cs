// ========================================================
// 描述：MapperConfiguration.cs
// 作者：Bambomtan 
// 创建时间：2024-12-31 16:14:26 星期二 
// Email：837659628@qq.com
// 版 本：1.0
// ========================================================
// MapperConfiguration.cs


using FastMapper.Models;

namespace FastMapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapperAttribute : Attribute
    {
        public MapperAttribute(string sourceTypeName, string destTypeName, bool isBidirectional = false)
        {
            SourceTypeName = sourceTypeName;
            DestTypeName = destTypeName;
            IsBidirectional = isBidirectional;
        }

        public string SourceTypeName { get; }
        public string DestTypeName { get; }
        public bool IsBidirectional { get; }
    }
    
    // 定义映射配置的静态类
    // 使用 MapperAttribute 标记需要生成映射的类
    //[Mapper("SourceClass", "DestClass", true)]
    //public partial class ConfigMapper { }
    
    //[ManualMapper(typeof(SourceClass), typeof(DestClass))]
   // public class UserMapper { }
}
