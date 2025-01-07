using CliWrap;

namespace Devantler.SOPSCLI.Tests.SOPSTests;

/// <summary>
/// Tests for the <see cref="SOPS.RunAsync(string[], CommandResultValidation, bool, bool, CancellationToken)" /> method.
/// </summary>
public class RunAsyncTests
{
  /// <summary>
  /// Tests that the binary can return the version of the sops CLI command.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task RunAsync_Version_ReturnsVersion()
  {
    // Act
    var (exitCode, message) = await SOPS.RunAsync(["--version"]);

    // Assert
    Assert.Equal(0, exitCode);
    // "sops 3.7.1 (latest)"
    Assert.Matches(@"^sops \d+\.\d+\.\d+ \(latest\)$", message.Trim());
  }
}
