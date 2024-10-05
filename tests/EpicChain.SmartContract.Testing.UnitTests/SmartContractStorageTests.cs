using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class SmartContractStorageTests
    {
        // Defines the prefix used to store the registration price in EpicChain

        private readonly byte[] _registerPricePrefix = new byte[] { 13 };

        [TestMethod]
        public void TestAlterStorage()
        {
            // Create and initialize TestEngine

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.EpicChain.RegisterPrice);

            // Alter data

            engine.Native.EpicChain.Storage.Put(_registerPricePrefix, BigInteger.MinusOne);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.EpicChain.RegisterPrice);
        }

        [TestMethod]
        public void TestExportImport()
        {
            // Create and initialize TestEngine

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.EpicChain.RegisterPrice);

            var storage = engine.Native.EpicChain.Storage.Export();

            // Alter data

            storage[storage.Properties.First().Key]![Convert.ToBase64String(_registerPricePrefix)] = Convert.ToBase64String(BigInteger.MinusOne.ToByteArray());
            engine.Native.EpicChain.Storage.Import(storage);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.EpicChain.RegisterPrice);
        }
    }
}
