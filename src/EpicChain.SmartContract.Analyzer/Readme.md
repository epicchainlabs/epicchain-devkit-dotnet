# Next Generation Ecosystem Roslyn Analyzers

This repository contains a set of Roslyn analyzers and code fix providers for Next Generation Ecosystems.

## Content
### EpicChainContractAnalyzer

- [FloatUsageAnalyzer.cs](EpicChainContractAnalyzer/FloatUsageAnalyzer.cs): This analyzer checks for usage of float type, which is not supported in Next Generation Ecosystems.
- [DecimalUsageAnalyzer.cs](EpicChainContractAnalyzer/DecimalUsageAnalyzer.cs): This analyzer detects usage of decimal type, which is not supported in Next Generation Ecosystems.
- [DoubleUsageAnalyzer.cs](EpicChainContractAnalyzer/DoubleUsageAnalyzer.cs): This analyzer identifies usage of double type, which is not supported in Next Generation Ecosystems.
- [SystemMathUsageAnalyzer.cs](EpicChainContractAnalyzer/SystemMathUsageAnalyzer.cs): This analyzer flags usage of certain System.Math methods that are not supported in Next Generation Ecosystems.
- [BigIntegerUsageAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerUsageAnalyzer.cs): This analyzer checks for specific methods of the BigInteger class that are not supported.
- [StringMethodUsageAnalyzer.cs](EpicChainContractAnalyzer/StringMethodUsageAnalyzer.cs): This analyzer identifies and reports specific methods of the string class that are not supported.
- [BigIntegerCreationAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerCreationAnalyzer.cs): This analyzer checks for creation patterns of the BigInteger class that are not supported.
- [InitialValueAnalyzer.cs](EpicChainContractAnalyzer/InitialValueAnalyzer.cs): This analyzer suggests converting attribute-based initializations to literal initializations for certain types.
- [RefKeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/RefKeywordUsageAnalyzer.cs): This analyzer flags the usage of the 'ref' keyword, which might not be supported in smart contracts.
- [LinqUsageAnalyzer.cs](EpicChainContractAnalyzer/LinqUsageAnalyzer.cs): This analyzer detects usage of LINQ methods which are not supported in smart contracts.
- [CharMethodsUsageAnalyzer.cs](EpicChainContractAnalyzer/CharMethodsUsageAnalyzer.cs): This analyzer reports specific methods of the char class that are not recommended for use in smart contracts.
- [CollectionTypesUsageAnalyzer.cs](EpicChainContractAnalyzer/CollectionTypesUsageAnalyzer.cs): This analyzer flags the usage of unsupported collection types like List and Dictionary.
- [VolatileKeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/VolatileKeywordUsageAnalyzer.cs): This analyzer warns about the usage of the 'volatile' keyword, which might not be supported in smart contracts.
- [KeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/KeywordUsageAnalyzer.cs): This analyzer detects usage of restricted keywords in smart contracts.
- [BanCastMethodAnalyzer.cs](EpicChainContractAnalyzer/BanCastMethodAnalyzer.cs): This analyzer flags usage of certain cast methods that are not supported in smart contracts.
- [SmartContractMethodNamingAnalyzer.cs](EpicChainContractAnalyzer/SmartContractMethodNamingAnalyzer.cs): This analyzer ensures smart contract methods follow the correct naming convention.
- [NotifyEventNameAnalyzer.cs](EpicChainContractAnalyzer/NotifyEventNameAnalyzer.cs): This analyzer checks for correct usage of event names in notify calls.
- [SmartContractMethodNamingAnalyzerUnderline.cs](EpicChainContractAnalyzer/SmartContractMethodNamingAnalyzerUnderline.cs): This analyzer warns about smart contract method names containing underscores.
- [SupportedStandardsAnalyzer.cs](EpicChainContractAnalyzer/SupportedStandardsAnalyzer.cs): This analyzer checks for correct implementation of supported standards in smart contracts.
- [BigIntegerUsingUsageAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerUsingUsageAnalyzer.cs): This analyzer warns about incorrect usage of BigInteger in using statements.
- [StaticFieldInitializationAnalyzer.cs](EpicChainContractAnalyzer/StaticFieldInitializationAnalyzer.cs): This analyzer checks for proper initialization of static fields in smart contracts.
- [MultipleCatchBlockAnalyzer.cs](EpicChainContractAnalyzer/MultipleCatchBlockAnalyzer.cs): This analyzer checks for multiple catch blocks in try statements.
- [SystemDiagnosticsUsageAnalyzer.cs](EpicChainContractAnalyzer/SystemDiagnosticsUsageAnalyzer.cs): This analyzer detects and reports usage of System.Diagnostics namespace, which is not supported in Next Generation Ecosystems.

## How to Use

To use these analyzers in your Next Generation Ecosystem project:

1. Add a reference to the EpicChain.SmartContract.Analyzer project in your smart contract project.
2. Build the EpicChain.SmartContract.Analyzer project.
3. The analyzers will automatically run when you build your smart contract project.

## Contributing

Contributions to improve existing analyzers or add new ones are welcome. Please submit a pull request with your changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
