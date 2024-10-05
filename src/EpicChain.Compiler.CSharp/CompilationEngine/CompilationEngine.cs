// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp is open-source software that is distributed under the widely recognized and permissive MIT License.
// This software is intended to provide developers with a powerful framework to create and deploy smart contracts on the EpicChain blockchain,
// and it is made freely available to all individuals and organizations. Whether you are building for personal, educational, or commercial
// purposes, you are welcome to utilize this framework with minimal restrictions, promoting the spirit of open innovation and collaborative
// development within the blockchain ecosystem.
//
// As a permissive license, the MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
// source code or its binary versions as needed. You are permitted to incorporate the EpicChain Lab's Project into your own
// projects, whether for profit or non-profit, and may make changes to suit your specific needs. There is no requirement to make your
// modifications open-source, though doing so contributes to the overall growth of the open-source community.
//
// To comply with the terms of the MIT License, we kindly ask that you include a copy of the original copyright notice, this permission
// notice, and the license itself in all substantial portions of the software that you redistribute, whether in source code form or as
// compiled binaries. The purpose of this requirement is to ensure that future users and developers are aware of the origin of the software,
// the freedoms granted to them, and the limitations of liability that apply.
//
// The complete terms and conditions of the MIT License are documented in the LICENSE file that accompanies this project. This file can be
// found in the main directory of the source code repository. Alternatively, you may access the license text online at the following URL:
// http://www.opensource.org/licenses/mit-license.php. We encourage you to review these terms in detail to fully understand your rights
// and responsibilities when using this software.
//
// Redistribution and use of the EpicChain Lab's Project, whether in source or binary forms, with or without modification, are
// permitted as long as the following conditions are met:
//
// 1. The original copyright notice, along with this permission notice, must be retained in all copies or significant portions of the software.
// 2. The software is provided "as-is," without any express or implied warranty. This means that the authors and contributors are not
//    responsible for any issues that may arise from the use of the software, including but not limited to damages caused by defects or
//    performance issues. Users assume all responsibility for determining the suitability of the software for their specific needs.
//
// In addition to the above terms, the authors of the EpicChain Lab's Project encourage developers to explore and experiment
// with the framework's capabilities. Whether you are an individual developer, a startup, or a large organization, you are invited to
// leverage the power of blockchain technology to create decentralized applications, smart contracts, and more. We believe that by fostering
// a robust ecosystem of developers and contributors, we can help drive innovation in the blockchain space and unlock new opportunities
// for distributed ledger technology.
//
// However, please note that while the MIT License allows for modifications and redistribution, it does not imply endorsement of any
// derived works by the original authors. Therefore, if you significantly modify the EpicChain Lab's Project and redistribute it
// under your own brand or as part of a larger project, you must clearly indicate the changes you have made, and the original authors
// cannot be held liable for any issues resulting from your modifications.
//
// By choosing to use the EpicChain Lab's Project, you acknowledge that you have read and understood the terms of the MIT License.
// You agree to abide by these terms and recognize that this software is provided without warranty of any kind, express or implied, including
// but not limited to warranties of merchantability, fitness for a particular purpose, or non-infringement. Should any legal issues or
// disputes arise as a result of using this software, the authors and contributors disclaim all liability and responsibility.
//
// Finally, we encourage all users of the EpicChain Lab's Project to consider contributing back to the community. Whether through
// bug reports, feature suggestions, or code contributions, your involvement helps improve the framework for everyone. Open-source projects
// thrive when developers collaborate and share their knowledge, and we welcome your input as we continue to develop and refine the
// EpicChain ecosystem.


extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using BigInteger = System.Numerics.BigInteger;

namespace EpicChain.Compiler
{
    public class CompilationEngine(CompilationOptions options)
    {
        internal Compilation? Compilation;
        internal CompilationOptions Options { get; private set; } = options;
        private static readonly MetadataReference[] CommonReferences;
        private static readonly Dictionary<string, MetadataReference> MetaReferences = [];
        private static readonly Regex s_pattern = new(@"^(EpicChain\.SmartContract\.Framework\.SmartContract|SmartContract\.Framework\.SmartContract|Framework\.SmartContract|SmartContract)$");
        internal readonly ConcurrentDictionary<INamedTypeSymbol, CompilationContext> Contexts = new(SymbolEqualityComparer.Default);

        static CompilationEngine()
        {
            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            CommonReferences =
            [
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location)
            ];
        }

