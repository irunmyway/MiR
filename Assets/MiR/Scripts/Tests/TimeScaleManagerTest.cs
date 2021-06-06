using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using Foundation;

[TestFixture]
public sealed class TimeScaleManagerTest : ZenjectUnitTestFixture
{
    const float FloatEpsilon = 0.0001f;

    [SetUp]
    public void Install()
    {
        Container.Bind<ITimeScaleManager>().To<TimeScaleManager>().FromNewComponentOnNewGameObject().AsSingle();
    }

    [Test]
    public void TestOneHandle()
    {
        var manager = Container.Resolve<ITimeScaleManager>();
        var handle = new TimeScaleHandle();

        Assert.AreEqual(1.0f, Time.timeScale, FloatEpsilon);

        //////////////////////////////////////////////////////////////
        // Test with 0.5

        // add 0.5
        manager.BeginTimeScale(handle, 0.5f);
        Assert.AreEqual(0.5f, Time.timeScale, FloatEpsilon);

        // remove 0.5
        manager.EndTimeScale(handle);
        Assert.AreEqual(1.0f, Time.timeScale, FloatEpsilon);

        //////////////////////////////////////////////////////////////
        // Test with 0.0

        // add 0.0
        manager.BeginTimeScale(handle, 0.0f);
        Assert.AreEqual(0.0f, Time.timeScale, FloatEpsilon);

        // remove 0.0
        manager.EndTimeScale(handle);
        Assert.AreEqual(1.0f, Time.timeScale, FloatEpsilon);
    }

    [Test]
    public void TestTwoHandles()
    {
        var manager = Container.Resolve<ITimeScaleManager>();
        var handle1 = new TimeScaleHandle();
        var handle2 = new TimeScaleHandle();

        Assert.AreEqual(1.0f, Time.timeScale);

        //////////////////////////////////////////////////////////////
        // Test removal in LIFO order

        // add 0.5
        manager.BeginTimeScale(handle1, 0.5f);
        Assert.AreEqual(0.5f, Time.timeScale);

        // add 0.25
        manager.BeginTimeScale(handle2, 0.25f);
        Assert.AreEqual(0.125f, Time.timeScale);

        // remove 0.25
        manager.EndTimeScale(handle2);
        Assert.AreEqual(0.5f, Time.timeScale);

        // remove 0.5
        manager.EndTimeScale(handle1);
        Assert.AreEqual(1.0f, Time.timeScale);

        //////////////////////////////////////////////////////////////
        // Test removal in FIFO order

        // add 0.5
        manager.BeginTimeScale(handle1, 0.5f);
        Assert.AreEqual(0.5f, Time.timeScale);

        // add 0.25
        manager.BeginTimeScale(handle2, 0.25f);
        Assert.AreEqual(0.125f, Time.timeScale);

        // remove 0.5
        manager.EndTimeScale(handle1);
        Assert.AreEqual(0.25f, Time.timeScale);

        // remove 0.25
        manager.EndTimeScale(handle2);
        Assert.AreEqual(1.0f, Time.timeScale);
    }

    [Test]
    public void TestThreeHandles()
    {
        var manager = Container.Resolve<ITimeScaleManager>();
        var handle1 = new TimeScaleHandle();
        var handle2 = new TimeScaleHandle();
        var handle3 = new TimeScaleHandle();

        Assert.AreEqual(1.0f, Time.timeScale);

        // add 0.5
        manager.BeginTimeScale(handle1, 0.5f);
        Assert.AreEqual(0.5f, Time.timeScale);

        // add 0.0
        manager.BeginTimeScale(handle2, 0.0f);
        Assert.AreEqual(0.0f, Time.timeScale);

        // add 0.25
        manager.BeginTimeScale(handle3, 0.25f);
        Assert.AreEqual(0.0f, Time.timeScale);

        // remove 0.0
        manager.EndTimeScale(handle2);
        Assert.AreEqual(0.125f, Time.timeScale);

        // remove 0.25
        manager.EndTimeScale(handle3);
        Assert.AreEqual(0.5f, Time.timeScale);

        // remove 0.5
        manager.EndTimeScale(handle1);
        Assert.AreEqual(1.0f, Time.timeScale);
    }
}
