using Devantler.KeyManager.Core.Models;
using Devantler.KeyManager.Local.Age;
using Devantler.KubernetesGenerator.Native.ConfigAndStorage;
using k8s.Models;

namespace Devantler.SOPSCLI.Tests.SOPSTests;

/// <summary>
/// Tests for the <see cref="SOPS.EncryptAsync(string, string, CancellationToken)"/> and <see cref="SOPS.DecryptAsync(string, string, CancellationToken)"/> methods.
/// </summary>
public class EncryptAndDecryptAsyncTests
{
  readonly LocalAgeKeyManager _keyManager = new();
  readonly SecretGenerator _secretGenerator = new();
  /// <summary>
  /// Test to verify a *.sops.yaml file can be encrypted and decrypted.
  /// </summary>
  [Fact]
  public async Task EncryptAndDecrypt_ShouldEncryptAndDecryptFile()
  {
    // Arrange
    string tempDir = Path.Combine(Path.GetTempPath(), "sops-cli-tests");
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
    await SOPS.DecryptAsync(tempDir + "/test-secret.sops.yaml", tempDir + "/sops-test-key.txt");
    string decryptedFile = File.ReadAllText(tempDir + "/test-secret.sops.yaml");
    // Add --- to the beginning of the file to make it a valid yaml file
    decryptedFile = "---" + Environment.NewLine + decryptedFile;
    // Make decrypted file use two spaces for indentation
    decryptedFile = decryptedFile.Replace("    ", "  ", StringComparison.OrdinalIgnoreCase);

    // Assert
    Assert.NotEqual(originalFile, encryptedFile);
    Assert.Equal(originalFile, decryptedFile);

    // Cleanup
    Directory.Delete(tempDir, true);
    File.Delete(".sops.yaml");
  }
}
