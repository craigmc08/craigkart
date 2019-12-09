using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxleSteerBehavior : MonoBehaviour
{
    public Transform steeringAssembly;
    public float maxFrontAxleYaw = 15f;
    [Range(0, 1)]
    public float frontAxleYawSmoothing = 0.8f;

    public void Steer(SteerParameters p)
    {
        float currentFrontY = steeringAssembly.localEulerAngles.y;
        if (currentFrontY > 180) currentFrontY -= 360;
        float frontAxleAngle = Mathf.Lerp(-maxFrontAxleYaw, maxFrontAxleYaw, (p.steer + 1) / 2);
        frontAxleAngle = Mathf.Lerp(frontAxleAngle, currentFrontY, frontAxleYawSmoothing);
        frontAxleAngle = Mathf.Clamp(frontAxleAngle, -maxFrontAxleYaw, maxFrontAxleYaw);
        Quaternion frontAxleRotation = Quaternion.AngleAxis(frontAxleAngle, Vector3.up);

        steeringAssembly.localRotation = frontAxleRotation;
    }
}
