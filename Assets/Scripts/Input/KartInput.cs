using UnityEngine;

public interface IKartInput
{
    float Horizontal { get; }
    float Vertical { get; }
    bool IsDrifting { get; }
}

public class KeyboardKartInput : MonoBehaviour, IKartInput
{
    public float Horizontal => Input.GetAxis("Horizontal");
    public float Vertical => Input.GetAxis("Vertical");
    public bool IsDrifting => Input.GetKey(KeyCode.LeftShift);
}
