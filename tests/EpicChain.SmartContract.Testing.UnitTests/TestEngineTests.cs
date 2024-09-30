using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using EpicChain.SmartContract.Testing.Extensions;
using EpicChain.SmartContract.Testing.Native;
using EpicChain.VM;
using EpicChain.VM.Types;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestEngineTests
    {
        public abstract class MyUndeployedContract : SmartContract
        {
            public abstract int myReturnMethod();
            protected MyUndeployedContract(SmartContractInitialize initialize) : base(initialize) { }
        }

        //[TestMethod]
        public void GenerateNativeArtifacts()
        {
            foreach (var n in EpicChain.SmartContract.Native.NativeContract.Contracts)
            {
                var manifest = n.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;
                var source = manifest.GetArtifactsSource(manifest.Name, generateProperties: true);
                var fullPath = Path.GetFullPath($"../../../../../src/EpicChain.SmartContract.Testing/Native/{manifest.Name}.cs");

                File.WriteAllText(fullPath, source);
            }
        }

        [TestMethod]
        public void TestOnGetEntryScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetEntryScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual("0xfa99b1aeedab84a47856358515e7f982341aa767", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());

            engine.OnGetEntryScriptHash = (current, expected) => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());
        }

        [TestMethod]
        public void TestOnGetCallingScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetCallingScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual(StackItem.Null, engine.Execute(script));

            engine.OnGetCallingScriptHash = (current, expected) => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());
        }

        [TestMethod]
        public void TestHashExists()
        {
            TestEngine engine = new(false);

            Assert.ThrowsException<KeyNotFoundException>(() => engine.FromHash<EpicChain\src\>(engine.Native.EpicChain.Hash, true));

            engine.Native.Initialize(false);

            Assert.IsInstanceOfType<EpicChain\src\>(engine.FromHash<EpicChain\src\>(engine.Native.EpicChain.Hash, true));
        }

        [TestMethod]
        public void TestCustomMock()
        {
            // Initialize TestEngine and native smart contracts

            TestEngine engine = new(true);

            // Get epicchain token smart contract and mock balanceOf to always return 123

            var epicchain = engine.FromHash<EpicChain\src\>(engine.Native.EpicChain.Hash,
                mock => mock.Setup(o => o.BalanceOf(It.IsAny<UInt160>())).Returns(new BigInteger(123)),
                false);

            // Test direct call

            Assert.AreEqual(123, EpicChain.BalanceOf(engine.ValidatorsAddress));

            // Test vm call

            using (ScriptBuilder script = new())
            {
                script.EmitDynamicCall(EpicChain.Hash, "balanceOf", engine.ValidatorsAddress);

                Assert.AreEqual(123, engine.Execute(script.ToArray()).GetInteger());
            }

            // Test mock on undeployed contract

            var undeployed = engine.FromHash<MyUndeployedContract>(UInt160.Zero,
                mock => mock.Setup(o => o.myReturnMethod()).Returns(1234),
                false);

            using (ScriptBuilder script = new())
            {
                script.EmitDynamicCall(UInt160.Zero, nameof(undeployed.myReturnMethod));

                Assert.AreEqual(1234, engine.Execute(script.ToArray()).GetInteger());
            }
        }

        [TestMethod]
        public void TestNativeContracts()
        {
            TestEngine engine = new(false);

            Assert.AreEqual(engine.Native.ContractManagement.Hash, EpicChain.SmartContract.Native.NativeContract.ContractManagement.Hash);
            Assert.AreEqual(engine.Native.StdLib.Hash, EpicChain.SmartContract.Native.NativeContract.StdLib.Hash);
            Assert.AreEqual(engine.Native.CryptoLib.Hash, EpicChain.SmartContract.Native.NativeContract.CryptoLib.Hash);
            Assert.AreEqual(engine.Native.GAS.Hash, EpicChain.SmartContract.Native.NativeContract.GAS.Hash);
            Assert.AreEqual(engine.Native.EpicChain.Hash, EpicChain.SmartContract.Native.NativeContract.EpicChain.Hash);
            Assert.AreEqual(engine.Native.Oracle.Hash, EpicChain.SmartContract.Native.NativeContract.Oracle.Hash);
            Assert.AreEqual(engine.Native.Policy.Hash, EpicChain.SmartContract.Native.NativeContract.Policy.Hash);
            Assert.AreEqual(engine.Native.RoleManagement.Hash, EpicChain.SmartContract.Native.NativeContract.RoleManagement.Hash);
        }

        [TestMethod]
        public void FromHashWithoutCheckTest()
        {
            UInt160 hash = UInt160.Parse("0x1230000000000000000000000000000000000000");
            TestEngine engine = new(false);

            var contract = engine.FromHash<ContractManagement>(hash, false);

            Assert.AreEqual(contract.Hash, hash);
        }

        [TestMethod]
        public void FromHashTest()
        {
            // Create the engine initializing the native contracts

            var engine = new TestEngine(true);

            // Instantiate epicchain contract from native hash, (not necessary if we use engine.Native.EpicChain\src\)

            var epicchain = engine.FromHash<EpicChain\src\>(engine.Native.EpicChain.Hash, true);

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(1_000_000_000, EpicChain.TotalSupply);
            Assert.AreEqual(EpicChain.TotalSupply, EpicChain.BalanceOf(engine.ValidatorsAddress));
        }
    }
}
