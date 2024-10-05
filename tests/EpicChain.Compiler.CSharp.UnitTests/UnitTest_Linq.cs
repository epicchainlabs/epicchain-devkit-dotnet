using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.VM.Types;
using System.Collections.Generic;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Linq : DebugAndTestBase<Contract_Linq>
    {
        [TestMethod]
        public void Test_AggregateSum()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(new BigInteger(-101), Contract.AggregateSum(array));
            AssertEpicPulseConsumed(1226460);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(new BigInteger(5), Contract.AggregateSum(array));
            AssertEpicPulseConsumed(1288650);
        }

        [TestMethod]
        public void Test_AllGreaterThanZero()
        {
            var array = new List<object>
            {
                1,
                100
            };
            Assert.IsTrue(Contract.AllGreaterThanZero(array));
            AssertEpicPulseConsumed(1205130);
            array.Add(0);
            Assert.IsFalse(Contract.AllGreaterThanZero(array));
            AssertEpicPulseConsumed(1225170);
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            var array = new List<object>();

            Assert.IsTrue(Contract.IsEmpty(array));
            AssertEpicPulseConsumed(1084530);

            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);

            Assert.IsFalse(Contract.IsEmpty(array));
            AssertEpicPulseConsumed(1147740);
        }

        [TestMethod]
        public void Test_AnyGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.AnyGreaterThanZero(array));
            AssertEpicPulseConsumed(1225230);
            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
            AssertEpicPulseConsumed(1245150);
        }

        [TestMethod]
        public void Test_AnyGreaterThan()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.AnyGreaterThan(array, 0));
            AssertEpicPulseConsumed(1225470);

            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThan(array, 0));
            AssertEpicPulseConsumed(1245420);
            Assert.IsFalse(Contract.AnyGreaterThan(array, 100));
            AssertEpicPulseConsumed(1245840);
        }

        [TestMethod]
        public void Test_Average()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.Average(array));
            AssertEpicPulseConsumed(1101270);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.Average(array));
            AssertEpicPulseConsumed(1158810);
            array.Add(3);
            Assert.AreEqual(1, Contract.Average(array));
            AssertEpicPulseConsumed(1162740);
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.AverageTwice(array));
            AssertEpicPulseConsumed(1120080);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(2, Contract.AverageTwice(array));
            AssertEpicPulseConsumed(1231530);
            array.Add(3);
            Assert.AreEqual(3, Contract.AverageTwice(array));
            AssertEpicPulseConsumed(1253430);
        }

        [TestMethod]
        public void Test_Count()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(3, Contract.Count(array));
            AssertEpicPulseConsumed(1155150);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(7, Contract.Count(array));
            AssertEpicPulseConsumed(1167990);
        }

        [TestMethod]
        public void Test_CountGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(0, Contract.CountGreaterThanZero(array));
            AssertEpicPulseConsumed(1225350);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(3, Contract.CountGreaterThanZero(array));
            AssertEpicPulseConsumed(1308690);
        }

        [TestMethod]
        public void Test_Contains()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsTrue(Contract.Contains(array, 0));
            AssertEpicPulseConsumed(1202490);
            array.Add(1);
            Assert.IsFalse(Contract.Contains(array, 9));
            AssertEpicPulseConsumed(1266120);
            Assert.IsTrue(Contract.Contains(array, 1));
            AssertEpicPulseConsumed(1265700);
        }

        [TestMethod]
        public void Test_ContainsText()
        {
            var array = new List<object>
            {
                "Hello",
                "AA",
                "bbb"
            };
            Assert.IsTrue(Contract.ContainsText(array, "bbb"));
            AssertEpicPulseConsumed(1245450);
            Assert.IsFalse(Contract.ContainsText(array, "c"));
            AssertEpicPulseConsumed(1245870);
        }

        [TestMethod]
        public void Test_ContainsPerson()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.ContainsPerson(array, 0));
            AssertEpicPulseConsumed(9681900);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPerson(array, 1));
            AssertEpicPulseConsumed(11873970);
            Assert.IsTrue(Contract.ContainsPersonIndex(array, 0));
            AssertEpicPulseConsumed(9889770);
        }

        [TestMethod]
        public void Test_ContainsPersonS()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsTrue(Contract.ContainsPersonS(array, 0));
            AssertEpicPulseConsumed(10378440);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPersonS(array, 10));
            AssertEpicPulseConsumed(12797820);
            Assert.IsTrue(Contract.ContainsPersonS(array, -100));
            AssertEpicPulseConsumed(12776340);
        }

        [TestMethod]
        public void Test_FirstGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100,
                1
            };
            Assert.AreEqual(1, Contract.FirstGreaterThanZero(array));
            AssertEpicPulseConsumed(1245210);

            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            Assert.AreEqual(2, Contract.FirstGreaterThanZero(array));
            AssertEpicPulseConsumed(1184280);
        }

        [TestMethod]
        public void Test_SelectTwice()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.SelectTwice(array)!;
            AssertEpicPulseConsumed(1963980);
            Assert.AreEqual(3, result.Count);

            array.Add(5);
            result = (Array)Contract.SelectTwice(array)!;
            AssertEpicPulseConsumed(2230380);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(-2, result[1]);
            Assert.AreEqual(-200, result[2]);
            Assert.AreEqual(10, result[3]);
        }

        [TestMethod]
        public void Test_SelectPersonS()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100),
                new BigInteger(5)
            };
            var result = (Array)Contract.SelectPersonS(array)!;
            AssertEpicPulseConsumed(14934450);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[0])[1].GetInteger());
            Assert.AreEqual(array[1], ((Struct)result[1])[1].GetInteger());
            Assert.AreEqual(array[2], ((Struct)result[2])[1].GetInteger());
            Assert.AreEqual(array[3], ((Struct)result[3])[1].GetInteger());
        }

        [TestMethod]
        public void Test_Skip()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100)
            };
            var result = (Array)Contract.Skip(array, 0)!;
            AssertEpicPulseConsumed(1892520);
            Assert.AreEqual(3, result.Count);

            array.Add(new BigInteger(1));
            array.Add(new BigInteger(5));
            array.Add(new BigInteger(100));

            result = (Array)Contract.Skip(array, 2)!;
            AssertEpicPulseConsumed(2148660);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(-100, result[0]);
            Assert.AreEqual(100, result[3].GetInteger());
            Assert.AreEqual(100, result[3].GetInteger());
        }

        [TestMethod]
        public void Test_Sum()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(-101, Contract.Sum(array));
            AssertEpicPulseConsumed(1155330);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.Sum(array));
            AssertEpicPulseConsumed(1165140);
            Assert.AreEqual(10, Contract.SumTwice(array));
            AssertEpicPulseConsumed(1291770);
        }

        [TestMethod]
        public void Test_Take()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.Take(array, 0)!;
            AssertEpicPulseConsumed(1148700);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)Contract.Take(array, 2)!;
            AssertEpicPulseConsumed(1647690);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0]);
        }

        [TestMethod]
        public void Test_ToMap()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100),
                new BigInteger(5)
            };
            var result = (Map)Contract.ToMap(array)!;
            AssertEpicPulseConsumed(11873580);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[array[0]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[1], ((Struct)result[array[1]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[2], ((Struct)result[array[2]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[3], ((Struct)result[array[3]!.ToString()!])[1].GetInteger());
        }

        [TestMethod]
        public void Test_WhereGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.WhereGreaterThanZero(array)!;
            AssertEpicPulseConsumed(1225800);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)Contract.WhereGreaterThanZero(array)!;
            AssertEpicPulseConsumed(2044800);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);
        }
    }
}
