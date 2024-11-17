using Devantler.KeyManager.Core.Models;
using Devantler.KeyManager.Local.Age;
using Devantler.KubernetesGenerator.Native.ConfigAndStorage;
using k8s.Models;

namespace Devantler.SOPSCLI.Tests.SOPSTests;

/// <summary>
/// Tests for the <see cref="SOPS.RotateAsync(string, string, CancellationToken)"/> and <see cref="SOPS.UpdateKeysAsync(string, string, CancellationToken)"/> methods.
/// </summary>
[Collection("SOPS")]
public class RotateAndUpdateKeysAsyncTests
{
  readonly LocalAgeKeyManager _keyManager = new();
  readonly SecretGenerator _secretGenerator = new();

  /// <summary>
  /// Test to verify a *.sops.yaml file can be rotated and updated.
  /// </summary>
  [Fact]
  public async Task RotateAndUpdateKeys_ShouldRotateAndUpdateKeysFile()
  {
    // Arrange
    string tempDir = Path.Combine(Path.GetTempPath(), "sops-cli-tests-rotate-and-update");
    if (Directory.Exists(tempDir))
    {
      Directory.Delete(tempDir, true);
    }
    var ageKey = await _keyManager.CreateKeyAsync(tempDir + "/sops-test-key.txt");
    var sopsConfig = new SOPSConfig
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          Age = ageKey.PublicKey,
          EncryptedRegex = "^(data | stringData)$",
          PathRegex = ".*"
        }
      ]
    };
    await _keyManager.CreateSOPSConfigAsync(".sops.yaml", sopsConfig, overwrite: true);
    await _secretGenerator.GenerateAsync(new V1Secret
    {
      Metadata = new V1ObjectMeta
      {
        Name = "test-secret"
      },
      StringData = new Dictionary<string, string>
      {
        ["data"] = "test-data"
      }
    }, tempDir + "/test-secret.sops.yaml");

    // Act
    string originalFile = File.ReadAllText(tempDir + "/test-secret.sops.yaml");
    await SOPS.EncryptAsync(tempDir + "/test-secret.sops.yaml", tempDir + "/sops-test-key.txt");
    string encryptedFile = File.ReadAllText(tempDir + "/test-secret.sops.yaml");
    await SOPS.RotateAsync(tempDir + "/test-secret.sops.yaml", tempDir + "/sops-test-key.txt");
    string rotatedFile = File.ReadAllText(tempDir + "/test-secret.sops.yaml");
    await SOPS.UpdateKeysAsync(tempDir + "/test-secret.sops.yaml", tempDir + "/sops-test-key.txt");

    // Assert
    Assert.NotEqual(originalFile, encryptedFile);
    Assert.NotEqual(originalFile, rotatedFile);
    Assert.NotEqual(encryptedFile, rotatedFile);

    // Cleanup
    Directory.Delete(tempDir, true);
    File.Delete(".sops.yaml");
  }
}
