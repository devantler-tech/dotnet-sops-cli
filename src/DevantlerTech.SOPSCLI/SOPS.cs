using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;

namespace DevantlerTech.SOPSCLI;

/// <summary>
/// A class to run sops CLI commands.
/// </summary>
public static class SOPS
{
  public static Command GetCommand()
  {
    string binaryName = "sops";
    string? pathEnv = Environment.GetEnvironmentVariable("PATH");

    if (!string.IsNullOrEmpty(pathEnv))
    {
      string[] paths = pathEnv.Split(Path.PathSeparator);
      foreach (string dir in paths)
      {
        string fullPath = Path.Combine(dir, binaryName);
        if (File.Exists(fullPath))
        {
          return Cli.Wrap(fullPath);
        }
      }
    }

    throw new FileNotFoundException($"The '{binaryName}' CLI was not found in PATH.");
  }

  /// <summary>
  /// Runs the sops CLI command with the given arguments.
  /// </summary>
  /// <param name="arguments"></param>
  /// <param name="validation"></param>
  /// <param name="input"></param>
  /// <param name="silent"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async Task<(int ExitCode, string Message)> RunAsync(
    string[] arguments,
    CommandResultValidation validation = CommandResultValidation.ZeroExitCode,
    bool input = false,
    bool silent = false,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));
    if (arguments[0] == "edit")
    {
      var process = new ProcessStartInfo
      {
        FileName = Command.TargetFilePath,
        Arguments = string.Join(' ', arguments),
        UseShellExecute = true,
        CreateNoWindow = true,
      };
      using var proc = Process.Start(process) ?? throw new InvalidOperationException("Failed to start sops process.");
      await proc.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
      return (proc.ExitCode, string.Empty);
    }
    else
    {
      using var stdInConsole = input ? Stream.Null : Console.OpenStandardInput();
      using var stdOutConsole = silent ? Stream.Null : Console.OpenStandardOutput();
      using var stdErrConsole = silent ? Stream.Null : Console.OpenStandardError();
      var command = GetCommand().WithArguments(arguments)
        .WithValidation(validation)
        .WithStandardInputPipe(PipeSource.FromStream(stdInConsole))
        .WithStandardOutputPipe(PipeTarget.ToStream(stdOutConsole))
        .WithStandardErrorPipe(PipeTarget.ToStream(stdErrConsole));
      var result = await command.ExecuteBufferedAsync(cancellationToken);
      return (result.ExitCode, result.StandardOutput.ToString() + result.StandardError.ToString());
    }
  }
}
