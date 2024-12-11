using System.Text;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Utility;

namespace GameFrameX.Tests;

public class ActorIdGeneratorTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void compress()
    {
        var actorId = ActorIdGenerator.GetActorId((ushort)ActorType.World, 1000);

        var type = ActorIdGenerator.GetActorType(actorId);
        var serverId = ActorIdGenerator.GetServerId(actorId);

        Assert.That(type == (ushort)ActorType.World, Is.EqualTo(true));
        Assert.That(serverId == 1000, Is.EqualTo(true));
    }
}