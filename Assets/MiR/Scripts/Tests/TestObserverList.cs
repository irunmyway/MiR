using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using Foundation;

[TestFixture]
public sealed class TestObserverList : ZenjectUnitTestFixture
{
    interface ITestObserver
    {
    }

    class TestObserver : ITestObserver
    {
    }

    [Test]
    public void TestEmptyList()
    {
        var list = new ObserverList<ITestObserver>();

        Assert.AreEqual(0, list.ObserverCount);

        int n = 0;
        foreach (var item in list.Enumerate())
            n++;

        Assert.AreEqual(0, n);
    }

    [Test]
    public void TestOneObserver()
    {
        int n;

        var list = new ObserverList<ITestObserver>();
        TestObserver observer = new TestObserver();

        //////////////////////////////////////////////////////////////
        // Add and test

        ObserverHandle handle = null;
        list.Add(ref handle, observer);

        Assert.NotNull(handle);
        Assert.AreEqual(1, list.ObserverCount);

        n = 0;
        foreach (var item in list.Enumerate()) {
            n++;
            Assert.AreEqual(item, observer);
        }

        Assert.AreEqual(1, n);

        //////////////////////////////////////////////////////////////
        // Remove and test

        list.Remove(handle);

        Assert.AreEqual(0, list.ObserverCount);

        n = 0;
        foreach (var item in list.Enumerate())
            n++;

        Assert.AreEqual(0, n);

        //////////////////////////////////////////////////////////////
        // Add again and test

        list.Add(ref handle, observer);

        Assert.NotNull(handle);
        Assert.AreEqual(1, list.ObserverCount);

        n = 0;
        foreach (var item in list.Enumerate()) {
            n++;
            Assert.AreEqual(item, observer);
        }

        Assert.AreEqual(1, n);
    }

    [Test]
    public void TestTwoObservers()
    {
        Dictionary<ITestObserver, int> dict;
        int n1, n2, nTotal;

        var list = new ObserverList<ITestObserver>();
        TestObserver observer1 = new TestObserver();
        TestObserver observer2 = new TestObserver();

        //////////////////////////////////////////////////////////////
        // Add and test

        ObserverHandle handle1 = null;
        ObserverHandle handle2 = null;
        list.Add(ref handle1, observer1);
        list.Add(ref handle2, observer2);

        Assert.NotNull(handle1);
        Assert.NotNull(handle2);
        Assert.AreEqual(2, list.ObserverCount);

        dict = new Dictionary<ITestObserver, int>();
        nTotal = 0;
        foreach (var item in list.Enumerate()) {
            ++nTotal;
            dict.TryGetValue(item, out int n);
            dict[item] = n + 1;
        }

        dict.TryGetValue(observer1, out n1);
        dict.TryGetValue(observer2, out n2);
        Assert.AreEqual(1, n1);
        Assert.AreEqual(1, n2);
        Assert.AreEqual(2, nTotal);

        //////////////////////////////////////////////////////////////
        // Remove one and test

        list.Remove(handle1);

        Assert.AreEqual(1, list.ObserverCount);

        dict = new Dictionary<ITestObserver, int>();
        nTotal = 0;
        foreach (var item in list.Enumerate()) {
            ++nTotal;
            dict.TryGetValue(item, out int n);
            dict[item] = n + 1;
        }

        dict.TryGetValue(observer1, out n1);
        dict.TryGetValue(observer2, out n2);
        Assert.AreEqual(0, n1);
        Assert.AreEqual(1, n2);
        Assert.AreEqual(1, nTotal);

        //////////////////////////////////////////////////////////////
        // Remove remaining one and test

        list.Remove(handle2);

        Assert.AreEqual(0, list.ObserverCount);

        nTotal = 0;
        foreach (var item in list.Enumerate())
            ++nTotal;

        Assert.AreEqual(0, nTotal);
    }

    [Test]
    public void TestModifyDuringIteration()
    {
        Dictionary<ITestObserver, int> dict;
        int n1, n2, n3, nTotal;

        var list = new ObserverList<ITestObserver>();
        TestObserver observer1 = new TestObserver();
        TestObserver observer2 = new TestObserver();
        TestObserver observer3 = new TestObserver();

        //////////////////////////////////////////////////////////////
        // Add during iteration

        ObserverHandle handle1 = null;
        ObserverHandle handle2 = null;
        ObserverHandle handle3 = null;
        list.Add(ref handle1, observer1);
        list.Add(ref handle2, observer2);

        Assert.NotNull(handle1);
        Assert.NotNull(handle2);
        Assert.AreEqual(2, list.ObserverCount);

        dict = new Dictionary<ITestObserver, int>();
        nTotal = 0;
        foreach (var item in list.Enumerate()) {
            ++nTotal;
            dict.TryGetValue(item, out int n);
            dict[item] = n + 1;

            if (nTotal == 1)
                list.Add(ref handle3, observer3);
        }

        Assert.NotNull(handle3);
        Assert.AreEqual(3, list.ObserverCount);

        dict.TryGetValue(observer1, out n1);
        dict.TryGetValue(observer2, out n2);
        dict.TryGetValue(observer3, out n3);
        Assert.AreEqual(1, n1);
        Assert.AreEqual(1, n2);
        Assert.AreEqual(0, n3);
        Assert.AreEqual(2, nTotal);

        //////////////////////////////////////////////////////////////
        // Remove during iteration

        dict = new Dictionary<ITestObserver, int>();
        nTotal = 0;
        foreach (var item in list.Enumerate()) {
            ++nTotal;
            dict.TryGetValue(item, out int n);
            dict[item] = n + 1;

            if (nTotal == 1)
                list.Remove(handle3);
        }

        Assert.AreEqual(2, list.ObserverCount);

        dict.TryGetValue(observer1, out n1);
        dict.TryGetValue(observer2, out n2);
        dict.TryGetValue(observer3, out n3);
        Assert.AreEqual(1, n1);
        Assert.AreEqual(1, n2);
        Assert.AreEqual(1, n3);
        Assert.AreEqual(3, nTotal);

        //////////////////////////////////////////////////////////////
        // Finally iterate and make sure it was removed

        dict = new Dictionary<ITestObserver, int>();
        nTotal = 0;
        foreach (var item in list.Enumerate()) {
            ++nTotal;
            dict.TryGetValue(item, out int n);
            dict[item] = n + 1;
        }

        dict.TryGetValue(observer1, out n1);
        dict.TryGetValue(observer2, out n2);
        dict.TryGetValue(observer3, out n3);
        Assert.AreEqual(1, n1);
        Assert.AreEqual(1, n2);
        Assert.AreEqual(0, n3);
        Assert.AreEqual(2, nTotal);
    }
}
