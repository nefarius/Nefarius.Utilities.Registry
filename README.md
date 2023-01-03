<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.Registry

> *This is a fork of the fantastic [Registry Export File (.reg) Parser](https://www.codeproject.com/Tips/125573/Registry-Export-File-reg-Parser) project by Henryk Filipowicz.*

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/nefarius/Nefarius.Utilities.Registry/dotnet.yml) ![Requirements](https://img.shields.io/badge/Requires-.NET%207-blue.svg)

A fast parser for Registry Export (.reg) Files.

## Changes of this fork

- Modernized code and style to latest C# language version
- Settled on .NET 7 to benefit from [.NET regular expression source generators](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators)
- `RegFile` class can now parse from streams in addition to file paths
- Added `RegValueType` for `REG_DWORD`, `REG_SZ`, ... as a [SmartEnum](https://github.com/ardalis/SmartEnum)
- Migrated strings to better suited types where applicable
- Migrated string processing to using [Spans](https://learn.microsoft.com/en-us/dotnet/api/system.span-1?view=net-7.0) where applicable
- Greatly improved parsing speed and reduced memory footprint
- Added type-specific `RegValue` classes with pre-parsed `Value` property in the expected managed type (`string`, `byte[]`, etc.)

## 3rd party credits

- [Registry Export File (.reg) Parser](https://www.codeproject.com/Tips/125573/Registry-Export-File-reg-Parser)
- [SmartEnum](https://github.com/ardalis/SmartEnum)
- [.NET regular expression source generators](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators)
