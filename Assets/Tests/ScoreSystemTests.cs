using NUnit.Framework;

public class ScoreSystemTests
{
    private ScoreData _data;

    [SetUp]
    public void SetUp()
    {
        _data = new ScoreData();
    }

    [Test]
    public void AddCollectable_IncreasesCount()
    {
        _data.AddCollectable(1);
        Assert.AreEqual(1, _data.CollectableCount);
    }

    [Test]
    public void AddCollectable_AccumulatesMultipleCalls()
    {
        _data.AddCollectable(1);
        _data.AddCollectable(2);
        Assert.AreEqual(3, _data.CollectableCount);
    }

    [Test]
    public void TotalScore_IsCorrectSum()
    {
        _data.AddCollectable(5);
        _data.UpdateSteps(10f);
        _data.SetEnemiesDefeated(3);
        Assert.AreEqual(21, _data.TotalScore);
    }

    [Test]
    public void IsNewScoreRecord_ReturnsTrueWhenHigher()
    {
        _data.AddCollectable(10);
        Assert.IsTrue(_data.IsNewScoreRecord(5));
    }

    [Test]
    public void IsNewScoreRecord_ReturnsFalseWhenLower()
    {
        _data.AddCollectable(3);
        Assert.IsFalse(_data.IsNewScoreRecord(10));
    }
}

