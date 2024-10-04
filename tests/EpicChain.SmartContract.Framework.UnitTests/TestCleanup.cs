using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Compiler;
using EpicChain.SmartContract.Testing.Coverage;
using EpicChain.SmartContract.Testing.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Util;
using Microsoft.CodeAnalysis;
using EpicChain.SmartContract.Testing.TestingStandards;
using CompilationOptions = EpicChain.Compiler.CompilationOptions;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s", RegexOptions.Compiled);
        public static readonly ConcurrentDictionary<Type, (CompilationContext Context, EpicChainDebugInfo? DbgInfo)> CachedContracts = new();
        private static readonly string TestContractsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "EpicChain.SmartContract.Framework.TestContracts", "EpicChain.SmartContract.Framework.TestContracts.csproj"));
        private static readonly string ArtifactsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "TestingArtifacts"));
        private static readonly string RootPath = Path.GetDirectoryName(TestContractsPath) ?? string.Empty;

        private static readonly Lazy<CompilationEngine> _compilationEngine = new(() => new CompilationEngine(new CompilationOptions
        {
            Debug = true,
            CompilerVersion = "TestingEngine",
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable
        }));

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static List<INamedTypeSymbol> _sortedClasses;
        private static Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> _classDependencies;
        private static List<INamedTypeSymbol?> _allClassSymbols;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static readonly ConcurrentSet<string> UpdatedArtifactNames = new();

        [AssemblyInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void TestAssemblyInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            (_sortedClasses, _classDependencies, _allClassSymbols) = _compilationEngine.Value.PrepareProjectContracts(TestContractsPath);
        }

        public static void TestInitialize(Type contract)
        {
            if (!typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(contract))
            {
                throw new InvalidOperationException($"The type {contract.Name} does not inherit from SmartContract.Testing.SmartContract");
            }
            if (CachedContracts.ContainsKey(contract)) return;
            EnsureArtifactUpToDateInternal(contract.Name);
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            if (UpdatedArtifactNames.Count > 0)
                Assert.Fail($"Some artifacts were updated: {string.Join(", ", UpdatedArtifactNames)}. Please rerun the tests.");

            var list = _sortedClasses.Select(u => u.Name).ToList();

            foreach (var cl in CachedContracts)
            {
                list.Remove(cl.Key.Name);
            }

            if (list.Count == 0)
                EnsureCoverageInternal(Assembly.GetExecutingAssembly(), CachedContracts.Select(u => (u.Key, u.Value.DbgInfo)));
            else
            {
                Console.Error.WriteLine("Coverage not found for:");

                foreach (var line in list)
                {
                    Console.Error.WriteLine($"- {line}");
                }
            }
        }

        private static void EnsureArtifactUpToDateInternal(string singleContractName)
        {
            var result = _compilationEngine.Value.CompileProject(TestContractsPath, _sortedClasses, _classDependencies, _allClassSymbols, singleContractName).FirstOrDefault() ?? throw new InvalidOperationException($"No compilation result found for {singleContractName}");
            if (!result.Success)
            {
                var errors = string.Join(Environment.NewLine, result.Diagnostics
                    .Where(u => u.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));
                Assert.Fail($"Error compiling {result.ContractName}: {errors}");
            }

            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => typeof(Testing.SmartContract).IsAssignableFrom(t) &&
                                     t.Name.Equals(result.ContractName, StringComparison.OrdinalIgnoreCase));

            if (type == null)
            {
                Assert.Fail($"Could not find type for contract {result.ContractName}");
                return;
            }
            var debug = CreateArtifactAsync(result.ContractName!, result, RootPath, Path.Combine(ArtifactsPath, $"{result.ContractName}.cs")).GetAwaiter().GetResult();
            CachedContracts[type] = (result, debug);
        }

        private static async Task<EpicChainDebugInfo?> CreateArtifactAsync(string typeName, CompilationContext context, string rootDebug, string artifactsPath)
        {
            var (nef, manifest, debugInfo) = context.CreateResults(rootDebug);
            var debug = EpicChainDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            var writtenArtifact = File.Exists(artifactsPath)
                ? await File.ReadAllTextAsync(artifactsPath)
                : "";

            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        File.WriteAllText(artifactsPath, artifact);
                        Console.WriteLine($"{typeName} artifact was updated");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing artifact for {typeName}: {ex.Message}");
                    }
                });
                return null;
            }

            return debug;
        }
    }
}
