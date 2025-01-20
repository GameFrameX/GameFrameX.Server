using System.Text;
using System.Text.RegularExpressions;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 提供对 <see cref="string" /> 类型的扩展方法。
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 用于构建新句子的 StringBuilder 实例。
    /// </summary>
    private static readonly StringBuilder NewSentence = new();

    /// <summary>
    /// 匹配中文字符的正则表达式。
    /// </summary>
    private static readonly Regex CnReg = new(@"[\u4e00-\u9fa5]");

    /// <summary>
    /// 重复指定字符指定次数。
    /// </summary>
    /// <param name="c">要重复的字符。</param>
    /// <param name="count">重复次数。</param>
    /// <returns>由指定字符重复指定次数组成的新字符串。</returns>
    public static string RepeatChar(this char c, int count)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Clear();
        for (var i = 0; i < count; i++)
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获取在指定宽度内居中对齐的文本。
    /// </summary>
    /// <param name="text">要居中对齐的文本。</param>
    /// <param name="width">总宽度。</param>
    /// <returns>居中对齐后的文本，两侧填充空格。</returns>
    /// <remarks>如果指定宽度小于文本长度，将使用文本长度作为宽度。</remarks>
    public static string CenterAlignedText(this string text, int width)
    {
        if (width < text.Length)
        {
            width = text.Length;
            // throw new IndexOutOfRangeException(nameof(width));
        }

        var spaces = (width - text.Length) / 2;
        var paddedText = new string(' ', spaces) + text + new string(' ', spaces);
        return paddedText;
    }

    /// <summary>
    /// 将文本按指定宽度自动换行。
    /// </summary>
    /// <param name="text">要换行的文本。</param>
    /// <param name="width">每行的最大宽度。</param>
    /// <returns>按指定宽度换行后的文本。</returns>
    public static string WordWrap(this string text, int width)
    {
        var words = text.Split(' ');
        NewSentence.Clear();
        var line = "";
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
    /// 从字符串末尾移除指定字符（如果存在）。
    /// </summary>
    /// <param name="self">要处理的字符串。</param>
    /// <param name="toRemove">要移除的字符。</param>
    /// <returns>移除指定字符后的字符串。如果字符串为null或空，或不以指定字符结尾，则返回原字符串。</returns>
    public static string RemoveSuffix(this string self, char toRemove)
    {
        return self.IsNullOrEmpty() ? self : self.EndsWith(toRemove) ? self.Substring(0, self.Length - 1) : self;
    }

    /// <summary>
    /// 从字符串末尾移除指定的子字符串（如果存在）。
    /// </summary>
    /// <param name="self">要处理的字符串。</param>
    /// <param name="toRemove">要移除的子字符串。</param>
    /// <returns>移除指定子字符串后的字符串。如果字符串为null或空，或不以指定子字符串结尾，则返回原字符串。</returns>
    public static string RemoveSuffix(this string self, string toRemove)
    {
        return self.IsNullOrEmpty() ? self : self.EndsWith(toRemove) ? self.Substring(0, self.Length - toRemove.Length) : self;
    }

    /// <summary>
    /// 移除字符串中的所有空白字符。
    /// </summary>
    /// <param name="self">要处理的字符串。</param>
    /// <returns>移除所有空白字符后的字符串。如果输入为null或空，则返回原字符串。</returns>
    public static string RemoveWhiteSpace(this string self)
    {
        return self.IsNullOrEmpty() ? self : new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    /// <summary>
    /// 检查字符串是否为 null 或空字符串。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串为 null 或空字符串，则返回 true；否则返回 false。</returns>
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// 检查字符串是否为 null、空字符串或仅包含空白字符。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串为 null、空字符串或仅包含空白字符，则返回 true；否则返回 false。</returns>
    public static bool IsNullOrEmptyOrWhiteSpace(this string str)
    {
        return str.IsNullOrEmpty() || str.IsNullOrWhiteSpace();
    }

    /// <summary>
    /// 检查字符串是否不为 null、空字符串且不仅包含空白字符。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串不为 null、不为空字符串且不仅包含空白字符，则返回 true；否则返回 false。</returns>
    public static bool IsNotNullOrEmptyOrWhiteSpace(this string str)
    {
        return !str.IsNullOrEmptyOrWhiteSpace();
    }

    /// <summary>
    /// 检查字符串是否不为 null 且不为空字符串。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串不为 null 且不为空字符串，则返回 true；否则返回 false。</returns>
    public static bool IsNotNullOrEmpty(this string str)
    {
        return !str.IsNullOrEmpty();
    }

    /// <summary>
    /// 检查字符串是否为 null 或仅包含空白字符。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串为 null 或仅包含空白字符，则返回 true；否则返回 false。</returns>
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 检查字符串是否不为 null 且不仅包含空白字符。
    /// </summary>
    /// <param name="str">要检查的字符串。</param>
    /// <returns>如果字符串不为 null 且不仅包含空白字符，则返回 true；否则返回 false。</returns>
    public static bool IsNotNullOrWhiteSpace(this string str)
    {
        return !str.IsNullOrWhiteSpace();
    }

    /// <summary>
    /// 验证字符串不为 null 或空字符串，否则抛出异常。
    /// </summary>
    /// <param name="value">要验证的字符串。</param>
    /// <param name="name">参数名称，用于异常消息。</param>
    /// <exception cref="ArgumentNullException">当字符串为 null 或空字符串时抛出。</exception>
    public static void CheckNotNullOrEmpty(this string value, string name)
    {
        if (value.IsNullOrEmpty())
        {
            throw new ArgumentNullException(name, "不能为 null。");
        }
    }

    /// <summary>
    /// 验证字符串不为 null、空字符串或仅包含空白字符，否则抛出异常。
    /// </summary>
    /// <param name="value">要验证的字符串。</param>
    /// <param name="name">参数名称，用于异常消息。</param>
    /// <exception cref="ArgumentNullException">当字符串为 null、空字符串或仅包含空白字符时抛出。</exception>
    public static void CheckNotNullOrEmptyOrWhiteSpace(this string value, string name)
    {
        if (value.IsNullOrEmptyOrWhiteSpace())
        {
            throw new ArgumentNullException(name, "不能为 null 或空白字符串。");
        }
    }

    /// <summary>
    /// 将字符串按指定分隔符拆分为整数数组。
    /// </summary>
    /// <param name="str">要拆分的字符串。</param>
    /// <param name="sep">分隔符，默认为 '+'。</param>
    /// <returns>拆分并转换后的整数数组。如果字符串为null或空，则返回空数组。</returns>
    public static int[] SplitToIntArray(this string str, char sep = '+')
    {
        if (string.IsNullOrEmpty(str))
        {
            return Array.Empty<int>();
        }

        var arr = str.Split(sep);
        var ret = new int[arr.Length];
        for (var i = 0; i < arr.Length; ++i)
        {
            if (int.TryParse(arr[i], out var t))
            {
                ret[i] = t;
            }
        }

        return ret;
    }

    /// <summary>
    /// 将字符串按两级分隔符拆分为二维整数数组。
    /// </summary>
    /// <param name="str">要拆分的字符串。</param>
    /// <param name="sep1">第一级分隔符，默认为 ';'。</param>
    /// <param name="sep2">第二级分隔符，默认为 '+'。</param>
    /// <returns>拆分并转换后的二维整数数组。如果字符串为null或空，则返回空数组。</returns>
    public static int[][] SplitTo2IntArray(this string str, char sep1 = ';', char sep2 = '+')
    {
        if (string.IsNullOrEmpty(str))
        {
            return Array.Empty<int[]>();
        }

        var arr = str.Split(sep1);
        if (arr.Length <= 0)
        {
            return Array.Empty<int[]>();
        }

        var ret = new int[arr.Length][];

        for (var i = 0; i < arr.Length; ++i)
        {
            ret[i] = arr[i].SplitToIntArray(sep2);
        }

        return ret;
    }

    /// <summary>
    /// 根据路径创建目录，支持递归创建。
    /// </summary>
    /// <param name="path">目录路径。</param>
    /// <param name="isFile">是否为文件路径，如果为true，则创建文件所在的目录。默认为false。</param>
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
    /// 将驼峰命名的字符串转换为下划线分隔的小写形式（蛇形命名）。
    /// </summary>
    /// <param name="input">要转换的字符串。</param>
    /// <returns>转换后的蛇形命名字符串。如果输入为null或空，则返回原字符串。</returns>
    public static string ConvertToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    /// <summary>
    /// 移除字符串中的所有中文字符。
    /// </summary>
    /// <param name="self">要处理的字符串。</param>
    /// <returns>移除所有中文字符后的字符串。</returns>
    public static string TrimZhCn(this string self)
    {
        self = CnReg.Replace(self, string.Empty);
        return self;
    }

    /// <summary>
    /// 快速比较两个字符串是否相等，从末尾开始比较。
    /// </summary>
    /// <param name="self">当前字符串。</param>
    /// <param name="target">要比较的目标字符串。</param>
    /// <returns>如果两个字符串相等则返回true，否则返回false。</returns>
    /// <exception cref="ArgumentNullException">当前字符串为null时抛出。</exception>
    public static bool EqualsFast(this string self, string target)
    {
        if (self == null)
        {
            return target == null;
        }

        if (target == null)
        {
            return false;
        }

        if (self.Length != target.Length)
        {
            return false;
        }

        int ap = self.Length - 1;
        int bp = target.Length - 1;

        while (ap >= 0 && bp >= 0 && self[ap] == target[bp])
        {
            ap--;
            bp--;
        }

        return (bp < 0);
    }

    /// <summary>
    /// 快速检查字符串是否以指定字符串结尾，从末尾开始比较。
    /// </summary>
    /// <param name="self">当前字符串。</param>
    /// <param name="target">要检查的结尾字符串。</param>
    /// <returns>如果字符串以指定字符串结尾则返回true，否则返回false。</returns>
    public static bool EndsWithFast(this string self, string target)
    {
        int ap = self.Length - 1;
        int bp = target.Length - 1;

        while (ap >= 0 && bp >= 0 && self[ap] == target[bp])
        {
            ap--;
            bp--;
        }

        return (bp < 0);
    }

    /// <summary>
    /// 快速检查字符串是否以指定字符串开头，从开头开始比较。
    /// </summary>
    /// <param name="self">当前字符串。</param>
    /// <param name="target">要检查的开头字符串。</param>
    /// <returns>如果字符串以指定字符串开头则返回true，否则返回false。</returns>
    public static bool StartsWithFast(this string self, string target)
    {
        int aLen = self.Length;
        int bLen = target.Length;

        int ap = 0;
        int bp = 0;

        while (ap < aLen && bp < bLen && self[ap] == target[bp])
        {
            ap++;
            bp++;
        }

        return (bp == bLen);
    }
}