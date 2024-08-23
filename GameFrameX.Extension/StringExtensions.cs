using System.Text;
using System.Text.RegularExpressions;

namespace GameFrameX.Extension
{
    /// <summary>
    /// 提供对 <see cref="string"/> 类型的扩展方法。
    /// </summary>
    public static class StringExtension
    {
        private static readonly StringBuilder NewSentence = new StringBuilder();

        /// <summary>
        /// 替换重复字符
        /// </summary>
        /// <param name="c"></param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static string RepeatChar(this char c, int count)
        {
            NewSentence.Clear();
            for (int i = 0; i < count; i++)
            {
                NewSentence.Append(c);
            }

            return NewSentence.ToString();
        }

        /// <summary>
        /// 获取居中的字符串的固定长度打印
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string CenterAlignedText(this string text, int width)
        {
            if (width < text.Length)
            {
                throw new IndexOutOfRangeException(nameof(width));
            }

            int spaces = (width - text.Length) / 2;
            string paddedText = new string(' ', spaces) + text + new string(' ', spaces);
            return paddedText;
        }

        /// <summary>
        /// 将字符串按指定宽度进行换行
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string WordWrap(this string text, int width)
        {
            var words = text.Split(' ');
            NewSentence.Clear();
            string line = "";
            foreach (var word in words)
            {
                if ((line + word).Length > width)
                {
                    NewSentence.AppendLine(line);
                    line = "";
                }

                line += $"{word} ";
            }

            if (line.Length > 0)
            {
                NewSentence.AppendLine(line);
            }

            return NewSentence.ToString();
        }

        /// <summary>
        /// 从当前字符串中移除指定字符结尾的后缀
        /// </summary>
        /// <param name="self">当前字符串</param>
        /// <param name="toRemove">要移除的字符</param>
        /// <returns>移除后的字符串</returns>
        public static string RemoveSuffix(this string self, char toRemove)
        {
            return self.IsNullOrEmpty() ? self : (self.EndsWith(toRemove) ? self.Substring(0, self.Length - 1) : self);
        }

        /// <summary>
        /// 从当前字符串中移除指定子字符串结尾的后缀
        /// </summary>
        /// <param name="self">当前字符串</param>
        /// <param name="toRemove">要移除的子字符串</param>
        /// <returns>移除后的字符串</returns>
        public static string RemoveSuffix(this string self, string toRemove)
        {
            return self.IsNullOrEmpty() ? self : (self.EndsWith(toRemove) ? self.Substring(0, self.Length - toRemove.Length) : self);
        }

        /// <summary>
        /// 移除当前字符串中的所有空白字符
        /// </summary>
        /// <param name="self">当前字符串</param>
        /// <returns>移除空白字符后的字符串</returns>
        public static string RemoveWhiteSpace(this string self)
        {
            return self.IsNullOrEmpty() ? self : new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        /// 检查字符串是否为 null 或空。
        /// </summary>
        /// <param name="str">要检查的字符串。</param>
        /// <returns>如果字符串为 null 或空，则为 true；否则为 false。</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 检查字符串是否为 null 或 空 或 空白字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string str)
        {
            return str.IsNullOrEmpty() || str.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// 检查字符串是否 [不] 为 null 或 空 或 空白字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmptyOrWhiteSpace(this string str)
        {
            return !str.IsNullOrEmptyOrWhiteSpace();
        }

        /// <summary>
        /// 检查字符串是否不为 null 或空。
        /// </summary>
        /// <param name="str">要检查的字符串。</param>
        /// <returns>如果字符串为 null 或空，则为 true；否则为 false。</returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        /// <summary>
        /// 检查字符串是否为 null 或空白字符串。
        /// </summary>
        /// <param name="str">要检查的字符串。</param>
        /// <returns>如果字符串为 null 或空，则为 true；否则为 false。</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 检查字符串是否不为 null 或空白字符串。
        /// </summary>
        /// <param name="str">要检查的字符串。</param>
        /// <returns>如果字符串为 null 或空，则为 true；否则为 false。</returns>
        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !str.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// 确保指定的值不为null或空白字符串。
        /// </summary>
        /// <param name="value">要检查的值。</param>
        /// <param name="name">值的名称。</param>
        /// <exception cref="ArgumentNullException">当值为null或空白字符串时引发。</exception>
        public static void CheckNotNullOrEmpty(this string value, string name)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentNullException(name, " can not be null.");
            }
        }

        /// <summary>
        /// 将字符串按指定的分隔符拆分为整数数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <param name="sep">用于分隔字符串的字符。</param>
        /// <returns>拆分得到的整数数组。</returns>
        public static int[] SplitToIntArray(this string str, char sep = '+')
        {
            if (string.IsNullOrEmpty(str))
                return Array.Empty<int>();

            var arr = str.Split(sep);
            int[] ret = new int[arr.Length];
            for (int i = 0; i < arr.Length; ++i)
            {
                if (int.TryParse(arr[i], out var t))
                    ret[i] = t;
            }

            return ret;
        }

        /// <summary>
        /// 将字符串按指定的分隔符拆分为二维整数数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <param name="sep1">用于第一层分隔的字符。</param>
        /// <param name="sep2">用于第二层分隔的字符。</param>
        /// <returns>拆分得到的二维整数数组。</returns>
        public static int[][] SplitTo2IntArray(this string str, char sep1 = ';', char sep2 = '+')
        {
            if (string.IsNullOrEmpty(str))
                return Array.Empty<int[]>();

            var arr = str.Split(sep1);
            if (arr.Length <= 0)
                return Array.Empty<int[]>();

            int[][] ret = new int[arr.Length][];

            for (int i = 0; i < arr.Length; ++i)
                ret[i] = arr[i].SplitToIntArray(sep2);
            return ret;
        }

        /// <summary>
        /// 根据字符串创建目录，递归创建。
        /// </summary>
        /// <param name="path">要创建的目录路径。</param>
        /// <param name="isFile">指示是否为文件路径。</param>
        public static void CreateAsDirectory(this string path, bool isFile = false)
        {
            if (isFile)
            {
                path = Path.GetDirectoryName(path);
            }

            if (!Directory.Exists(path))
            {
                CreateAsDirectory(path, true);
                Directory.CreateDirectory(path);
            }
        }


        /// <summary>
        /// 转换字符串为下划线分割的小写形式，用于命名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}