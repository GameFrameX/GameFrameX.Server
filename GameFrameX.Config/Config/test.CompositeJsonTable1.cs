
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.test
{
    public sealed partial class CompositeJsonTable1 : BeanBase
    {
        /*
        public CompositeJsonTable1(int Id, string X) 
        {
            this.Id = Id;
            this.X = X;
            PostInit();
        }        
        */

        public CompositeJsonTable1(JsonElement _buf) 
        {
            Id = _buf.GetProperty("id").GetInt32();
            X = _buf.GetProperty("x").GetString();
        }
    
        public static CompositeJsonTable1 DeserializeCompositeJsonTable1(JsonElement _buf)
        {
            return new test.CompositeJsonTable1(_buf);
        }

        public int Id { private set; get; }
        public string X { private set; get; }

        private const int __ID__ = 1566207894;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(TablesComponent tables)
        {
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "x:" + X + ","
            + "}";
        }

        partial void PostInit();
    }
}
