using CliWrap;

namespace DevantlerTech.SOPSCLI.Tests.SOPSTests;

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
    var (exitCode, output) = await SOPS.RunAsync(["--version"]);

    // Assert
    Assert.Equal(0, exitCode);
    string version = output.Trim().Split(["\r\n", "\r", "\n", " "], StringSplitOptions.RemoveEmptyEntries)[1];
    Assert.Matches(@"^\d+\.\d+\.\d+$", version);
  }
}
