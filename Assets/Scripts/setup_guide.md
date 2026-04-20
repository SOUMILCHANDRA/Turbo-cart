# Anti-Gravity Kart Controller Setup Guide

This guide explains how to implement the anti-gravity kart racing controller in your Unity project.

## 1. Project Setup

1.  **Create the Script**: Save the provided `AntiGravityKartController.cs` into your `Assets/Scripts` folder.
2.  **Create the Kart**:
    *   Create an empty GameObject named **Kart**.
    *   Add a **Rigidbody** component.
    *   Add the **AntiGravityKartController** component.
3.  **Setup Layer**:
    *   Create a new Layer named `Track`.
    *   Assign this layer to your track models/meshes.
    *   In the Kart's Controller component, set the **Track Layer** mask to `Track`.

## 2. GameObject Hierarchy (Recommended)

To ensure stability, use child objects for raycast points:

*   **Kart** (Rigidbody + Controller)
    *   **Visuals** (Your kart model)
    *   **GroundPoints** (Empty parent)
        *   **FrontLeft**
        *   **FrontRight**
        *   **BackLeft**
        *   **BackRight**

Assign these four children to the **Ground Points** array in the inspector.

## 3. Configuration Tips

### Rigidbody
*   **Mass**: ~1000-1500 (standard for vehicles).
*   **Use Gravity**: **Uncheck** this (the script handles custom gravity).
*   **Interpolation**: Set to **Interpolate** for smooth movement.
*   **Collision Detection**: **Continuous** or **Continuous Speculative**.

### Controller Settings
*   **Hover Height**: `0.8 - 1.2` usually feels best.
*   **Gravity Force**: `30 - 50` is good for "heavy" arcade feel.
*   **Hover Snappiness**: `20 - 40`. Too high causes jitter; too low causes "bottoming out" on loops.
*   **Rotation Smoothness**: `10 - 15`. This controls how quickly the kart snaps to the track's normal.

## 4. Troubleshooting

*   **Kart is jittering**: Increase the number of `GroundPoints` and ensure `Hover Snappiness` isn't too high.
*   **Kart flies off loops**: Increase `Gravity Force` or `Ground Check Distance`.
*   **Kart doesn't move**: Ensure the `Track` layer is correctly assigned and the rays are hitting something.

## 5. Camera Setup

1.  **Main Camera**: 
    *   Find the **Main Camera** in your scene.
    *   Add the **AntiGravityCamera** component.
    *   Assign your **Kart** GameObject to the **Target** field.
2.  **Tuning**:
    *   **Distance**: `6 - 10` is standard.
    *   **Height**: `2 - 4` provides a clear view.
    *   **Smooth Speed**: `10 - 15` for a responsive but cinematic feel.
    *   **Rotation Smoothness**: `5 - 10`. Lower values add more "lag" and cinematic weight.

## 6. Controls

*   **Accelerate/Reverse**: `W / S` or `Up / Down Arrow`
*   **Steer**: `A / D` or `Left / Right Arrow`
*   **Drift (Hold)**: `Left Shift` (requires turning)
*   **Boost (Release)**: Release `Left Shift` after drifting to gain speed.

## 7. Drift & Boost Tuning

*   **Drift Turning Multiplier**: `1.5 - 2.5`. Higher means sharper turns while drifting.
*   **Drift Grip Amount**: `1.0 - 4.0`. Lower means more lateral sliding (traditional drift).
*   **Normal Grip Amount**: `8.0 - 15.0`. Controls how much the kart "sticks" to the track normally.
*   **Boost Force**: `50 - 150`. Controls the strength of the burst on release.
