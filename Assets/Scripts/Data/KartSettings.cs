using UnityEngine;

[CreateAssetMenu(fileName = "NewKartSettings", menuName = "TurboKart/Settings")]
public class KartSettings : ScriptableObject
{
    [Header("Movement")]
    public float acceleration = 50f;
    public float maxSpeed = 100f;
    public float steeringSensitivity = 5f;
    public float brakingForce = 30f;
    public float dragBase = 0.5f;

    [Header("Anti-Gravity")]
    public float baseGravity = 30f;
    public float hoverHeight = 1.0f;
    public float hoverSnappiness = 20f;
    public float groundCheckDistance = 3.0f;
    public float rotationSmoothness = 10f;
    public LayerMask trackLayer;

    [Header("Adhesion & Grip")]
    public float adhesionStrength = 1.5f;
    public float gripSpeedFactor = 0.05f;
    public float maxRaycastDistance = 5.0f;
    public float normalGripAmount = 10.0f;

    [Header("Drift & Boost")]
    public float driftTurningMultiplier = 2.0f;
    public float driftGripAmount = 2.0f;
    public float boostChargeRate = 0.5f;
    public float maxBoost = 2.0f;
    public float boostForce = 150f;
}
