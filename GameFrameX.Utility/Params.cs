namespace GameFrameX.Utility;

/// <summary>
/// 参数
/// </summary>
public abstract class Param
{
}

/// <summary>
/// 单个参数
/// </summary>
/// <typeparam name="T"></typeparam>
public class OneParam<T> : Param
{
    /// <summary>
    /// 参数
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// 单个参数
    /// </summary>
    /// <param name="t"></param>
    public OneParam(T t)
    {
        Value = t;
    }
}

/// <summary>
/// 两个参数
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class TwoParam<T1, T2> : Param
{
    /// <summary>
    /// 参数1
    /// </summary>
    public readonly T1 Value1;

    /// <summary>
    /// 参数2
    /// </summary>
    public readonly T2 Value2;

    /// <summary>
    /// 两个参数
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    public TwoParam(T1 t1, T2 t2)
    {
        Value1 = t1;
        Value2 = t2;
    }
}

/// <summary>
/// 三个参数
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public class ThreeParam<T1, T2, T3> : Param
{
    /// <summary>
    /// 参数1
    /// </summary>
    public readonly T1 Value1;

    /// <summary>
    /// 参数2
    /// </summary>
    public readonly T2 Value2;

    /// <summary>
    /// 参数3
    /// </summary>
    public readonly T3 Value3;

    /// <summary>
    /// 三个参数
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    public ThreeParam(T1 t1, T2 t2, T3 t3)
    {
        Value1 = t1;
        Value2 = t2;
        Value3 = t3;
    }
}

/// <summary>
/// 四个参数
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public class FourParam<T1, T2, T3, T4> : Param
{
    /// <summary>
    /// 参数1
    /// </summary>
    public readonly T1 Value1;

    /// <summary>
    /// 参数2
    /// </summary>
    public readonly T2 Value2;

    /// <summary>
    /// 参数3
    /// </summary>
    public readonly T3 Value3;

    /// <summary>
    /// 参数4
    /// </summary>
    public readonly T4 Value4;

    /// <summary>
    /// 四个参数
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    public FourParam(T1 t1, T2 t2, T3 t3, T4 t4)
    {
        Value1 = t1;
        Value2 = t2;
        Value3 = t3;
        Value4 = t4;
    }
}

/// <summary>
/// 五个参数
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
public class FiveParam<T1, T2, T3, T4, T5> : Param
{
    /// <summary>
    /// 参数1
    /// </summary>
    public readonly T1 Value1;

    /// <summary>
    /// 参数2
    /// </summary>
    public readonly T2 Value2;

    /// <summary>
    /// 参数3
    /// </summary>
    public readonly T3 Value3;

    /// <summary>
    /// 参数4
    /// </summary>
    public readonly T4 Value4;

    /// <summary>
    /// 参数5
    /// </summary>
    public readonly T5 Value5;


    /// <summary>
    /// 五个参数
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    /// <param name="t5"></param>
    public FiveParam(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        Value1 = t1;
        Value2 = t2;
        Value3 = t3;
        Value4 = t4;
        Value5 = t5;
    }
}