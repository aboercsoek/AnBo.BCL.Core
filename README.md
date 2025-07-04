# AnBo.BCL8.Core - Legacy Base Class Library Migration to .NET 8

## Project Overview

This repository contains the migration of a legacy Base Class Library originally developed for use in customer projects targeting .NET Framework 4.0. The library has served as a foundational utility set in various enterprise applications, offering reusable components and helper functions.

## Migration Goals

The main objective of this project is to modernize the codebase by migrating it to the latest .NET 8 platform. This includes:

- **Eliminating obsolete components**: Removing parts of the library that are no longer needed due to changes in the .NET ecosystem or application architecture.
- **Refactoring and modernizing code**: Updating and improving existing functionality where better solutions or new .NET 8 features are available.
- **Retaining valuable functionality**: Preserving stable and proven components that are still relevant and beneficial.
- **Improving test coverage**: Introducing a modern testing approach with high test coverage to ensure long-term maintainability and reliability. The original version of the library had limited or no unit tests, which will be addressed in this migration.

## Key Changes

- Target framework upgraded from **.NET Framework 4.0** to **.NET 8**.
- Transition to modern C# language features and coding conventions.
- Integration of unit testing using **xUnit** (or another preferred testing framework).
- Modularization of components to improve clarity, reusability, and testability.
- Documentation and comments updated for better developer experience.

## Status

This project is a **work in progress**. Migration is being performed incrementally, module by module. See the [project board](#) or [issues](#) section for detailed progress.

## Contributing

This is currently a personal refactoring and modernization project. Contributions are welcome, especially regarding testing strategies and modern .NET development patterns.

## License

[GNU GENERAL PUBLIC LICENSE v3](LICENSE.txt)
