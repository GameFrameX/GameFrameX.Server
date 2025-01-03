
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config
{
    /// <summary>
    /// 取两个值的范围:<br/>1,3 就相当于[1,3)
    /// </summary>
    public sealed partial class RangeTowNumber : BeanBase
    {
        /*
        public RangeTowNumber(int Range1, int Range2) 
        {
            this.Range1 = Range1;
            this.Range2 = Range2;
            PostInit();
        }        
        */

        public RangeTowNumber(JsonElement _buf) 
        {
            Range1 = _buf.GetProperty("range_1").GetInt32();
            Range2 = _buf.GetProperty("range_2").GetInt32();
        }
    
        public static RangeTowNumber DeserializeRangeTowNumber(JsonElement _buf)
        {
            return new RangeTowNumber(_buf);
        }

        /// <summary>
        /// 范围1
        /// </summary>
        public int Range1 { private set; get; }
        /// <summary>
        /// 范围2
        /// </summary>
        public int Range2 { private set; get; }

        private const int __ID__ = 601676584;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(TablesComponent tables)
        {
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "range1:" + Range1 + ","
            + "range2:" + Range2 + ","
            + "}";
        }

        partial void PostInit();
    }
}