        internal List<CompilationContext> CompileFromCodeBlock(string codeBlock)
        {
            var sourceCode = $"using EpicChain.SmartContract.Framework.Native;\n" +
                             $"using EpicChain.SmartContract.Framework.Services;\n" +
                             $"using System;\n" +
                             $"using System.Text;\n" +
                             $"using System.Numerics;\n" +
                             $"using EpicChain.SmartContract.Framework;\n\n" +
                             $"namespace EpicChain.Compiler.CSharp.TestContracts;\n\n" +
                             $"public class CodeBlockTest : SmartContract.Framework.SmartContract\n{{\n\n" +
                             $"    public static void CodeBlock()\n" +
                             $"    {{\n" +
                                $"        {codeBlock}\n" +
                             $"    }}\n" +
                             $"}}\n";

            string tempFilePath = Path.GetTempFileName();
            string newTempFilePath = Path.ChangeExtension(tempFilePath, ".cs");
            File.Move(tempFilePath, newTempFilePath);
            tempFilePath = newTempFilePath;
            File.WriteAllText(tempFilePath, sourceCode);

            try
            {
                return CompileSources(tempFilePath);
            }
            finally { File.Delete(tempFilePath); }
        }

        public List<CompilationContext> Compile(IEnumerable<string> sourceFiles, IEnumerable<MetadataReference> references)
        {
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable, allowUnsafe: false);
            Compilation = CSharpCompilation.Create(null, syntaxTrees, references, compilationOptions);
            return CompileProjectContracts(Compilation);
        }

        public List<CompilationContext> CompileSources(params string[] sourceFiles)
        {
            return CompileSources(new CompilationSourceReferences()
            {
                Packages = [new("EpicChain.SmartContract.Framework", "1.0.0-*")]
            },
            sourceFiles);
        }

        public List<CompilationContext> CompileSources(CompilationSourceReferences references, params string[] sourceFiles)
        {
            // Generate a dummy csproj

            var packageGroup = references.Packages is null ? "" : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, references.Packages!.Select(u => $" <PackageReference Include =\"{u.packageName}\" Version=\"{u.packageVersion}\" />"))}
    </ItemGroup>";

            var projectsGroup = references.Projects is null ? "" : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, references.Projects!.Select(u => $" <ProjectReference Include =\"{u}\"/>"))}
    </ItemGroup>";

            var csproj = $@"
<Project Sdk=""Microsoft.NET.Sdk"">

    <PropertyGroup>
        <TargetFramework>{AppContext.TargetFrameworkName!}</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <!-- Remove all Compile items from compilation -->
    <ItemGroup>
        <Compile Remove=""*.cs"" />
    </ItemGroup>

    <!-- Add specific files for compilation -->
    <ItemGroup>
        {string.Join(Environment.NewLine, sourceFiles.Select(u => $"<Compile Include=\"{Path.GetFullPath(u)}\" />"))}
    </ItemGroup>

    {packageGroup}
    {projectsGroup}

