using SimpleUtils;

namespace SimpleUtilsTests;

public class Tests
{
    private ObjectPool<int> _pool;

    [SetUp]
    public void Setup()
    {
        var random = new Random();
        _pool = new ObjectPool<int>(() => random.Next(0, 100), 3);
    }

    [Test]
    public void Test1()
    {
        var poolSize = _pool.Size;
        Assert.That(poolSize, Is.EqualTo(3));
    }

    [Test]
    public void Test2()
    {
        var i = _pool.Get();

        Assert.That(_pool.IsAvailable(i), Is.EqualTo(false));
    }

    [Test]
    public void Test3()
    {
        var i = _pool.Get();
        _pool.Release(i);

        Assert.That(_pool.IsAvailable(i), Is.EqualTo(true));
    }
    

    [Test]
    public void Test4()
    {
        var i = _pool.Get();
        i = _pool.Get();
        i = _pool.Get();
        i = _pool.Get();

        Assert.That(_pool.Size, Is.EqualTo(4));
    }
}