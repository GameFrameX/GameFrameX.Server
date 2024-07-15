
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.ai
{
    public sealed partial class IntKeyData : ai.KeyData
    {
        /*
        public IntKeyData(int Value)  : base() 
        {
            this.Value = Value;
            PostInit();
        }        
        */

        public IntKeyData(JsonElement _buf)  : base(_buf) 
        {
            Value = _buf.GetProperty("value").GetInt32();
        }
    
        public static IntKeyData DeserializeIntKeyData(JsonElement _buf)
        {
            return new ai.IntKeyData(_buf);
        }

        public int Value { private set; get; }

        private const int __ID__ = -342751904;
        public override int GetTypeId() => __ID__;

        public override void ResolveRef(TablesComponent tables)
        {
            base.ResolveRef(tables);
            
        }

        public override string ToString()
        {
            return "{ "
            + "value:" + Value + ","
            + "}";
        }

        partial void PostInit();
    }
}
