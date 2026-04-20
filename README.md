# Turbo Kart: Anti-Gravity Racing Controller

A professional-grade, Rigidbody-based anti-gravity racing system for Unity 3D. Designed for high-speed, arcade-style gameplay with support for complex geometry like loops, wall-driving, and inverted tracks.

![License](https://img.shields.io/badge/Unity-2021.3%2B-blue)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac%20%7C%20Linux-green)

## 🚀 Key Features

- **Modular Track Builder**: An editor tool for snapping track segments together with automated alignment, supporting loops, vertical walls, and jumps.
- **Magnetic Adhesion Physics**: Dynamic downforce and grip that scales with speed, ensuring the kart stays "glued" to the track even on extreme inverted geometry.
- **Multi-Point Surface Detection**: Average normal calculation using multiple raycast points to prevent jitter on sharp edges or bumps.
- **Smooth Cinematic Camera**: A custom third-person camera that aligns with the kart's orientation to handle loops without flipping or gimbal lock.
- **Drift & Boost System**: Drift mechanics with side-slip physics, allowing for high-speed cornering and a visualizable boost charge system.
- **Modular Boost Pads**: Reusable track elements to temporarily enhance speed and acceleration with integrated particle and audio feedback.
- **Hover Suspension**: Spring-damper hover logic for a satisfying, futuristic racing feel.

## 🛠️ Setup Overview

1.  **Layers**: Create a `Track` layer and assign it to your track meshes.
2.  **Kart Setup**:
    *   Attach `AntiGravityKartController` to your kart model.
    *   Disable `Use Gravity` on the Rigidbody.
    *   Set **Track Layer** to `Track`.
    *   Assign child objects as **Ground Points** for maximum stability.
3.  **Camera**: Attach `AntiGravityCamera` to your Main Camera and assign the kart as the target.
4.  **Boost Pads**: Place `BoostPad` scripts on triggers around your track for tactical speed gains.

> [!TIP]
> For a detailed walkthrough, check out the [Setup Guide](Assets/Scripts/setup_guide.md).

## 🎮 Controls

| Action | Control |
| :--- | :--- |
| **Accelerate** | `W` or `Up Arrow` |
| **Reverse / Brake** | `S` or `Down Arrow` |
| **Steer** | `A / D` or `Left / Right Arrow` |
| **Drift** | Hold `Left Shift` while turning |
| **Boost** | Release `Left Shift` after drifting |

## 🧪 Tuning Parameters

*   **Adhesion Strength**: Controls how hard the kart is pulled toward the surface at speed.
*   **Grip Speed Factor**: Adjusts cornering stability during high-speed maneuvers.
*   **Hover Snappiness**: Controls the "weight" and response of the hover suspension.
*   **Rotation Smoothness**: Determines how quickly the kart aligns to new surface normals.

## 📜 License

This project is designed for arcade racing enthusiasts and developers. Feel free to use and modify it for your futuristic racing projects!
