using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PDriverController : MonoBehaviour
{
    [Header("Speed")]
    public float maxForwardSpeed;
    public float forwardAcceleration;
    public float maxBackwardSpeed;
    public float backwardAcceleration;
    [Space(10)]
    public float boostingSpeed;
    public float boostingAcceleration;

    [Header("Turning")]
    public float kartWeight = 1;
    public float cofMultiplier = 1;
    public float minTurnRadius = 1;

    [Header("Drifting/Hopping")]
    public float hopSpeed;
    public bool insideDrift = true;
    [Tooltip("Multiplies the maximum turning force by this value. Higher means tighter turns")]
    public float driftTightness = 3;

    [Header("Mini-Turbo Charging")]
    public float requiredCharge = 250;
    public float tightChargeRate = 150;
    public float looseChargeRate = 50;
    [Tooltip("If the steering input is higher than this value (adjusted for drift direction), then the turn is considered 'tight'.")]
    [Range(-1, 1)]
    public float driftTightnessThreshold = 0.5f;
    [Tooltip("After pressing the hop button, the hop will be remembered on the ground for this amount of time")]
    public float hopExtensionTime = 0.1f;

    private float m_Accelerate = 0;
    private float m_Decelerate = 0;
    private float m_Steer;
    private bool m_Hop;

    private Vector2 m_Lean;
    private bool m_Trick;

    private Rigidbody rb;

    // Driving stuff
    float currentSpeed = 0;
    float kartHeading = 0;
    Vector3 surfaceNormal = Vector3.up;
    Vector3 surfaceTangent = Vector3.forward;
    Vector3 velocity = Vector3.zero;

    // Drifting stuff
    bool readyToDrift = false;
    float hoppedAt = 0;

    bool drifting = false;
    bool driftingRight = false;
    float miniTurboCharge = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = kartWeight;
    }

    private void FixedUpdate()
    {
        Vector3 alongFloorAtHeading = new Vector3(Mathf.Sin(kartHeading), Mathf.Cos(kartHeading));
        surfaceTangent = Vector3.ProjectOnPlane(alongFloorAtHeading, surfaceNormal);

        float accelerationConstant = m_Accelerate - m_Decelerate;
        float targetAcceleration = accelerationConstant * (accelerationConstant < 0 ? backwardAcceleration : forwardAcceleration);

        currentSpeed += targetAcceleration * Time.fixedDeltaTime;
        // velocity += targetAcceleration along surface tangent
        // project velocity onto surface tangent

        float groundFrictionAccel = 1;
        float turningRadius = Mathf.Max(velocity.sqrMagnitude / groundFrictionAccel, minTurnRadius);
        float angularSpeed = currentSpeed / turningRadius;

        rb.angularVelocity = Vector3.up * angularSpeed;
        rb.velocity = currentSpeed * surfaceNormal;

        Debug.Log(surfaceTangent + ", " + surfaceNormal + ", " + accelerationConstant + ", " + targetAcceleration + ", " + currentSpeed + ", " + turningRadius + ", " + angularSpeed + ", " + rb.velocity);

        m_Trick = false;
    }

    public void OnAccelerate(InputValue value)
    {
        m_Accelerate = value.Get<float>();
    }

    public void OnDecelerate(InputValue value)
    {
        m_Decelerate = value.Get<float>();
    }

    public void OnSteer(InputValue value)
    {
        m_Steer = value.Get<float>();
    }

    public void OnHop(InputValue value)
    {
        m_Hop = value.Get<float>() > 0.5f;
    }

    public void OnLean(InputValue value)
    {
        m_Lean = value.Get<Vector2>();
    }

    public void OnTrick(InputValue value)
    {
        m_Trick = value.Get<bool>();
    }
}
