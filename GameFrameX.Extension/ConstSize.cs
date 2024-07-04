namespace GameFrameX.Extension;

/// <summary>
/// 常量型变量的字节数
/// </summary>
public static class ConstSize
{
    /// <summary>
    /// 整型变量（32 位）的字节数。
    /// </summary>
    public const int IntSize = sizeof(int);

    /// <summary>
    /// 短整型变量（16 位）的字节数。
    /// </summary>
    public const int ShortSize = sizeof(short);

    /// <summary>
    /// 长整型变量（64 位）的字节数。
    /// </summary>
    public const int LongSize = sizeof(long);

    /// <summary>
    /// 单精度浮点型变量的字节数。
    /// </summary>
    public const int FloatSize = sizeof(float);

    /// <summary>
    /// 双精度浮点型变量的字节数。
    /// </summary>
    public const int DoubleSize = sizeof(double);

    /// <summary>
    /// 字节型变量（8 位）的字节数。
    /// </summary>
    public const int ByteSize = sizeof(byte);

    /// <summary>
    /// 有符号字节类型变量的字节数。
    /// </summary>
    public const int SbyteSize = sizeof(sbyte);

    /// <summary>
    /// 布尔型变量的字节数。
    /// </summary>
    public const int BoolSize = sizeof(bool);

    /// <summary>
    /// 无符号整型变量（32 位）的字节数。
    /// </summary>
    public const int UIntSize = sizeof(uint);

    /// <summary>
    /// 无符号短整型变量（16 位）的字节数。
    /// </summary>
    public const int UShortSize = sizeof(ushort);

    /// <summary>
    /// 无符号长整型变量（64 位）的字节数。
    /// </summary>
    public const int ULongSize = sizeof(ulong);
}