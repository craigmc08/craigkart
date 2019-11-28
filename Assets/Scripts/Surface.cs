using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    public enum SurfaceType
    {
        Ground = 1 << 0,
        Obstacle = 1 << 1,
        Offroad = 1 << 2
    }

    public enum DriveType
    {
        Road = 1 << 0,
        TurboPanel = 1 << 1,
    }

    [SerializeField]
    public SurfaceType surfaceType;
    [SerializeField]
    public DriveType driveType;
    public float OffroadFactor { get; private set; }

    public bool IsGround
    {
        get => surfaceType == SurfaceType.Ground;
    }
    public bool IsObstacle
    {
        get => surfaceType == SurfaceType.Obstacle;
    }
    public bool IsOffroad
    {
        get => surfaceType == SurfaceType.Offroad;
    }

    public bool IsRoad
    {
        get => driveType.HasFlag(DriveType.Road);
    }
    public bool IsTurboPanel
    {
        get => driveType.HasFlag(DriveType.TurboPanel);
    }
}
