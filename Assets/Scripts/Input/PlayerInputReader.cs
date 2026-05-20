using UnityEngine;

/// <summary>
/// Reads local player inputs via Unity's legacy Input Manager.
/// Implements IKartInput interface.
/// </summary>
public class PlayerInputReader : MonoBehaviour, IKartInput
{
    [Header("Axis Setup")]
    [SerializeField] private string throttleAxis = "Vertical";
    [SerializeField] private string steeringAxis = "Horizontal";
    [SerializeField] private KeyCode driftKey = KeyCode.LeftShift;

    /// <summary>
    /// Vertical axis input (Throttle/Brake).
    /// </summary>
    public float Throttle => Input.GetAxis(throttleAxis);

    /// <summary>
    /// Horizontal axis input (Steering).
    /// </summary>
    public float Steering => Input.GetAxis(steeringAxis);

    /// <summary>
    /// Active state of drift command key (Shift).
    /// </summary>
    public bool IsDrifting => Input.GetKey(driftKey);
}
