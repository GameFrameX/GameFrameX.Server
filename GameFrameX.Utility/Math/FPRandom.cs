// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

namespace GameFrameX.Utility.Math;

/// <summary>
/// 生成基于确定性方法的随机数。
/// </summary>
public sealed class FPRandom
{
    // 来源：http://www.codeproject.com/Articles/164087/Random-Number-Generation
    // TSRandom 类使用梅森旋转算法生成均匀分布的随机数。
    private const int N = 624; // 状态向量的大小
    private const int M = 397; // 子向量的大小
    private const uint MATRIX_A = 0x9908b0dfU; // 变换矩阵的常量
    private const uint UPPER_MASK = 0x80000000U; // 上半部分掩码
    private const uint LOWER_MASK = 0x7fffffffU; // 下半部分掩码
    private const int MAX_RAND_INT = 0x7fffffff; // 最大随机整数值

    /// <summary>
    /// 使用种子 1 的 {@link TSRandom} 的静态实例。
    /// </summary>
    public static FPRandom instance;

    private readonly uint[] mag01 = { 0x0U, MATRIX_A, }; // 变换矩阵
    private readonly uint[] mt = new uint[N]; // 梅森旋转状态向量
    private int mti = N + 1; // 当前状态向量索引

    /// <summary>
    /// 初始化一个新的实例，使用当前时间的毫秒数作为种子。
    /// </summary>
    private FPRandom()
    {
        init_genrand((uint)DateTime.UtcNow.Millisecond);
    }

    /// <summary>
    /// 使用指定的种子初始化一个新的实例。
    /// </summary>
    /// <param name="seed">用于初始化的种子。</param>
    private FPRandom(int seed)
    {
        init_genrand((uint)seed);
    }

    /// <summary>
    /// 使用指定的初始化数组初始化一个新的实例。
    /// </summary>
    /// <param name="init">初始化数组。</param>
    private FPRandom(int[] init)
    {
        var initArray = new uint[init.Length];
        for (var i = 0; i < init.Length; ++i)
        {
            initArray[i] = (uint)init[i];
        }

        init_by_array(initArray, (uint)initArray.Length);
    }

    /// <summary>
    /// 获取最大随机整数值。
    /// </summary>
    public static int MaxRandomInt
    {
        get { return 0x7fffffff; }
    }

    /// <summary>
    /// 返回一个 {@link FP} 值，范围在 0.0 [包含] 到 1.0 [包含] 之间。
    /// </summary>
    public static FP value
    {
        get { return instance.NextFP(); }
    }

    /// <summary>
    /// 返回一个随机的 {@link TSVector}，表示半径为 1 的球体内的一个点。
    /// </summary>
    public static FPVector3 insideUnitSphere
    {
        get { return new FPVector3(value, value, value); }
    }

    internal static void Init()
    {
        instance = New(1);
    }

    /// <summary>
    /// 根据给定的种子生成一个新的实例。
    /// </summary>
    /// <param name="seed">用于生成新实例的种子。</param>
    /// <returns>新的 FPRandom 实例。</returns>
    public static FPRandom New(int seed)
    {
        var r = new FPRandom(seed);

        // StateTracker.AddTracking(r, "mt");
        // StateTracker.AddTracking(r, "mti");

        return r;
    }

    /// <summary>
    /// 返回一个随机整数。
    /// </summary>
    /// <returns>生成的随机整数。</returns>
    public int Next()
    {
        return genrand_int31();
    }

    /// <summary>
    /// 返回一个随机整数。
    /// </summary>
    /// <returns>生成的随机整数。</returns>
    public static int CallNext()
    {
        return instance.Next();
    }

