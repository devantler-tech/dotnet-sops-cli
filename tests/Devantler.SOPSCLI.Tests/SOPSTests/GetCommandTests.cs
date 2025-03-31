using System.Runtime.InteropServices;

namespace Devantler.SOPSCLI.Tests.SOPSTests;

/// <summary>
/// Tests for the <see cref="SOPS.GetCommand(PlatformID?, Architecture?, string?)"/> method.
/// </summary>
public class GetCommandTests
{
  /// <summary>
  /// Test to verify that the command returns the correct binary for MacOS on x64 architecture.
  /// </summary>
  [Theory]
  [InlineData(PlatformID.Unix, Architecture.X64, "osx-x64", "sops-osx-x64")]
  [InlineData(PlatformID.Unix, Architecture.Arm64, "osx-arm64", "sops-osx-arm64")]
  [InlineData(PlatformID.Unix, Architecture.X64, "linux-x64", "sops-linux-x64")]
  [InlineData(PlatformID.Unix, Architecture.Arm64, "linux-arm64", "sops-linux-arm64")]
  [InlineData(PlatformID.Win32NT, Architecture.X64, "win-x64", "sops-win-x64.exe")]
  [InlineData(PlatformID.Win32NT, Architecture.Arm64, "win-arm64", "sops-win-arm64.exe")]
  public void GetCommand_ShouldReturnOSXx64Binary(PlatformID platformID, Architecture architecture, string runtimeIdentifier, string expectedBinary)
  {
    // Act
    string actualBinary = Path.GetFileName(SOPS.GetCommand(platformID, architecture, runtimeIdentifier).TargetFilePath);

    // Assert
    Assert.Equal(expectedBinary, actualBinary);
  }

  /// <summary>
  /// Test to verify that the command returns a <see cref="PlatformNotSupportedException"/> when the platform is not supported.
  /// </summary>
  [Fact]
  public void GetCommand_GivenInvaldiPlatform_ShouldThrowPlatformNotSupportedException()
  {
    // Arrange
    var platformID = PlatformID.Other;
    var architecture = Architecture.Wasm;
    string runtimeIdentifier = "wasm";

    // Act
    void Act() => SOPS.GetCommand(platformID, architecture, runtimeIdentifier);

    // Assert
    _ = Assert.Throws<PlatformNotSupportedException>(Act);
  }
}
