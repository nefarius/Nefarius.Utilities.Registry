# Nefarius.Utilities.Registry

> *This is a fork of the fantastic [Registry Export File (.reg) Parser](https://www.codeproject.com/Tips/125573/Registry-Export-File-reg-Parser) project by Henryk Filipowicz and contributors.*

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/nefarius/Nefarius.Utilities.Registry/dotnet.yml) ![Requirements](https://img.shields.io/badge/Requires-.NET%20Standard%202.1-blue.svg)

Work in progress, use with care 🔥

## Changes of this fork

- Modernized code and style to latest C# language version
- Settled on .NET Standard 2.0 minimum compatibility
- `RegFile` class can now parse from streams in addition to file paths
- Added `RegValueType` for `REG_DWORD`, `REG_SZ`, ... as a [SmartEnum](https://github.com/ardalis/SmartEnum)
- Migrated strings to better suited types where applicable
- Migrated string processing to using Spans where applicable
- Greatly improved parsing speed and reduced memory footprint

## 3rd party credits

- [Registry Export File (.reg) Parser](https://www.codeproject.com/Tips/125573/Registry-Export-File-reg-Parser)
- [SmartEnum](https://github.com/ardalis/SmartEnum)