    /// <summary>
    /// 返回一个介于 minValue [包含] 和 maxValue [不包含] 之间的整数。
    /// </summary>
    /// <param name="minValue">最小值。</param>
    /// <param name="maxValue">最大值。</param>
    /// <returns>生成的随机整数。</returns>
    public int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            (maxValue, minValue) = (minValue, maxValue);
        }

        var range = maxValue - minValue;

        return minValue + Next() % range;
    }

    /// <summary>
    /// 返回一个 {@link FP} 值，范围在 minValue [包含] 到 maxValue [包含] 之间。
    /// </summary>
    /// <param name="minValue">最小值。</param>
    /// <param name="maxValue">最大值。</param>
    /// <returns>生成的随机 FP 值。</returns>
    public FP Next(float minValue, float maxValue)
    {
        int minValueInt = (int)(minValue * 1000), maxValueInt = (int)(maxValue * 1000);

        if (minValueInt > maxValueInt)
        {
            (maxValueInt, minValueInt) = (minValueInt, maxValueInt);
        }

        return FP.Floor((maxValueInt - minValueInt + 1) * NextFP() +
                        minValueInt) / 1000;
    }

    /// <summary>
    /// 返回一个介于 minValue [包含] 和 maxValue [不包含] 之间的整数。
    /// </summary>
    /// <param name="minValue">最小值。</param>
    /// <param name="maxValue">最大值。</param>
    /// <returns>生成的随机整数。</returns>
    public static int Range(int minValue, int maxValue)
    {
        return instance.Next(minValue, maxValue);
    }

    /// <summary>
    /// 返回一个 {@link FP} 值，范围在 minValue [包含] 到 maxValue [包含] 之间。
    /// </summary>
    /// <param name="minValue">最小值。</param>
    /// <param name="maxValue">最大值。</param>
    /// <returns>生成的随机 FP 值。</returns>
    public static FP Range(float minValue, float maxValue)
    {
        return instance.Next(minValue, maxValue);
    }

    /// <summary>
    /// 返回一个 {@link FP} 值，范围在 0.0 [包含] 到 1.0 [包含] 之间。
    /// </summary>
    /// <returns>生成的随机 FP 值。</returns>
    public FP NextFP()
    {
        return (FP)Next() / MaxRandomInt;
    }

    private float NextFloat()
    {
        return (float)genrand_real2();
    }

    private float NextFloat(bool includeOne)
    {
        if (includeOne)
        {
            return (float)genrand_real1();
        }

        return (float)genrand_real2();
    }

    private float NextFloatPositive()
    {
        return (float)genrand_real3();
    }

    private double NextDouble()
    {
        return genrand_real2();
    }

    private double NextDouble(bool includeOne)
    {
        if (includeOne)
        {
            return genrand_real1();
        }

        return genrand_real2();
    }

    private double NextDoublePositive()
    {
        return genrand_real3();
    }

    private double Next53BitRes()
    {
        return genrand_res53();
    }

    /// <summary>
    /// 使用当前时间的毫秒数初始化随机数生成器。
    /// </summary>
    public void Initialize()
    {
        init_genrand((uint)DateTime.UtcNow.Millisecond);
    }

    /// <summary>
    /// 使用指定的种子初始化随机数生成器。
    /// </summary>
    /// <param name="seed">用于初始化的种子。</param>
    public void Initialize(int seed)
    {
        init_genrand((uint)seed);
    }

    /// <summary>
    /// 使用指定的初始化数组初始化随机数生成器。
    /// </summary>
    /// <param name="init">初始化数组。</param>
    public void Initialize(int[] init)
    {
        var initArray = new uint[init.Length];
        for (var i = 0; i < init.Length; ++i)
        {
            initArray[i] = (uint)init[i];
        }

        init_by_array(initArray, (uint)initArray.Length);
    }

    private void init_genrand(uint s)
    {
        mt[0] = s & 0xffffffffU;
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = (uint)(1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
            mt[mti] &= 0xffffffffU;
        }
    }

    private void init_by_array(uint[] init_key, uint key_length)
    {
        int i, j, k;
        init_genrand(19650218U);
        i = 1;
        j = 0;
        k = (int)(N > key_length ? N : key_length);
        for (; k > 0; k--)
        {
            mt[i] = (uint)((mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + init_key[j] + j);
            mt[i] &= 0xffffffffU;
            i++;
            j++;
            if (i >= N)
            {
                mt[0] = mt[N - 1];
                i = 1;
            }

            if (j >= key_length)
            {
                j = 0;
            }
        }

        for (k = N - 1; k > 0; k--)
        {
            mt[i] = (uint)((mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) *
                                     1566083941U)) - i);
            mt[i] &= 0xffffffffU;
            i++;
            if (i >= N)
            {
                mt[0] = mt[N - 1];
                i = 1;
            }
        }

        mt[0] = 0x80000000U;
    }

    private uint genrand_int32()
    {
        uint y;
        if (mti >= N)
        {
            int kk;
            if (mti == N + 1)
            {
                init_genrand(5489U);
            }

            for (kk = 0; kk < N - M; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
            }

            for (; kk < N - 1; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
            }

            y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];
            mti = 0;
        }

        y = mt[mti++];
        y ^= y >> 11;
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= y >> 18;
        return y;
    }

    private int genrand_int31()
    {
        return (int)(genrand_int32() >> 1);
    }

    private FP genrand_FP()
    {
        return genrand_int32() * (FP.One / 4294967295);
    }

    private double genrand_real1()
    {
        return genrand_int32() * (1.0 / 4294967295.0);
    }

    private double genrand_real2()
    {
        return genrand_int32() * (1.0 / 4294967296.0);
    }

    private double genrand_real3()
    {
        return (genrand_int32() + 0.5) * (1.0 / 4294967296.0);
    }

    private double genrand_res53()
    {
        uint a = genrand_int32() >> 5, b = genrand_int32() >> 6;
        return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
    }
}