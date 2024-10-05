using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    EpicChain.SmartContract.Analyzer.LinqUsageAnalyzer,
    EpicChain.SmartContract.Analyzer.LinqUsageCodeFixProvider>;

namespace EpicChain.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class LinqUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task LinqUsage_ShouldReportDiagnostic()
        {
            var test = """
                       using System;
                       using System.Linq;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               var numbers = new int[] { 1, 2, 3, 4, 5 };
                               var evenNumbers = numbers.Where(x => x % 2 == 0);
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                .WithLocation(2, 1)
                .WithArguments("System.Linq");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task LinqUsage_ShouldChangeTo_EpicChainLinq()
        {
            var test = """
                       using System;
                       using System.Linq;

                       namespace EpicChain.SmartContract.Framework.Linq
                       {
                           public static class LinqExtensions
                           {}
                       }

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               var numbers = new int[] { 1, 2, 3, 4, 5 };
                           }
                       }
                       """;

            var fixtest = """
                          using System;
                          using EpicChain.SmartContract.Framework.Linq;

                          namespace EpicChain.SmartContract.Framework.Linq
                          {
                              public static class LinqExtensions
                              {}
                          }

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  var numbers = new int[] { 1, 2, 3, 4, 5 };
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                .WithLocation(2, 1)
                .WithArguments("System.Linq");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }
    }
}
