﻿using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PDriverController))]
public class PCameraController : MonoBehaviour
{
    [Range(0,1)]
    public float directionSmoothing = 0.8f;
    [Range(0, 1)]
    public float cameraUpLimit;

    public Vector2 swivelSpan;
    public float yRestAngle;
    public Vector3 center;

    public float dist;
    public float zOffset;

    [Range(0f, 1f)]
    public float swivelSmoothing = 0.2f;

    public Transform playerCamera;
    public Transform cameraRotationEmpty;

    Vector2 m_CameraSwivel;
    Vector2 cameraSwivelValue;

    bool m_LookBack;

    PDriverController controller;

    void Start()
    {
        m_CameraSwivel = Vector2.zero;
        controller = GetComponent<PDriverController>();
    }

    Quaternion lastEmptyRot = default;
    void Update()
    {
        var cameraUp = Vector3.Slerp(Vector3.up, controller.GroundUp, cameraUpLimit);
        Quaternion emptyRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(controller.KartForward, cameraUp).normalized, cameraUp);

        if (lastEmptyRot == default) {
            lastEmptyRot = emptyRot;
        }
        emptyRot = Quaternion.Slerp(emptyRot, lastEmptyRot, directionSmoothing);
        lastEmptyRot = emptyRot;

        cameraRotationEmpty.rotation = emptyRot;

        cameraSwivelValue = Vector2.Lerp(cameraSwivelValue, m_CameraSwivel, 1f - swivelSmoothing);

        float yAngle = -cameraSwivelValue.y * swivelSpan.y * Mathf.Deg2Rad / 2 + yRestAngle * Mathf.Deg2Rad;
        float xAngle = -cameraSwivelValue.x * swivelSpan.x * Mathf.Deg2Rad / 2;

        Vector3 cameraPos = new Vector3(
            Mathf.Sin(xAngle) * Mathf.Cos(yAngle),
            Mathf.Sin(yAngle),
            -Mathf.Cos(xAngle) * Mathf.Cos(yAngle)
        ) * dist + Vector3.forward * zOffset;

        if (m_LookBack)
        {
            cameraPos.x *= -1;
            cameraPos.z *= -1;
        }
        Quaternion cameraRotation = Quaternion.LookRotation(-cameraPos, Vector3.up);

        Vector3 actualCameraPos = cameraPos + center;
        actualCameraPos.y = Mathf.Max(0.25f, actualCameraPos.y);
        playerCamera.localPosition = actualCameraPos;
        playerCamera.localRotation = cameraRotation;
    }


    public void OnCameraSwivel(InputValue value)
    {
        m_CameraSwivel = value.Get<Vector2>();
        if (!controller.controllable) m_CameraSwivel = Vector2.zero;
        Debug.Log(m_CameraSwivel);
    }

    public void OnLookBack(InputValue value)
    {
        m_LookBack = value.Get<float>() > 0.5f;   
        if (!controller.controllable) m_LookBack = false;
        Debug.Log(m_LookBack);
    }
}
