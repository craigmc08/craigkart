using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PDriverController))]
public class PCameraController : MonoBehaviour
{
    [Range(0,1)]
    public float cameraUpSmoothing = 0.8f;
    [Range(0, 1)]
    public float cameraUpLimit;

    public Vector2 swivelSpan;
    public float yRestAngle;
    public Vector3 center;

    public float dist;
    public float zOffset;

    [Range(0f, 1f)]
    public float smoothing = 0.2f;

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

    Vector3 groundNormal = Vector3.up;
    void Update()
    {
        groundNormal = Vector3.Slerp(controller.GroundUp, groundNormal, cameraUpSmoothing);
        var cameraUp = Vector3.Slerp(Vector3.up, groundNormal, cameraUpLimit);
        Quaternion emptyRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(controller.KartForward, cameraUp).normalized, cameraUp);
        cameraRotationEmpty.rotation = emptyRot;

        cameraSwivelValue = Vector2.Lerp(cameraSwivelValue, m_CameraSwivel, 1f - smoothing);

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
        Debug.Log(m_CameraSwivel);
    }

    public void OnLookBack(InputValue value)
    {
        m_LookBack = value.Get<float>() > 0.5f;   
        Debug.Log(m_LookBack);
    }
}
