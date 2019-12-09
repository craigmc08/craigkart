using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpinBehavior : MonoBehaviour
{
    public Transform frontAxle;
    public Transform backAxle;
    public float frontWheelRadii;
    public float backWheelRadii;

    public void Drive(SpeedParameters p)
    {
        float frontWheelSpeed = p.speed / frontWheelRadii;
        Quaternion frontWheelRotation = Quaternion.AngleAxis(frontWheelSpeed * Mathf.Rad2Deg * Time.fixedDeltaTime, Vector3.right);
        float backWheelSpeed = p.speed / backWheelRadii;
        Quaternion backWheelRotation = Quaternion.AngleAxis(backWheelSpeed * Mathf.Rad2Deg * Time.fixedDeltaTime, Vector3.right);

        backAxle.localRotation *= backWheelRotation;
        float currentFrontX = frontAxle.localEulerAngles.x;
        frontAxle.localRotation *= frontWheelRotation;
    }
}
