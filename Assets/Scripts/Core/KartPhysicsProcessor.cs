using UnityEngine;

public static class KartPhysicsProcessor
{
    public static void ProcessSurfaceDetection(KartController kart)
    {
        Vector3 avgNormal = Vector3.zero;
        int hitCount = 0;
        float totalDist = 0;

        foreach (Transform p in kart.GroundPoints)
        {
            if (Physics.Raycast(p.position, -kart.transform.up, out var hit, kart.Settings.maxRaycastDistance, kart.Settings.trackLayer))
            {
                avgNormal += hit.normal;
                totalDist += hit.distance;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            kart.IsGrounded = true;
            kart.SurfaceNormal = (avgNormal / hitCount).normalized;
            
            float avgDist = totalDist / hitCount;
            float speedMult = 1f + (kart.Rb.velocity.magnitude * 0.02f);
            float lift = (kart.Settings.hoverHeight - avgDist) * kart.Settings.hoverSnappiness * speedMult;
            
            float vertVel = Vector3.Dot(kart.Rb.velocity, kart.transform.up);
            lift -= vertVel * 2f;

            kart.Rb.AddForce(kart.transform.up * lift, ForceMode.Acceleration);
        }
        else
        {
            kart.IsGrounded = false;
            kart.SurfaceNormal = Vector3.Lerp(kart.SurfaceNormal, Vector3.up, Time.fixedDeltaTime);
        }
    }

    public static void ApplyAntiGravity(KartController kart)
    {
        float adhesion = kart.Settings.baseGravity + (kart.Rb.velocity.magnitude * kart.Settings.adhesionStrength);
        kart.Rb.AddForce(-kart.SurfaceNormal * adhesion, ForceMode.Acceleration);
    }

    public static void UpdateOrientation(KartController kart)
    {
        float speedFactor = Mathf.Clamp01(kart.Rb.velocity.magnitude / 10f);
        float multiplier = kart.IsDrifting ? kart.Settings.driftTurningMultiplier : 1.0f;
        float hInput = kart.IsDrifting ? kart.DriftDirection : kart.InputSource.Horizontal;
        
        float turnAmount = hInput * kart.Settings.steeringSensitivity * speedFactor * multiplier;
        Quaternion turnRot = Quaternion.Euler(0f, turnAmount, 0f);

        Vector3 fwd = kart.transform.rotation * turnRot * Vector3.forward;
        Vector3 projectedFwd = Vector3.ProjectOnPlane(fwd, kart.SurfaceNormal);
        
        if (projectedFwd.sqrMagnitude < 0.001f)
            projectedFwd = Vector3.ProjectOnPlane(kart.transform.up, kart.SurfaceNormal);

        Quaternion targetRot = Quaternion.LookRotation(projectedFwd, kart.SurfaceNormal);
        kart.Rb.MoveRotation(Quaternion.Slerp(kart.Rb.rotation, targetRot, kart.Settings.rotationSmoothness * Time.fixedDeltaTime));
    }
}
