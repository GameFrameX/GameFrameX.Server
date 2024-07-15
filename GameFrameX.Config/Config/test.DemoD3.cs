
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
    public abstract partial class DemoD3 : test.DemoDynamic
    {
        /*
        public DemoD3(int X1, int X3)  : base(X1) 
        {
            this.X3 = X3;
            PostInit();
        }        
        */

        public DemoD3(JsonElement _buf)  : base(_buf) 
        {
            X3 = _buf.GetProperty("x3").GetInt32();
        }
    
        public static DemoD3 DeserializeDemoD3(JsonElement _buf)
        {
            switch (_buf.GetProperty("$type").GetString())
            {
                case "DemoE1": return new test.DemoE1(_buf);
                case "test.login.RoleInfo": return new test.login.RoleInfo(_buf);
                default: throw new SerializationException();
            }
        }

        public int X3 { private set; get; }


        public override void ResolveRef(TablesComponent tables)
        {
            base.ResolveRef(tables);
            
        }

        public override string ToString()
        {
            return "{ "
            + "x1:" + X1 + ","
            + "x3:" + X3 + ","
            + "}";
        }

        partial void PostInit();
    }
}
