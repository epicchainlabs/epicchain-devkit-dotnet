using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using EpicChain.SmartContract.Testing.Coverage;
using System.Numerics;
using System.Text.RegularExpressions;

namespace EpicChain.SmartContract.Testing.UnitTests.Coverage
{
    [TestClass]
    public class CoverageDataTests
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");

        [TestMethod]
        public void TestDump()
        {
            var engine = new TestEngine(true);

            // Check totalSupply

            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);

            Assert.AreEqual(WhiteSpaceRegex.Replace(@"
EpicChain [0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5] [5.00 % - 100.00 %]
┌-───────────────────────────────-┬-────────-┬-────────-┐
│ Method                          │   Line   │   Branch │
├-───────────────────────────────-┼-────────-┼-────────-┤
│ totalSupply()                   │ 100.00 % │ 100.00 % │
│ balanceOf(account)              │   0.00 % │ 100.00 % │
│ decimals()                      │   0.00 % │ 100.00 % │
│ getAccountState(account)        │   0.00 % │ 100.00 % │
│ getAllCandidates()              │   0.00 % │ 100.00 % │
│ getCandidates()                 │   0.00 % │ 100.00 % │
│ getCandidateVote(pubKey)        │   0.00 % │ 100.00 % │
│ getCommittee()                  │   0.00 % │ 100.00 % │
│ getCommitteeAddress()           │   0.00 % │ 100.00 % │
│ GetEpicPulsePerBlock()                │   0.00 % │ 100.00 % │
│ getNextBlockValidators()        │   0.00 % │ 100.00 % │
│ getRegisterPrice()              │   0.00 % │ 100.00 % │
│ registerCandidate(pubkey)       │   0.00 % │ 100.00 % │
│ setEpicPulsePerBlock(epicpulsePerBlock)     │   0.00 % │ 100.00 % │
│ setRegisterPrice(registerPrice) │   0.00 % │ 100.00 % │
│ symbol()                        │   0.00 % │ 100.00 % │
│ transfer(from,to,amount,data)   │   0.00 % │ 100.00 % │
│ UnclaimedEpicPulse(account,end)       │   0.00 % │ 100.00 % │
│ unregisterCandidate(pubkey)     │   0.00 % │ 100.00 % │
│ vote(account,voteTo)            │   0.00 % │ 100.00 % │
└-───────────────────────────────-┴-────────-┴-────────-┘
", ""), WhiteSpaceRegex.Replace(engine.GetCoverage(engine.Native.EpicChain)?.Dump()!, ""));

            Assert.AreEqual(WhiteSpaceRegex.Replace(@"
EpicChain [0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5] [5.00 % - 100.00 %]
┌-─────────────-┬-────────-┬-────────-┐
│ Method        │   Line   │   Branch │
├-─────────────-┼-────────-┼-────────-┤
│ totalSupply() │ 100.00 % │ 100.00 % │
└-─────────────-┴-────────-┴-────────-┘
", ""), WhiteSpaceRegex.Replace((engine.Native.EpicChain.GetCoverage(o => o.TotalSupply) as CoveredMethod)?.Dump()!, ""));
        }

        [TestMethod]
        public void TestCoverageByEngine()
        {
            // Create the engine initializing the native contracts
            // Native contracts use 3 opcodes per method

            //{
            //    sb.EmitPush(0); //version
            //    sb.EmitSysCall(ApplicationEngine.System_Contract_CallNative);
            //    sb.Emit(OpCode.RET);
            //}

            var engine = new TestEngine(true);

            // Check totalSupply

            Assert.IsNotNull(engine.GetCoverage(engine.Native.EpicChain));
            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);

            Assert.AreEqual(engine.Native.EpicChain.Hash, engine.GetCoverage(engine.Native.EpicChain)?.Hash);
            Assert.AreEqual(60, engine.GetCoverage(engine.Native.EpicChain)?.TotalLines);
            Assert.AreEqual(3, engine.GetCoverage(engine.Native.EpicChain)?.CoveredLines);
            Assert.AreEqual(3, engine.GetCoverage(engine.Native.EpicChain)?.CoveredLinesAll);

            // Check balanceOf

            Assert.AreEqual(0, engine.Native.EpicChain.BalanceOf(engine.Native.EpicChain.Hash));

            Assert.AreEqual(60, engine.GetCoverage(engine.Native.EpicChain)?.TotalLines);
            Assert.AreEqual(6, engine.GetCoverage(engine.Native.EpicChain)?.CoveredLines);
            Assert.AreEqual(6, engine.GetCoverage(engine.Native.EpicChain)?.CoveredLinesAll);

            // Check coverage by method and expression

            var methodCovered = engine.GetCoverage(engine.Native.Oracle, o => o.Finish());
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, o => o.TotalSupply);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, o => o.RegisterPrice);
            Assert.AreEqual(6, methodCovered?.TotalLines);
            Assert.AreEqual(0, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, o => o.BalanceOf(It.IsAny<UInt160>()));
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, o => o.Transfer(It.IsAny<UInt160>(), It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()));
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(0, methodCovered?.CoveredLines);

