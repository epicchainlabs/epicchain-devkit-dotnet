using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    EpicChain.SmartContract.Analyzer.SupportedStandardsAnalyzer,
    EpicChain.SmartContract.Analyzer.SupportedStandardsCodeFixProvider>;

namespace EpicChain.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class SupportedStandardsAnalyzerUnitTest
    {
        private const string TestNamespace = """

                                             using System;

                                             public enum XepStandard
                                             {
                                                 // The XEP-11 standard is used for non-fungible tokens (NFTs).
                                                 // Defined at https://github.com/neo-project/proposals/blob/master/XEP-11.mediawiki
                                                 XEP11,
                                                 // The XEP-17 standard is used for fungible tokens.
                                                 // Defined at https://github.com/neo-project/proposals/blob/master/XEP-17.mediawiki
                                                 Xep17
                                             }

                                             [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
                                             public class SupportedStandardsAttribute : Attribute
                                             {
                                                 public SupportedStandardsAttribute(params string[] supportedStandards){}

                                                 public SupportedStandardsAttribute(params XepStandard[] supportedStandards){}
                                             }

                                             """;
        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UnsupportedStandard_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("XEP5")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 28).WithArguments("XEP5");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_XEP11Suggestion_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("XEP11")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(XepStandard.XEP11)]");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_Xep17Suggestion_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("XEP17")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(XepStandard.Xep17)]");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UpdateXEP11_ShouldFixCode()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("XEP11")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            const string fixedCode = TestNamespace + """

                                                     [SupportedStandards(XepStandard.XEP11)]
                                                     public class TestContract
                                                     {
                                                         public static void Main()
                                                         {
                                                         }
                                                     }
                                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(XepStandard.XEP11)]");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UpdateXep17_ShouldFixCode()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("XEP17")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            const string fixedCode = TestNamespace + """

                                                     [SupportedStandards(XepStandard.Xep17)]
                                                     public class TestContract
                                                     {
                                                         public static void Main()
                                                         {
                                                         }
                                                     }
                                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(XepStandard.Xep17)]");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }
    }
}
