
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
    public sealed partial class MoveToRandomLocation : ai.Task
    {
        /*
        public MoveToRandomLocation(int Id, string NodeName, System.Collections.Generic.List<ai.Decorator> Decorators, System.Collections.Generic.List<ai.Service> Services, bool IgnoreRestartSelf, string OriginPositionKey, float Radius)  : base(Id, NodeName, Decorators, Services, IgnoreRestartSelf) 
        {
            this.OriginPositionKey = OriginPositionKey;
            this.Radius = Radius;
            PostInit();
        }        
        */

        public MoveToRandomLocation(JsonElement _buf)  : base(_buf) 
        {
            OriginPositionKey = _buf.GetProperty("origin_position_key").GetString();
            Radius = _buf.GetProperty("radius").GetSingle();
        }
    
        public static MoveToRandomLocation DeserializeMoveToRandomLocation(JsonElement _buf)
        {
            return new ai.MoveToRandomLocation(_buf);
        }

        public string OriginPositionKey { private set; get; }
        public float Radius { private set; get; }

        private const int __ID__ = -2140042998;
        public override int GetTypeId() => __ID__;

        public override void ResolveRef(TablesComponent tables)
        {
            base.ResolveRef(tables);
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "nodeName:" + NodeName + ","
            + "decorators:" + StringUtil.CollectionToString(Decorators) + ","
            + "services:" + StringUtil.CollectionToString(Services) + ","
            + "ignoreRestartSelf:" + IgnoreRestartSelf + ","
            + "originPositionKey:" + OriginPositionKey + ","
            + "radius:" + Radius + ","
            + "}";
        }

        partial void PostInit();
    }
}
