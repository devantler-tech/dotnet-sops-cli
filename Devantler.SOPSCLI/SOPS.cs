using System.Runtime.InteropServices;
using CliWrap;
using Devantler.CLIRunner;

namespace Devantler.SOPSCLI;

/// <summary>
/// A class to run sops CLI commands.
/// </summary>
public static class SOPS
{
  static Command Command => GetCommand();

  internal static Command GetCommand(PlatformID? platformID = default, Architecture? architecture = default, string? runtimeIdentifier = default)
  {
    platformID ??= Environment.OSVersion.Platform;
    architecture ??= RuntimeInformation.ProcessArchitecture;
    runtimeIdentifier ??= RuntimeInformation.RuntimeIdentifier;

    string binary = (platformID, architecture, runtimeIdentifier) switch
    {
      (PlatformID.Unix, Architecture.X64, "osx-x64") => "sops-osx-x64",
      (PlatformID.Unix, Architecture.Arm64, "osx-arm64") => "sops-osx-arm64",
      (PlatformID.Unix, Architecture.X64, "linux-x64") => "sops-linux-x64",
      (PlatformID.Unix, Architecture.Arm64, "linux-arm64") => "sops-linux-arm64",
      (PlatformID.Win32NT, Architecture.X64, "win-x64") => "sops-win-x64.exe",
      _ => throw new PlatformNotSupportedException($"Unsupported platform: {Environment.OSVersion.Platform} {RuntimeInformation.ProcessArchitecture}"),
    };
    string binaryPath = Path.Combine(AppContext.BaseDirectory, binary);
    return !File.Exists(binaryPath) ?
      throw new SOPSException($"{binaryPath} not found.") :
      Cli.Wrap(binaryPath);
  }

  /// <summary>
  /// Decrypt a file using SOPS.
  /// </summary>
  /// <param name="filePath">The path to the file to decrypt.</param>
  /// <param name="sopsAgeKeyFilePath">The path to the sops age key file.</param>
  /// <param name="cancellationToken">The cancellation cancellationToken.</param>
  /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
  /// <exception cref="CLIException">Thrown when the CLI command fails.</exception>
  public static async Task DecryptAsync(string filePath, string sopsAgeKeyFilePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
    {
      throw new SOPSException($"File '{filePath}' does not exist");
    }
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", sopsAgeKeyFilePath);
    var cmd = Command.WithArguments($"-d -i {filePath}");
    var (exitCode, result) = await CLI.RunAsync(cmd, silent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    if (exitCode != 0)
    {
      throw new SOPSException($"Failed to decrypt file '{filePath}': {result}");
    }
  }

  /// <summary>
  /// Encrypt a file using SOPS.
  /// </summary>
  /// <param name="filePath">The path to the file to encrypt.</param>
  /// <param name="sopsAgeKeyFilePath">The path to the sops age key file.</param>
  /// <param name="cancellationToken">The cancellation cancellationToken.</param>
  /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
  /// <exception cref="CLIException">Thrown when the CLI command fails.</exception>
  /// <returns>An integer representing the exit code.</returns>
  public static async Task EncryptAsync(string filePath, string sopsAgeKeyFilePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
    {
      throw new SOPSException($"File '{filePath}' does not exist");
    }
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", sopsAgeKeyFilePath);
    var cmd = Command.WithArguments($"-e -i {filePath}");
    var (exitCode, result) = await CLI.RunAsync(cmd, silent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    if (exitCode != 0)
    {
      throw new SOPSException($"Failed to encrypt file '{filePath}': {result}");
    }
  }

  /// <summary>
  /// Edit a file using SOPS.
  /// </summary>
  /// <param name="filePath"></param>
  /// <param name="sopsAgeKeyFilePath"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="SOPSException"></exception>
  public static async Task EditAsync(string filePath, string sopsAgeKeyFilePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
    {
      throw new SOPSException($"File '{filePath}' does not exist");
    }
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", sopsAgeKeyFilePath);
    var cmd = Command.WithArguments($"edit {filePath}");
    var (exitCode, result) = await CLI.RunAsync(cmd, silent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    if (exitCode != 0)
    {
      throw new SOPSException($"Failed to edit file '{filePath}': {result}");
    }
  }

  /// <summary>
  /// Generate a new data encryption key and reencrypt all values with the new key
  /// </summary>
  public static async Task RotateAsync(string filePath, string sopsAgeKeyFilePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
    {
      throw new SOPSException($"File '{filePath}' does not exist");
    }
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", sopsAgeKeyFilePath);
    var cmd = Command.WithArguments($"-r -i {filePath}");
    var (exitCode, result) = await CLI.RunAsync(cmd, silent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    if (exitCode != 0)
    {
      throw new SOPSException($"Failed to rotate file '{filePath}': {result}");
    }
  }

  /// <summary>
  /// Update the keys of SOPS files using the config file
  /// </summary>
  public static async Task UpdateKeysAsync(string filePath, string sopsAgeKeyFilePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
    {
      throw new SOPSException($"File '{filePath}' does not exist");
    }
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", sopsAgeKeyFilePath);
    var cmd = Command.WithArguments($"updatekeys {filePath}");
    var (exitCode, result) = await CLI.RunAsync(cmd, silent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
    Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    if (exitCode != 0)
    {
      throw new SOPSException($"Failed to rotate file '{filePath}': {result}");
    }
  }
}
