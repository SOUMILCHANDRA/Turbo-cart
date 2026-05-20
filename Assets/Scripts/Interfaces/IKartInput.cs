/// <summary>
/// Interface decoupling input gathering from movement mechanics.
/// Enables human player input, AI driving agents, and network replication.
/// </summary>
public interface IKartInput
{
    /// <summary>
    /// Value between -1.0 (reverse/brake) and 1.0 (accelerate).
    /// </summary>
    float Throttle { get; }

    /// <summary>
    /// Value between -1.0 (left steer) and 1.0 (right steer).
    /// </summary>
    float Steering { get; }

    /// <summary>
    /// State indicating if the kart is attempting to drift (e.g. holding drift button).
    /// </summary>
    bool IsDrifting { get; }
}
