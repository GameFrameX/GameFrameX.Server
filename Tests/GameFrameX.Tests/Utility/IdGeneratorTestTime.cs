using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility;

public class IdGeneratorTestTime
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetNextUniqueIntId()
    {
        HashSet<int> set = new();
        for (int i = 0; i < 10000; i++)
        {
            Task.Run(() =>
            {
                var value = IdGenerator.GetNextUniqueIntId();
                if (!set.Add(value))
                {
                    Assert.Fail("value:" + value);
                }
            });
        }

        Assert.Pass("ok");
    }

    [Test]
    public void GetNextUniqueId()
    {
        HashSet<long> set = new();
        for (int i = 0; i < 10000; i++)
        {
            Task.Run(() =>
            {
                var value = IdGenerator.GetNextUniqueId();
                if (!set.Add(value))
                {
                    Assert.Fail("value:" + value);
                }
            });
        }

        Assert.Pass("ok");
    }
}