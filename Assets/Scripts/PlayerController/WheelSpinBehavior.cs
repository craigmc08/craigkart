using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpinBehavior : MonoBehaviour
{
    public Transform frontAxle;
    public Transform backAxle;
    public float frontWheelRadii;
    public float backWheelRadii;

    float speed = 0;
    private void Update()
    {
        float frontWheelSpeed = speed / frontWheelRadii;
        Quaternion frontWheelRotation = Quaternion.AngleAxis(frontWheelSpeed * Mathf.Rad2Deg * Time.deltaTime, Vector3.right);
        float backWheelSpeed = speed / backWheelRadii;
        Quaternion backWheelRotation = Quaternion.AngleAxis(backWheelSpeed * Mathf.Rad2Deg * Time.deltaTime, Vector3.right);

        backAxle.localRotation *= backWheelRotation;
        float currentFrontX = frontAxle.localEulerAngles.x;
        frontAxle.localRotation *= frontWheelRotation;
    }

    public void Drive(SpeedParameters p)
    {
        speed = p.speed;
    }
}
