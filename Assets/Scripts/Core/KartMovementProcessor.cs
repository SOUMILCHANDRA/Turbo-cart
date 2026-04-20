using UnityEngine;

public static class KartMovementProcessor
{
    public static void ProcessMovement(KartController kart)
    {
        float maxSpeed = kart.Settings.maxSpeed;
        float accel = kart.Settings.acceleration;

        if (kart.ExternalBoostTimer > 0)
        {
            maxSpeed *= kart.ExternalBoostMultiplier;
            accel *= kart.ExternalBoostMultiplier;
            kart.ExternalBoostTimer -= Time.fixedDeltaTime;
        }

        if (kart.Rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = kart.transform.forward * kart.InputSource.Vertical * accel;
            kart.Rb.AddForce(force, ForceMode.Acceleration);
        }

        // Auto-braking / Drag
        kart.Rb.drag = (kart.InputSource.Vertical == 0 && kart.Rb.velocity.magnitude > 0.1f) 
            ? kart.Settings.dragBase * 2 
            : kart.Settings.dragBase;

        // Apply Lateral Grip
        float grip = kart.IsDrifting ? kart.Settings.driftGripAmount : kart.Settings.normalGripAmount;
        grip *= (1f + (kart.Rb.velocity.magnitude * kart.Settings.gripSpeedFactor));
        
        Vector3 latVel = Vector3.Project(kart.Rb.velocity, kart.transform.right);
        kart.Rb.AddForce(-latVel * grip, ForceMode.Acceleration);
    }

    public static void ProcessDrift(KartController kart)
    {
        bool wantsToDrift = kart.InputSource.IsDrifting && Mathf.Abs(kart.InputSource.Horizontal) > 0.1f;

        // Enter Drift
        if (!kart.IsDrifting && wantsToDrift)
        {
            kart.IsDrifting = true;
            kart.DriftDirection = Mathf.Sign(kart.InputSource.Horizontal);
            kart.CurrentBoostCharge = 0f;
        }

        // Continue Drift
        if (kart.IsDrifting)
        {
            if (kart.InputSource.IsDrifting)
            {
                kart.CurrentBoostCharge = Mathf.MoveTowards(kart.CurrentBoostCharge, kart.Settings.maxBoost, kart.Settings.boostChargeRate * Time.fixedDeltaTime);
            }
            else
            {
                // Exit Drift & Apply Boost
                if (kart.CurrentBoostCharge > 0.5f)
                {
                    kart.Rb.AddForce(kart.transform.forward * kart.CurrentBoostCharge * kart.Settings.boostForce, ForceMode.VelocityChange);
                }
                kart.IsDrifting = false;
                kart.CurrentBoostCharge = 0f;
            }
        }
    }
}
