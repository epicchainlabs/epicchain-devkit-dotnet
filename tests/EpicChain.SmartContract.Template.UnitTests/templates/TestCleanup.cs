using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Compiler;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Coverage;
using EpicChain.SmartContract.Testing.Extensions;
using System.Reflection;
using System.Text.RegularExpressions;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        public static readonly ConcurrentDictionary<Type, (CompilationContext Context, EpicChainDebugInfo? DbgInfo)> CachedContracts = new();

        [AssemblyCleanup]
        public static void EnsureCoverage() => EnsureCoverageInternal(Assembly.GetExecutingAssembly(), CachedContracts.Select(u => (u.Key, u.Value.DbgInfo)));

        [TestMethod]
        public void EnsureArtifactsUpToDate()
        {
            if (CachedContracts.Count > 0) return; // Maybe a UT call it

            // Define paths

            string frameworkPath = Path.GetFullPath("../../../../../src/EpicChain.SmartContract.Framework/EpicChain.SmartContract.Framework.csproj");
            string templatePath = Path.GetFullPath("../../../../../src/EpicChain.SmartContract.Template/templates");
            string artifactsPath = Path.GetFullPath("../../../templates");

            // Compile

            var result = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(new CompilationSourceReferences() { Projects = [frameworkPath] },
                [
                    Path.Combine(templatePath, "epicchaincontractxep17/Xep17Contract.cs"),
                    Path.Combine(templatePath, "epicchaincontractoracle/OracleRequest.cs"),
                    Path.Combine(templatePath, "epicchaincontractowner/Ownable.cs")
                ]);

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Xep17

            var root = Path.GetPathRoot(templatePath) ?? "";
            var context = result.FirstOrDefault(p => p.ContractName == "Xep17Contract") ?? throw new InvalidOperationException();
            (var artifact, var dbg) = CreateArtifact<Xep17ContractTemplate>(context, root,
                Path.Combine(artifactsPath, "epicchaincontractxep17/TestingArtifacts/Xep17ContractTemplate.artifacts.cs"));

            CachedContracts[typeof(Xep17ContractTemplate)] = (context, dbg);

            // Ensure Oracle

            context = result.FirstOrDefault(p => p.ContractName == "OracleRequest") ?? throw new InvalidOperationException();
            (artifact, dbg) = CreateArtifact<OracleRequestTemplate>(context, root,
                Path.Combine(artifactsPath, "epicchaincontractoracle/TestingArtifacts/OracleRequestTemplate.artifacts.cs"));

            CachedContracts[typeof(OracleRequestTemplate)] = (context, dbg);

            // Ensure Ownable

            context = result.FirstOrDefault(p => p.ContractName == "Ownable") ?? throw new InvalidOperationException();
            (artifact, dbg) = CreateArtifact<OwnableTemplate>(context, root,
                Path.Combine(artifactsPath, "epicchaincontractowner/TestingArtifacts/OwnableTemplate.artifacts.cs"));

            CachedContracts[typeof(OwnableTemplate)] = (context, dbg);
        }

        private static (string artifact, EpicChainDebugInfo debugInfo) CreateArtifact<T>(CompilationContext context, string rootDebug, string artifactsPath)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = EpicChainDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeof(T).Name, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);
                Assert.Fail($"{typeof(T).Name} artifact was wrong");
            }

            return (artifact, debug);
        }
    }
}
