# 🔐 .NET SOPS CLI

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Test](https://github.com/devantler-tech/dotnet-sops-cli/actions/workflows/test.yaml/badge.svg)](https://github.com/devantler-tech/dotnet-sops-cli/actions/workflows/test.yaml)
[![codecov](https://codecov.io/gh/devantler-tech/dotnet-sops-cli/graph/badge.svg?token=RhQPb4fE7z)](https://codecov.io/gh/devantler-tech/dotnet-sops-cli)

A simple .NET library that embeds the SOPS CLI.

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 or later
- [SOPS CLI](https://github.com/getsops/sops/releases) installed and available in your system's PATH

### Installation

To get started, you can install the package from NuGet.

```bash
dotnet add package DevantlerTech.SOPSCLI
```

## 📝 Usage

You can execute the SOPS CLI commands using the `SOPS` class.

```csharp
using DevantlerTech.SOPSCLI;

var (exitCode, output) = await SOPS.RunAsync(["arg1", "arg2"]);
```