            // Check coverage by raw method

            methodCovered = engine.GetCoverage(engine.Native.Oracle, "finish", 0);
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "transfer", 4);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(0, methodCovered?.CoveredLines);
        }

        [TestMethod]
        public void TestCoverageByExtension()
        {
            // Create the engine initializing the native contracts
            // Native contracts use 3 opcodes per method

            //{
            //    sb.EmitPush(0); //version
            //    sb.EmitSysCall(ApplicationEngine.System_Contract_CallNative);
            //    sb.Emit(OpCode.RET);
            //}

            var engine = new TestEngine(true);

            // Check totalSupply

            Assert.IsNotNull(engine.Native.EpicChain.GetCoverage());
            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);

            Assert.AreEqual(engine.Native.EpicChain.Hash, engine.Native.EpicChain.GetCoverage()?.Hash);
            Assert.AreEqual(60, engine.Native.EpicChain.GetCoverage()?.TotalLines);
            Assert.AreEqual(3, engine.Native.EpicChain.GetCoverage()?.CoveredLines);
            Assert.AreEqual(3, engine.Native.EpicChain.GetCoverage()?.CoveredLinesAll);

            // Check balanceOf

            Assert.AreEqual(0, engine.Native.EpicChain.BalanceOf(engine.Native.EpicChain.Hash));

            Assert.AreEqual(60, engine.Native.EpicChain.GetCoverage()?.TotalLines);
            Assert.AreEqual(6, engine.Native.EpicChain.GetCoverage()?.CoveredLines);
            Assert.AreEqual(6, engine.Native.EpicChain.GetCoverage()?.CoveredLinesAll);

            // Check coverage by method and expression

            var methodCovered = engine.Native.Oracle.GetCoverage(o => o.Finish());
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.Native.EpicChain.GetCoverage(o => o.TotalSupply);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.Native.EpicChain.GetCoverage(o => o.BalanceOf(It.IsAny<UInt160>()));
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.Native.EpicChain.GetCoverage(o => o.Transfer(It.IsAny<UInt160>(), It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()));
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(0, methodCovered?.CoveredLines);

            // Check coverage by raw method

            methodCovered = engine.GetCoverage(engine.Native.Oracle, "finish", 0);
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(3, methodCovered?.CoveredLines);

            methodCovered = engine.GetCoverage(engine.Native.EpicChain, "transfer", 4);
            Assert.AreEqual(3, methodCovered?.TotalLines);
            Assert.AreEqual(0, methodCovered?.CoveredLines);
        }

        [TestMethod]
        public void TestHits()
        {
            var coverage = new CoverageHit(0, "test");

            Assert.AreEqual(0, coverage.Hits);
            Assert.AreEqual("test", coverage.Description);
            Assert.AreEqual(0, coverage.FeeAvg);
            Assert.AreEqual(0, coverage.FeeMax);
            Assert.AreEqual(0, coverage.FeeMin);
            Assert.AreEqual(0, coverage.FeeTotal);

            coverage.Hit(123);

            Assert.AreEqual(1, coverage.Hits);
            Assert.AreEqual(123, coverage.FeeAvg);
            Assert.AreEqual(123, coverage.FeeMax);
            Assert.AreEqual(123, coverage.FeeMin);
            Assert.AreEqual(123, coverage.FeeTotal);

            coverage.Hit(377);

            Assert.AreEqual(2, coverage.Hits);
            Assert.AreEqual(250, coverage.FeeAvg);
            Assert.AreEqual(377, coverage.FeeMax);
            Assert.AreEqual(123, coverage.FeeMin);
            Assert.AreEqual(500, coverage.FeeTotal);

            coverage.Hit(500);

            Assert.AreEqual(3, coverage.Hits);
            Assert.AreEqual(333, coverage.FeeAvg);
            Assert.AreEqual(500, coverage.FeeMax);
            Assert.AreEqual(123, coverage.FeeMin);
            Assert.AreEqual(1000, coverage.FeeTotal);

            coverage.Hit(0);

            Assert.AreEqual(4, coverage.Hits);
            Assert.AreEqual(250, coverage.FeeAvg);
            Assert.AreEqual(500, coverage.FeeMax);
            Assert.AreEqual(0, coverage.FeeMin);
            Assert.AreEqual(1000, coverage.FeeTotal);
        }
    }
}
