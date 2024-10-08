using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.VM.Types;
using System.Linq;
using System.Numerics;
using EpicChain.SmartContract.Testing.Exceptions;
using Array = EpicChain.VM.Types.Array;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Array : DebugAndTestBase<Contract_Array>
    {
        [TestMethod]
        public void Test_GetTreeByteLengthPrefix()
        {
            using var fee = Engine.CreateEpicPulseWatcher();
            var result = Contract.GetTreeByteLengthPrefix();
            Assert.AreEqual(1784760, fee.Value);

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x03 }, result);
        }

        [TestMethod]
        public void Test_GetTreeByteLengthPrefix2()
        {
            var result = Contract.GetTreeByteLengthPrefix2();
            AssertEpicPulseConsumed(1784760);

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x03 }, result);
        }

        [TestMethod]
        public void Test_JaggedArray()
        {
            var arr = Contract.TestJaggedArray();
            AssertEpicPulseConsumed(2094930);

            Assert.AreEqual(4, arr?.Count);
            var element0 = (Array?)arr?[0];
            CollectionAssert.AreEqual(new Integer[] { 1, 2, 3, 4 }, element0?.ToArray());
        }

        [TestMethod]
        public void Test_JaggedByteArray()
        {
            var arr = Contract.TestJaggedByteArray();
            AssertEpicPulseConsumed(2832570);

            Assert.AreEqual(4, arr?.Count);
            var element0 = (byte[]?)arr?[0];
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, element0);
        }

        [TestMethod]
        public void Test_EmptyArray()
        {
            var arr = Contract.TestEmptyArray();
            AssertEpicPulseConsumed(1787220);

            Assert.AreEqual(0, arr?.Count);
        }

        [TestMethod]
        public void Test_IntArray()
        {
            var arr = Contract.TestIntArray();
            AssertEpicPulseConsumed(2540310);

            //test 0,1,2
            CollectionAssert.AreEqual(new BigInteger[] { 0, 1, 2 }, arr?.ToArray());
        }

        [TestMethod]
        public void Test_IntArrayInit()
        {
            //test 1,4,5
            var arr = Contract.TestIntArrayInit();
            AssertEpicPulseConsumed(2340420);
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());


            //test 1,4,5
            arr = Contract.TestIntArrayInit2();
            AssertEpicPulseConsumed(2340420);
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());

            //test 1,4,5
            arr = Contract.TestIntArrayInit3();
            AssertEpicPulseConsumed(2340420);
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());
        }

        [TestMethod]
        public void Test_StructArray()
        {
            var result = Contract.TestStructArray();
            AssertEpicPulseConsumed(3543240);

            //test (1+5)*7 == 42
            var bequal = result as Struct != null;
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_DefaultState()
        {
            var result = Contract.TestDefaultState();
            var state = result as Struct;
            Assert.IsNotNull(state);
            Assert.AreEqual(3, state.Count);
            Assert.IsInstanceOfType(state[0], typeof(VM.Types.Null));
            Assert.IsInstanceOfType(state[1], typeof(VM.Types.Null));
            Assert.IsInstanceOfType(state[2], typeof(VM.Types.Integer));
            Assert.AreEqual(0, state[2]);
            AssertEpicPulseConsumed(3279750);
        }

        [TestMethod]
        public void Test_StructArrayInit()
        {
            var result = Contract.TestStructArrayInit();
            AssertEpicPulseConsumed(3343410);

            //test (1+5)*7 == 42
            var bequal = result as Struct != null;
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_ByteArrayOwner()
        {
            var bts = Contract.TestByteArrayOwner();
            AssertEpicPulseConsumed(1784760);

            CollectionAssert.AreEqual(new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe }, bts);
        }

        [TestMethod]
        public void Test_DynamicArrayInit()
        {
            var arr = Contract.TestDynamicArrayInit(3);
            AssertEpicPulseConsumed(2605350);

            Assert.AreEqual(3, arr?.Count);
            Assert.AreEqual(new BigInteger(0), arr?[0]);
            Assert.AreEqual(new BigInteger(1), arr?[1]);
            Assert.AreEqual(new BigInteger(2), arr?[2]);

            arr = Contract.TestDynamicArrayInit(0);
            AssertEpicPulseConsumed(1863750);
            Assert.AreEqual(0, arr?.Count);
            Assert.ThrowsException<TestException>(() => Contract.TestDynamicArrayInit(-1));
            Assert.ThrowsException<TestException>(() => Contract.TestDynamicArrayInit(int.MaxValue));
        }

        [TestMethod]
        public void Test_DefaultArray()
        {
            var arr = Contract.TestDefaultArray();
            AssertEpicPulseConsumed(1805160);
            Assert.IsTrue(arr!.Value);
        }

        [TestMethod]
        public void Test_DynamicArrayStringInit()
        {
            var arr = Contract.TestDynamicArrayStringInit("hello");
            AssertEpicPulseConsumed(1855710);

            Assert.AreEqual(5, arr?.Length);
            Assert.IsTrue(arr?.All(a => a == 0));
        }

        [TestMethod]
        public void Test_ByteArrayOwnerCall()
        {
            var bts = Contract.TestByteArrayOwnerCall();
            AssertEpicPulseConsumed(2048100);

            CollectionAssert.AreEqual(new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe }, bts);
        }

        [TestMethod]
        public void Test_StringArray()
        {
            var items = Contract.TestSupportedStandards();
            AssertEpicPulseConsumed(1784760);

            Assert.AreEqual((ByteString)"XEP-5", items?[0]);
            Assert.AreEqual((ByteString)"XEP-10", items?[1]);
        }

        [TestMethod]
        public void Test_Collectionexpressions()
        {
            var arr = Contract.TestCollectionexpressions();
            AssertEpicPulseConsumed(3387420);

            Assert.AreEqual(4, arr?.Count);

            var element0 = (Array?)arr?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                element0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());

            var element1 = (Array?)arr?[1];
            CollectionAssert.AreEqual(new[] { "one", "two", "three" },
                element1?.Cast<PrimitiveType>().Select(u => u.GetString()).ToArray());

            var element2 = (Array?)arr?[2];
            Assert.AreEqual(3, element2?.Count);
            var element2_0 = (Array?)element2?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 },
                element2_0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());

            var element3 = (Array?)arr?[3];
            Assert.AreEqual(3, element3?.Count);
            var element3_0 = (Array?)element3?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 },
                element3_0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());
        }

        [TestMethod]
        public void Test_ElementBinding()
        {
            Contract.TestElementBinding();
            AssertEpicPulseConsumed(5907840);
        }
    }
}
