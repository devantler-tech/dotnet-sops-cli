using Devantler.CLIRunner;

namespace Devantler.SOPSCLI;

/// <summary>
/// An exception thrown by the SOPS library.
/// </summary>
public class SOPSException : CLIException
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public SOPSException()
  {
  }

  /// <summary>
  /// Constructor with message.
  /// </summary>
  /// <param name="message"></param>
  public SOPSException(string message) : base(message)
  {
  }

  /// <summary>
  /// Constructor with message and inner exception.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="innerException"></param>
  public SOPSException(string message, Exception innerException) : base(message, innerException)
  {
  }

}
