using UnityEngine;

/// <summary>
/// Configuration settings for the Anti-Gravity Kart System.
/// Allows dynamic tuning of karts in the editor.
/// </summary>
[CreateAssetMenu(fileName = "KartSettings", menuName = "TurboKart/Settings")]
public class KartSettings : ScriptableObject
{
    [Header("Movement Settings")]
    [Tooltip("Forward force applied to the kart.")]
    public float acceleration = 60f;
    [Tooltip("Maximum velocity magnitude allowed under standard throttle.")]
    public float maxSpeed = 120f;
    [Tooltip("Braking deceleration force.")]
    public float brakingForce = 40f;
    [Tooltip("Base linear drag of the Rigidbody.")]
    public float baseDrag = 0.6f;

    [Header("Steering Settings")]
    [Tooltip("Multiplier for turning yaw velocity.")]
    public float steeringSensitivity = 4f;
    [Tooltip("Force countering lateral drift to stick the kart to its trajectory.")]
    public float gripAmount = 12f;
    [Tooltip("Factor by which lateral grip scales with speed.")]
    public float speedGripFactor = 0.08f;

    [Header("Anti-Gravity Settings")]
    [Tooltip("Target height the suspension tries to maintain above track surface.")]
    public float hoverHeight = 1.2f;
    [Tooltip("Spring stiffness coefficient for the hover suspension.")]
    public float hoverSnappiness = 25f;
    [Tooltip("Damping factor for the hover suspension to prevent oscillation.")]
    public float dampening = 2f;
    [Tooltip("Base gravity force keeping the kart attached to surfaces.")]
    public float baseGravity = 35f;
    [Tooltip("How much gravity increases with velocity (magnetic adhesion).")]
    public float speedAdhesionFactor = 1.2f;
    [Tooltip("Maximum length of surface-finding raycasts.")]
    public float maxRaycastDistance = 5f;
    [Tooltip("Interpolation speed for aligning the kart's orientation to surface normals.")]
    public float rotationSmoothness = 12f;
    [Tooltip("Layers recognized as valid tracks for anti-gravity.")]
    public LayerMask trackLayer;

    [Header("Drift & Boost Settings")]
    [Tooltip("Reduced grip amount applied when the kart is drifting/sliding.")]
    public float driftGripAmount = 2.5f;
    [Tooltip("Multiplier for steering sensitivity when initiating a drift.")]
    public float driftSteeringMultiplier = 1.8f;
    [Tooltip("Minimum velocity required to start a drift.")]
    public float minSpeedToDrift = 15f;
    [Tooltip("Rate at which drift boost charges per second.")]
    public float boostChargeRate = 0.6f;
    [Tooltip("Force multiplier applied in direction of travel upon releasing drift.")]
    public float boostForce = 120f;
    [Tooltip("Cap on the accumulated drift boost capacity.")]
    public float maxBoostCharge = 2.5f;
}
