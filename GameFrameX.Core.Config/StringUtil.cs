using System.Text;

namespace GameFrameX.Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToStr(object o)
        {
            return ToStr(o, new StringBuilder());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static string ToStr(object o, StringBuilder sb)
        {
            foreach (var p in o.GetType().GetFields())
            {
                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }

            foreach (var p in o.GetType().GetProperties())
            {
                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ArrayToString<T>(T[] arr)
        {
            return "[" + string.Join(",", arr) + "]";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string CollectionToString<T>(IEnumerable<T> arr)
        {
            return "[" + string.Join(",", arr) + "]";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <returns></returns>
        public static string CollectionToString<TK, TV>(IDictionary<TK, TV> dic)
        {
            var sb = new StringBuilder('{');
            foreach (var e in dic)
            {
                sb.Append(e.Key).Append(':');
                sb.Append(e.Value).Append(',');
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}