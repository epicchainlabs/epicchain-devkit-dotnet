# EpicChain Smart Contract Roslyn Analyzers

This repository contains a comprehensive set of Roslyn analyzers and code fix providers specifically designed for EpicChain smart contracts. These tools are intended to help developers adhere to best practices, prevent the use of unsupported features, and maintain high standards of quality and security when writing smart contracts for the EpicChain ecosystem.

## Overview of the Analyzers

The following is a detailed breakdown of the analyzers included in the repository, highlighting their purpose and functionality:

### EpicChainContractAnalyzer

- **[FloatUsageAnalyzer.cs](EpicChainContractAnalyzer/FloatUsageAnalyzer.cs)**: This analyzer checks for the usage of the `float` data type. Since floating-point arithmetic is generally unsupported in blockchain environments, this tool ensures that developers do not introduce `float` types in EpicChain smart contracts.

- **[DecimalUsageAnalyzer.cs](EpicChainContractAnalyzer/DecimalUsageAnalyzer.cs)**: Detects the usage of the `decimal` data type, which is not permitted in EpicChain smart contracts due to its precision-related issues in decentralized applications.

- **[DoubleUsageAnalyzer.cs](EpicChainContractAnalyzer/DoubleUsageAnalyzer.cs)**: Identifies the usage of the `double` data type, another unsupported floating-point type that could lead to errors in smart contract execution.

- **[SystemMathUsageAnalyzer.cs](EpicChainContractAnalyzer/SystemMathUsageAnalyzer.cs)**: Flags any attempt to use certain methods from the `System.Math` library that are incompatible with the limitations of EpicChain smart contracts.

- **[BigIntegerUsageAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerUsageAnalyzer.cs)**: Checks for unsupported methods from the `BigInteger` class, which is often used for handling large numbers in blockchain systems. This analyzer ensures only permissible `BigInteger` methods are utilized.

- **[StringMethodUsageAnalyzer.cs](EpicChainContractAnalyzer/StringMethodUsageAnalyzer.cs)**: Reports the use of specific methods from the `string` class that are not supported in EpicChain smart contracts, helping developers avoid potential issues with string manipulation.

- **[BigIntegerCreationAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerCreationAnalyzer.cs)**: Examines how instances of the `BigInteger` class are created and flags unsupported creation patterns, ensuring that developers use best practices for managing large integers.

- **[InitialValueAnalyzer.cs](EpicChainContractAnalyzer/InitialValueAnalyzer.cs)**: Suggests converting attribute-based initializations to literal initializations for certain types, a practice that can optimize performance in EpicChain smart contracts.

- **[RefKeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/RefKeywordUsageAnalyzer.cs)**: Warns about the usage of the `ref` keyword in smart contracts, which may introduce unpredictability and is generally not supported.

- **[LinqUsageAnalyzer.cs](EpicChainContractAnalyzer/LinqUsageAnalyzer.cs)**: Detects the usage of LINQ methods in smart contracts, which can introduce performance overhead and are typically not supported in the blockchain environment.

- **[CharMethodsUsageAnalyzer.cs](EpicChainContractAnalyzer/CharMethodsUsageAnalyzer.cs)**: Flags certain methods of the `char` class that are either inefficient or unsupported in the context of smart contracts, helping developers avoid potential pitfalls with character data types.

- **[CollectionTypesUsageAnalyzer.cs](EpicChainContractAnalyzer/CollectionTypesUsageAnalyzer.cs)**: Identifies the use of unsupported collection types such as `List` and `Dictionary`. These data structures can introduce inefficiencies or non-determinism in smart contract logic, so this analyzer promotes safer alternatives.

- **[VolatileKeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/VolatileKeywordUsageAnalyzer.cs)**: Issues warnings for the use of the `volatile` keyword, which is generally not suitable for blockchain smart contracts due to its interaction with concurrent programming constructs.

- **[KeywordUsageAnalyzer.cs](EpicChainContractAnalyzer/KeywordUsageAnalyzer.cs)**: Detects the usage of restricted or unsupported keywords in smart contracts. This analyzer helps ensure that contracts remain compatible with the EpicChain virtual machine.

- **[BanCastMethodAnalyzer.cs](EpicChainContractAnalyzer/BanCastMethodAnalyzer.cs)**: Flags unsupported cast methods, helping developers avoid runtime issues caused by inappropriate type conversions.

- **[SmartContractMethodNamingAnalyzer.cs](EpicChainContractAnalyzer/SmartContractMethodNamingAnalyzer.cs)**: Ensures that all smart contract methods follow the correct naming conventions, improving code readability and maintainability.

- **[NotifyEventNameAnalyzer.cs](EpicChainContractAnalyzer/NotifyEventNameAnalyzer.cs)**: This analyzer ensures the proper naming and usage of event names in notify calls within smart contracts, which is essential for maintaining consistency and clarity in event-driven logic.

- **[SmartContractMethodNamingAnalyzerUnderline.cs](EpicChainContractAnalyzer/SmartContractMethodNamingAnalyzerUnderline.cs)**: Warns developers if smart contract method names contain underscores, enforcing naming standards that align with best practices for smart contract development.

- **[SupportedStandardsAnalyzer.cs](EpicChainContractAnalyzer/SupportedStandardsAnalyzer.cs)**: Checks the contract for correct implementation of supported EpicChain standards, ensuring compatibility and interoperability with the broader EpicChain ecosystem.

- **[BigIntegerUsingUsageAnalyzer.cs](EpicChainContractAnalyzer/BigIntegerUsingUsageAnalyzer.cs)**: Identifies incorrect usage of `BigInteger` within `using` statements, guiding developers to safer and more efficient patterns for handling large numeric values.

- **[StaticFieldInitializationAnalyzer.cs](EpicChainContractAnalyzer/StaticFieldInitializationAnalyzer.cs)**: Analyzes the initialization of static fields to ensure that they are properly set up in smart contracts, avoiding common errors related to static data handling.

- **[MultipleCatchBlockAnalyzer.cs](EpicChainContractAnalyzer/MultipleCatchBlockAnalyzer.cs)**: Checks for the use of multiple `catch` blocks in `try` statements. This analyzer ensures that exception handling logic is simplified and error-prone patterns are avoided.

- **[SystemDiagnosticsUsageAnalyzer.cs](EpicChainContractAnalyzer/SystemDiagnosticsUsageAnalyzer.cs)**: Detects the usage of the `System.Diagnostics` namespace, which is not supported in EpicChain smart contracts, helping developers avoid introducing debug-related methods that are incompatible with production environments.

## How to Use These Analyzers

To incorporate these analyzers into your EpicChain smart contract project:

1. **Add a Reference**: Include a reference to the `EpicChain.SmartContract.Analyzer` project in your smart contract project.
2. **Build the Analyzer Project**: Once the project is built, the analyzers will automatically run during the build process of your smart contract.
3. **Analyze and Fix**: Review any issues flagged by the analyzers and apply the necessary code fixes to ensure that your smart contract complies with EpicChain standards.

## Contributing to This Repository

Contributions are highly encouraged! Whether you're interested in improving existing analyzers, fixing bugs, or adding new features, you're welcome to submit a pull request. Please ensure that your contributions follow the project's guidelines and are well-documented.

## License Information

This project is licensed under the MIT License. For more details, refer to the [LICENSE](LICENSE) file included in the repository.