</Project>";

            // Write and compile

            var path = Path.GetTempFileName();
            File.WriteAllText(path, csproj);

            try
            {
                return CompileProject(path);
            }
            finally { File.Delete(path); }
        }

        public List<CompilationContext> CompileProject(string csproj)
        {
            Compilation ??= GetCompilation(csproj);
            return CompileProjectContracts(Compilation);
        }

        public List<CompilationContext> CompileProject(string csproj, List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols, string? targetContractName = null)
        {
            if (sortedClasses == null || classDependencies == null || allClassSymbols == null)
            {
                throw new InvalidOperationException("Please call PrepareProjectContracts before calling CompileProject with sortedClasses, classDependencies and allClassSymbols parameters.");
            }
            Compilation ??= GetCompilation(csproj);
            return targetContractName == null ? CompileProjectContractsWithPrepare(sortedClasses, classDependencies, allClassSymbols) : [CompileProjectContractWithPrepare(sortedClasses, classDependencies, allClassSymbols, targetContractName)];
        }

        public (List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols) PrepareProjectContracts(string csproj)
        {
            Compilation ??= GetCompilation(csproj);
            var classDependencies = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
            var allSmartContracts = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var allClassSymbols = new List<INamedTypeSymbol?>();
            foreach (var tree in Compilation.SyntaxTrees)
            {
                var semanticModel = Compilation.GetSemanticModel(tree);
                var classNodes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                    allClassSymbols.Add(classSymbol);
                    if (classSymbol is { IsAbstract: false, DeclaredAccessibility: Accessibility.Public } && IsDerivedFromSmartContract(classSymbol, s_pattern))
                    {
                        allSmartContracts.Add(classSymbol);
                        classDependencies[classSymbol] = [];
                        foreach (var member in classSymbol.GetMembers())
                        {
                            var memberTypeSymbol = (member as IFieldSymbol)?.Type ?? (member as IPropertySymbol)?.Type;
                            if (memberTypeSymbol is INamedTypeSymbol namedTypeSymbol && allSmartContracts.Contains(namedTypeSymbol) && !namedTypeSymbol.IsAbstract)
                            {
                                classDependencies[classSymbol].Add(namedTypeSymbol);
                            }
                        }
                    }
                }
            }

            // Verify if there is any valid smart contract class
            if (classDependencies.Count == 0) throw new FormatException("No valid epicchain SmartContract found. Please make sure your contract is subclass of SmartContract and is not abstract.");
            // Check contract dependencies, make sure there is no cycle in the dependency graph
            var sortedClasses = TopologicalSort(classDependencies);

            return (sortedClasses, classDependencies, allClassSymbols);
        }

        private CompilationContext CompileProjectContractWithPrepare(List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols, string targetContractName)
        {
            var c = sortedClasses.FirstOrDefault(p => p.Name.Equals(targetContractName, StringComparison.InvariantCulture))
                ?? throw new ArgumentException($"targetContractName '{targetContractName}' was not found");
            var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
            var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
            var context = new CompilationContext(this, c, classesNotInDependencies!);
            context.Compile();
            return context;
        }

        private List<CompilationContext> CompileProjectContractsWithPrepare(List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols)
        {
            Parallel.ForEach(sortedClasses, c =>
            {
                var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
                var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
                var context = new CompilationContext(this, c, classesNotInDependencies!);
                context.Compile();
                // Process the target contract add this compilation context
                Contexts.TryAdd(c, context);
            });

            return Contexts.Select(p => p.Value).ToList();
        }

        private List<CompilationContext> CompileProjectContracts(Compilation compilation)
        {
            var classDependencies = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
            var allSmartContracts = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var allClassSymbols = new List<INamedTypeSymbol?>();
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var classNodes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                    allClassSymbols.Add(classSymbol);
                    if (classSymbol is { IsAbstract: false, DeclaredAccessibility: Accessibility.Public } && IsDerivedFromSmartContract(classSymbol, s_pattern))
                    {
                        allSmartContracts.Add(classSymbol);
                        classDependencies[classSymbol] = [];
                        foreach (var member in classSymbol.GetMembers())
                        {
                            var memberTypeSymbol = (member as IFieldSymbol)?.Type ?? (member as IPropertySymbol)?.Type;
                            if (memberTypeSymbol is INamedTypeSymbol namedTypeSymbol && allSmartContracts.Contains(namedTypeSymbol) && !namedTypeSymbol.IsAbstract)
                            {
                                classDependencies[classSymbol].Add(namedTypeSymbol);
                            }
                        }
                    }
                }
            }

            // Verify if there is any valid smart contract class
            if (classDependencies.Count == 0) throw new FormatException("No valid EpicChain SmartContract found. Please make sure your contract is subclass of SmartContract and is not abstract.");
            // Check contract dependencies, make sure there is no cycle in the dependency graph
            var sortedClasses = TopologicalSort(classDependencies);

            Parallel.ForEach(sortedClasses, c =>
            {
                var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
                var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
                var context = new CompilationContext(this, c, classesNotInDependencies!);
                context.Compile();
                // Process the target contract add this compilation context
                Contexts.TryAdd(c, context);
            });

            return Contexts.Select(p => p.Value).ToList();
        }

        /// <summary>
        /// Sort the classes based on their topological dependencies
        /// </summary>
        /// <param name="dependencies">Contract dependencies map</param>
        /// <returns>List of sorted contracts</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static List<INamedTypeSymbol> TopologicalSort(Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> dependencies)
        {
            var sorted = new List<INamedTypeSymbol>();
            var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var visiting = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default); // for detecting cycles

            void Visit(INamedTypeSymbol classSymbol)
            {
                if (visited.Contains(classSymbol))
                {
                    return;
                }
                if (!visiting.Add(classSymbol))
                {
                    throw new InvalidOperationException("Cyclic dependency detected");
                }

                if (dependencies.TryGetValue(classSymbol, out var dependency))
                {
                    foreach (var dep in dependency)
                    {
                        Visit(dep);
                    }
                }

                visiting.Remove(classSymbol);
                visited.Add(classSymbol);
                sorted.Add(classSymbol);
            }

            foreach (var classSymbol in dependencies.Keys)
            {
                Visit(classSymbol);
            }

            return sorted;
        }

        static bool IsDerivedFromSmartContract(INamedTypeSymbol classSymbol, Regex pattern)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                if (pattern.IsMatch(baseType.ToString() ?? string.Empty))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        public Compilation GetCompilation(string csproj)
        {
            // Restore project

            string folder = Path.GetDirectoryName(csproj)!;
            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore \"{csproj}\" --source \"https://www.myget.org/F/epicchain/api/v3/index.json\"",
                WorkingDirectory = folder
            })!.WaitForExit();

            // Parse csproj

            XDocument document = XDocument.Load(csproj);
            var remove = document.Root!.Elements("ItemGroup").Elements("Compile").Attributes("Remove")
                .Select(p => p.Value.Contains('*') ? p.Value : Path.GetFullPath(p.Value)).ToArray();
            var sourceFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!remove.Contains("*.cs"))
            {
                var obj = Path.Combine(folder, "obj");
                var binSc = Path.Combine(folder, "bin");
                foreach (var entry in Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                      .Where(p => !p.StartsWith(obj) && !p.StartsWith(binSc))
                      .Select(u => u))
                //.GroupBy(Path.GetFileName)
                //.Select(g => g.First()))
                {
                    if (!remove.Contains(entry)) sourceFiles.Add(entry);
                }
            }

            sourceFiles.UnionWith(document.Root!.Elements("ItemGroup").Elements("Compile").Attributes("Include").Select(p => Path.GetFullPath(p.Value, folder)));
            var assetsPath = Path.Combine(folder, "obj", "project.assets.json");
            var assets = (JObject)JToken.Parse(File.ReadAllBytes(assetsPath))!;
            List<MetadataReference> references = new(CommonReferences);
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable, allowUnsafe: false);
            foreach (var (name, package) in ((JObject)assets["targets"]![0]!).Properties)
            {
                MetadataReference? reference = GetReference(name, (JObject)package!, assets, folder, compilationOptions);
                if (reference is not null) references.Add(reference);
            }
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            return CSharpCompilation.Create(assets["project"]!["restore"]!["projectName"]!.GetString(), syntaxTrees, references, compilationOptions);
        }

        private MetadataReference? GetReference(string name, JObject package, JObject assets, string folder, CSharpCompilationOptions compilationOptions)
        {
            if (!MetaReferences.TryGetValue(name, out var reference))
            {
                switch (assets["libraries"]![name]!["type"]!.GetString())
                {
                    case "package":
                        string packagesPath = assets["project"]!["restore"]!["packagesPath"]!.GetString();
                        string namePath = assets["libraries"]![name]!["path"]!.GetString();
                        string[] files = ((JArray)assets["libraries"]![name]!["files"]!)
                            .Select(p => p!.GetString())
                            .Where(p => p.StartsWith("src/"))
                            .ToArray();
                        if (files.Length == 0)
                        {
                            JObject? dllFiles = (JObject?)(package["compile"] ?? package["runtime"]);
                            if (dllFiles is null) return null;
                            foreach (var (file, _) in dllFiles.Properties)
                            {
                                if (file.EndsWith("_._")) continue;
                                string path = Path.Combine(packagesPath, namePath, file);
                                if (!File.Exists(path)) continue;
                                reference = MetadataReference.CreateFromFile(path);
                                break;
                            }
                            if (reference is null) return null;
                        }
                        else
                        {
                            string assemblyName = Path.GetDirectoryName(name)!;
                            IEnumerable<SyntaxTree> st = files.OrderBy(p => p).Select(p => Path.Combine(packagesPath, namePath, p)).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
                            CSharpCompilation cr = CSharpCompilation.Create(assemblyName, st, CommonReferences, compilationOptions);
                            reference = cr.ToMetadataReference();
                        }
                        break;
                    case "project":
                        string msbuildProject = assets["libraries"]![name]!["msbuildProject"]!.GetString();
                        msbuildProject = Path.GetFullPath(msbuildProject, folder);
                        reference = GetCompilation(msbuildProject).ToMetadataReference();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                MetaReferences.Add(name, reference);
            }
            return reference;
        }
    }
}
