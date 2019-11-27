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
    [Space(10)]
    public float frictionCoF;
    public float gripFactor;

    [Header("Turning")]
    public float kartWeight = 1;
    public float turningForce = 1;
    public float minTurnRadius = 1;

    [Header("Drifting/Hopping")]
    public float hopSpeed;
    public float driftInitiateThreshold = 0.2f;
    [Tooltip("After pressing the hop button, the hop will be remembered on the ground for this amount of time")]
    public float hopExtensionTime = 0.1f;
    public bool insideDrift = false;
    [Tooltip("Multiplies the maximum turning force by this value. Higher means tighter turns")]
    public float driftTightness = 3;
    public Vector2 driftSteerRange;

    [Header("Mini-Turbo Charging")]
    public float requiredCharge = 250;
    public float tightChargeRate = 150;
    public float looseChargeRate = 50;
    [Tooltip("If the steering input is higher than this value (adjusted for drift direction), then the turn is considered 'tight'.")]
    [Range(-1, 1)]
    public float driftTightnessThreshold = 0.5f;

    [Header("Physics")]
    [Range(0,1)]
    [Tooltip("Smoothing to apply to surface normal value")]
    public float surfaceSmoothing = 0.2f;
    [Space(10)]
    public LayerMask groundLayerMask;
    public Vector3 groundRayOffset;
    public float groundRayLength;

    private float m_Accelerate = 0;
    private float m_Decelerate = 0;
    private float m_Steer;
    private bool m_Hop;

    private Vector2 m_Lean;
    private bool m_Trick;

    private Rigidbody rb;

    // Driving stuff
    Vector3 surfaceNormal = Vector3.up;
    Vector3 surfaceForward = Vector3.forward;

    // Drifting stuff
    bool lastHopVal = false;
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
        var velocity = rb.velocity;

        // ROAD SURFACE
        RaycastHit groundHit;
        bool onGround = Physics.Raycast(transform.position + groundRayOffset, -surfaceNormal, out groundHit, groundRayLength, groundLayerMask.value);
        if (onGround)
        {
            surfaceNormal = Vector3.Slerp(groundHit.normal, surfaceNormal, surfaceSmoothing);
            surfaceForward = Vector3.ProjectOnPlane(surfaceForward, surfaceNormal).normalized;
        }

        // DRIFTING
        if (lastHopVal != m_Hop && m_Hop && !readyToDrift && !drifting)
        {
            // Only hop if the button was pressed this update
            if (onGround)
            {
                velocity += surfaceNormal * hopSpeed;
                hoppedAt = Time.time;
            }
            readyToDrift = true;
        }

        if (readyToDrift && onGround && Time.time > hoppedAt + hopExtensionTime)
        {
            if (Mathf.Abs(m_Steer) > driftInitiateThreshold)
            {
                // Initiate drift
                drifting = true;
                driftingRight = m_Steer > 0;
                Debug.Log("starting drifting to the " + (driftingRight ? "right" : "left"));
                miniTurboCharge = 0;
            }
            // Don't initiate drift
            readyToDrift = false;
        }

        if (drifting && !m_Hop)
        {
            Debug.Log("stopping drift");
            drifting = false;
        }

        if (drifting)
        {
            var driftSteer = m_Steer * (driftingRight ? 1 : -1);

            if (driftSteer > driftTightnessThreshold)
            {
                miniTurboCharge += tightChargeRate * Time.fixedDeltaTime;
            } else
            {
                miniTurboCharge += looseChargeRate * Time.fixedDeltaTime;
            }

            if (miniTurboCharge > requiredCharge)
            {
                Debug.Log("MT charged!");
            }
        }


        // ROTATION
        // Minimum turn radius required to prevent div by 0 errors
        float turnRadius = Mathf.Max(velocity.sqrMagnitude / turningForce, minTurnRadius);
        float turnSpeed;
        if (!drifting)
        {
            turnSpeed = velocity.magnitude / turnRadius * m_Steer;
        } else
        {
            var steerMin = driftingRight ? driftSteerRange.x : -driftSteerRange.y;
            var steerMax = driftingRight ? driftSteerRange.y : -driftSteerRange.x;
            var driftSteer = Mathf.Lerp(steerMin, steerMax, (m_Steer + 1f) / 2f);
            turnSpeed = velocity.magnitude / turnRadius * driftSteer * driftTightness;
        }
        surfaceForward = Quaternion.AngleAxis(turnSpeed * Mathf.Rad2Deg * Time.fixedDeltaTime, surfaceNormal) * surfaceForward;

        Quaternion kartRotation = Quaternion.LookRotation(surfaceForward, surfaceNormal);
        transform.rotation = kartRotation;

        // MOVEMENT
        var surfaceRight = Vector3.Cross(surfaceForward, surfaceNormal);

        var friction = -velocity.normalized * frictionCoF;
        var gripForce = Vector3.Dot(velocity, surfaceRight) * gripFactor * -surfaceRight;

        var accelInput = m_Accelerate - m_Decelerate;
        var acceleration = accelInput * (accelInput < 0 ? backwardAcceleration : forwardAcceleration) * surfaceForward;

        var totalAcceleration = friction + gripForce + acceleration;
        // All of this acceleration should be in the surface plane
        totalAcceleration = Vector3.ProjectOnPlane(totalAcceleration, surfaceNormal);
        velocity += totalAcceleration * Time.fixedDeltaTime;

        var forwardsComponent = Vector3.Dot(velocity, surfaceForward);
        var movingForwards = forwardsComponent > 0;

        var upwardsComponent = Vector3.Dot(velocity, surfaceNormal);
        var tangentialVelocity = velocity - upwardsComponent * surfaceNormal;
        if (movingForwards && forwardsComponent > maxForwardSpeed)
        {
            tangentialVelocity *= maxForwardSpeed / forwardsComponent;
        } else if (!movingForwards && forwardsComponent < -maxBackwardSpeed)
        {
            tangentialVelocity *= -maxBackwardSpeed / forwardsComponent;
        }
        velocity = tangentialVelocity + upwardsComponent * surfaceNormal;

        rb.velocity = velocity;

        m_Trick = false;
        lastHopVal = m_Hop;
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
        m_Trick = value.Get<float>() > 0.5f;
    }
}
