using SimpleUtils;

namespace SimpleUtilsTests;

public class Tests2
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var s = TimeFormatter.MircoSecondsToString(1000);
        Assert.That(s, Is.EqualTo("0:00:01.000"));
    }

    [Test]
    public void Test2()
    {
        var s = TimeFormatter.MircoSecondsToString(1234);
        Assert.That(s, Is.EqualTo("0:00:01.234"));
    }

    [Test]
    public void Test3()
    {
        var s = TimeFormatter.MircoSecondsToString(61000);
        Assert.That(s, Is.EqualTo("0:01:01.000"));
    }

    [Test]
    public void Test4()
    {
        var s = TimeFormatter.MircoSecondsToString(0);
        Assert.That(s, Is.EqualTo("0:00:00.000"));
    }

    [Test]
    public void Test5()
    {
        var b = TimeFormatter.TryParseTimeMsFromStr("0:00:00.001", out _);
        Assert.That(b, Is.EqualTo(true));
    }
    
    [Test]
    public void Test6()
    {
        var b = TimeFormatter.TryParseTimeMsFromStr("0:0.1", out _);
        Assert.That(b, Is.EqualTo(false));
    }
    
    [Test]
    public void Test7()
    {
        var b = TimeFormatter.TryParseTimeMsFromStr("0:00:10", out _);
        Assert.That(b, Is.EqualTo(false));
    }
    
    [Test]
    public void Test8()
    {
        var b = TimeFormatter.TryParseTimeMsFromStr("0:00:a.000", out _);
        Assert.That(b, Is.EqualTo(false));
    }
}