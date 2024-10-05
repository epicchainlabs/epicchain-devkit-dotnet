using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Framework.UnitTests;

public class DebugAndTestBase<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract, IContractInfo
{

    internal bool TestGasConsume { set; get; } = true;

    static DebugAndTestBase()
    {
        TestCleanup.TestInitialize(typeof(T));
    }

    protected void AssertEpicPulseConsumed(long epicpulseConsumed)
    {
        if (TestGasConsume)
            Assert.AreEqual(epicpulseConsumed, Engine.FeeConsumed.Value);
    }
}
