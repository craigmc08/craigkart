using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SteerParameters
{
    public float steer;
    public bool drifting;
}

public struct SpeedParameters
{
    public float speed;
    public bool drifting;
}

public struct DriftParameters
{
    public bool drifting;
    public bool mtCharged;
    public bool stCharged;
    public bool utCharged;
}

public struct TurboParameters
{
    public bool turboing;
    public float duration;
    public float strength;
    public float progress;
}

[RequireComponent(typeof(Rigidbody))]
public class PDriverController : MonoBehaviour
{
    [Header("Speed")]
    public float maxForwardSpeed;
    public float forwardAcceleration;
    public float maxBackwardSpeed;
    public float backwardAcceleration;
    [Space(10)]
    public float boostingAcceleration;
    [Space(10)]
    public float frictionCoF;
    public float brakeStrength;

    [Header("Turning")]
    public float kartWeight = 1;
    public float turningForce;
    public float minTurnRadius = 1;

    [Header("Drifting/Hopping")]
    public float hopSpeed;
    public float driftInitiateThreshold = 0.2f;
    [Tooltip("After pressing the hop button, the hop will be remembered on the ground for this amount of time")]
    public float hopExtensionTime = 0.1f;
    public bool insideDrift = true;
    [Tooltip("Multiplies the maximum turning force by this value. Higher means tighter turns")]
    public float driftTightness = 3;
    public float outsideSlippiness = 10;
    public Vector2 driftSteerRange;

    [Header("Mini-Turbo Charging")]
    public float chargeForMiniturbo = 250;
    public float chargeForSuperturbo = 450;
    public float chargeForUltraturbo = 650;
    public bool canUltraTurbo = false;
    [Space(10)]
    public float tightChargeRate = 150;
    public float looseChargeRate = 50;
    [Tooltip("If the steering input is higher than this value (adjusted for drift direction), then the turn is considered 'tight'.")]
    [Range(-1, 1)]
    public float driftTightnessThreshold = 0.5f;
    [Space(10)]
    public float miniturboDuration = 0.5f;
    public float superturboDuration = 1f;
    public float ultraturboDuration = 1.5f;
    [Space(10)]
    [Tooltip("Multiplier on base speed while mini/super/ultra-turboing")]
    public float miniturboBoost = 2f;

    [Header("Physics")]
    [Range(0,1)]
    [Tooltip("How fast the surface normal can change")]
    public float surfaceSmoothing = 0.2f;
    [Space(10)]
    public LayerMask groundLayerMask;

    [Header("Wheels And Body")]
    public Transform kartRotationEmpty;

    private float m_Accelerate = 0;
    private float m_Decelerate = 0;
    private float m_Steer;
    private bool m_Hop;

    private Vector2 m_Lean;
    private bool m_Trick;

    private Rigidbody rb;

    // Driving stuff
    public bool grounded = false;
    Vector3 contactNormal = Vector3.up;
    public Vector3 saveSurfaceNormal = Vector3.up;
    public Vector3 surfaceNormal = Vector3.up;
    Vector3 surfaceForward = Vector3.forward;

    // Drifting stuff
    bool lastHopVal = false;
    bool readyToDrift = false;
    float hoppedAt = 0;

    bool drifting = false;
    bool driftingRight = false;
    float turboCharge = 0;

    bool turboActivated = false;
    float turboStartedAt = 0;
    float turboDuration = 0;
    float turboBoost = 0;

    public bool controllable = false;
    public bool runPhysics = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = kartWeight;
        surfaceForward = transform.forward;
    }

    private void FixedUpdate()
    {
        if (!runPhysics) return;

        canUltraTurbo = !insideDrift;

        var velocity = rb.velocity;

        // ROAD SURFACE
        if (grounded)
        {
            surfaceNormal = Vector3.Slerp(contactNormal, surfaceNormal, surfaceSmoothing);
        } else
        {
            surfaceNormal = Vector3.Slerp(saveSurfaceNormal, surfaceNormal, surfaceSmoothing);
        }
        surfaceForward = Vector3.ProjectOnPlane(surfaceForward, surfaceNormal).normalized;

        // DRIFTING
        bool tryHop = m_Hop && m_Accelerate > 0.1f; // Do not enter a drift if accelerator is not prssed
        if (lastHopVal != m_Hop && tryHop && !readyToDrift && !drifting)
        {
            // Only hop if the button was pressed this update
            if (grounded)
            {
                velocity += surfaceNormal * hopSpeed;
                hoppedAt = Time.time;
            }
            readyToDrift = true;
        }

        if (readyToDrift && grounded && Time.time > hoppedAt + hopExtensionTime)
        {
            if (Mathf.Abs(m_Steer) > driftInitiateThreshold)
            {
                // Initiate drift
                drifting = true;
                driftingRight = m_Steer > 0;
                turboCharge = 0;
            }
            // Don't initiate drift
            readyToDrift = false;
        }

        // Stop drifting when handbrake is released or accelerator is released
        if (drifting && (!m_Hop || m_Accelerate < 0.1f))
        {
            drifting = false;
            EndDrift();
        }

        if (drifting)
        {
            var driftSteer = m_Steer * (driftingRight ? 1 : -1);

            if (grounded)
            {
                if (driftSteer > driftTightnessThreshold)
                {
                    turboCharge += tightChargeRate * Time.fixedDeltaTime;
                } else
                {
                    turboCharge += looseChargeRate * Time.fixedDeltaTime;
                }
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
            turnRadius /= driftTightness;
            var driftSteer = Mathf.Lerp(steerMin, steerMax, (m_Steer + 1f) / 2f);
            turnSpeed = velocity.magnitude / turnRadius * driftSteer;
        }
        surfaceForward = Quaternion.AngleAxis(turnSpeed * Mathf.Rad2Deg * Time.fixedDeltaTime, surfaceNormal) * surfaceForward;

        Quaternion kartRotation = Quaternion.LookRotation(surfaceForward, surfaceNormal);
        kartRotationEmpty.rotation = kartRotation;

        // MOVEMENT
        var surfaceRight = Vector3.Cross(surfaceForward, surfaceNormal);

        var velocityDir = velocity.normalized;
        var friction = -velocityDir * frictionCoF;
        var slipVelocity = Vector3.Dot(velocity, surfaceRight);
        var gripAmount = !drifting ? turningForce
                         : insideDrift ? turningForce * driftTightness : turningForce / outsideSlippiness;
        var gripStrength = Mathf.Sign(slipVelocity) * Mathf.Min(Mathf.Abs(slipVelocity), gripAmount);
        // gripForce should immediately cancel out all slip velocity
        var gripForce = gripStrength * -surfaceRight / Time.fixedDeltaTime;

        var brakeForce = Vector3.zero;
        if (m_Hop && !tryHop)
        {
            brakeForce = -velocityDir * brakeStrength;
        }

        var accelInput = m_Accelerate - m_Decelerate;
        var acceleration = accelInput * (accelInput < 0 ? backwardAcceleration : forwardAcceleration) * surfaceForward;
        if (turboActivated) acceleration *= turboBoost;

        var totalAcceleration = friction + gripForce + brakeForce + acceleration;
        // All of this acceleration should be in the surface plane
        totalAcceleration = Vector3.ProjectOnPlane(totalAcceleration, surfaceNormal);
        velocity += totalAcceleration * Time.fixedDeltaTime;

        var forwardsComponent = Vector3.Dot(velocity, surfaceForward);
        var movingForwards = forwardsComponent > 0;

        var upwardsComponent = Vector3.Dot(velocity, surfaceNormal);
        var tangentialVelocity = velocity - upwardsComponent * surfaceNormal;
        var maxSpeedMultiplier = turboActivated ? turboBoost : 1;
        if (movingForwards && forwardsComponent > maxForwardSpeed * maxSpeedMultiplier)
        {
            tangentialVelocity *= maxForwardSpeed * maxSpeedMultiplier / forwardsComponent;
        } else if (!movingForwards && forwardsComponent < -maxBackwardSpeed * maxSpeedMultiplier)
        {
            tangentialVelocity *= -maxBackwardSpeed * maxSpeedMultiplier / forwardsComponent;
        }
        velocity = tangentialVelocity + upwardsComponent * surfaceNormal;

        rb.velocity = velocity;

        SendMessage("Steer", new SteerParameters { steer = m_Steer, drifting = drifting });
        SendMessage("Drive", new SpeedParameters { speed = forwardsComponent, drifting = drifting });
        SendMessage("Drift", new DriftParameters {
            drifting = drifting,
            mtCharged = drifting && turboCharge > chargeForMiniturbo,
            stCharged = drifting && turboCharge > chargeForSuperturbo,
            utCharged = drifting && turboCharge > chargeForUltraturbo
        });
        SendMessage("Turbo", new TurboParameters
        {
            duration = turboDuration,
            progress = (Time.time - turboStartedAt) / turboDuration,
            strength = turboBoost,
            turboing = turboActivated
        });

        m_Trick = false;
        lastHopVal = m_Hop;

        UpdateTurbo();
        UpdateSurfaceContacts();
    }

    void EndDrift()
    {
        bool utCharged = turboCharge > chargeForUltraturbo && canUltraTurbo;
        bool stCharged = turboCharge > chargeForSuperturbo;
        bool mtCharged = turboCharge > chargeForMiniturbo;

        if (mtCharged || stCharged || utCharged)
        {
            turboActivated = true;
            turboStartedAt = Time.time;
        }
        if (utCharged)
        {
            turboDuration = ultraturboDuration;
            turboBoost = miniturboBoost;
        } else if (stCharged)
        {
            turboDuration = superturboDuration;
            turboBoost = miniturboBoost;
        } else if (mtCharged)
        {
            turboDuration = miniturboDuration;
            turboBoost = miniturboBoost;
        }
    }

    void UpdateTurbo()
    {
        if (turboStartedAt + turboDuration < Time.time)
        {
            turboActivated = false;
            turboDuration = 0;
            turboBoost = 0;
        }
    }

    public void OnAccelerate(InputValue value)
    {
        m_Accelerate = value.Get<float>();
        if (!controllable) m_Accelerate = 0;
    }

    public void OnDecelerate(InputValue value)
    {
        m_Decelerate = value.Get<float>();
        if (!controllable) m_Decelerate = 0;
    }

    public void OnSteer(InputValue value)
    {
        m_Steer = value.Get<float>();
        if (!controllable) m_Steer = 0;
    }

    public void OnHop(InputValue value)
    {
        m_Hop = value.Get<float>() > 0.5f;
        if (!controllable) m_Hop = false;
    }

    public void OnLean(InputValue value)
    {
        m_Lean = value.Get<Vector2>();
        if (!controllable) m_Lean = Vector2.zero;
    }

    public void OnTrick(InputValue value)
    {
        m_Trick = value.Get<float>() > 0.5f;
        if (!controllable) m_Trick = false;
    }

    bool IsInLayerMask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    private ContactPoint[] surfaceContacts = new ContactPoint[16];
    // Collect all contacts each update. There can be multiple CollisionStays per update
    int groundContactLength = 0;
    private ContactPoint[] groundContactsThisUpdate = new ContactPoint[64];
    int surfaceContactLength = 0;
    private ContactPoint[] surfaceContactsThisUpdate = new ContactPoint[64];
    public void OnCollisionStay(Collision col)
    {
        if (!runPhysics) return;

        if (IsInLayerMask(groundLayerMask, col.gameObject.layer)) {
            grounded = true;
            var surface = col.gameObject.GetComponent<Surface>();
            if (surface == null)
            {
                Debug.LogWarning("Object is in layer 'Surface' but has no Surface object associated with it. Ignoring the collision.");
                return;
            }

            // Find average direction of each contact normal
            int length = col.GetContacts(surfaceContacts);
            for (var i = 0; i < length; i++)
            {
                if (surfaceContactLength >= surfaceContactsThisUpdate.Length)
                {
                    Debug.LogWarning("Surface Contact buffer out of space");
                } else
                {
                    surfaceContactsThisUpdate[surfaceContactLength] = surfaceContacts[i];
                    surfaceContactLength++;
                }

                if (surface.IsGround || surface.IsOffroad)
                {
                    if (groundContactLength >= groundContactsThisUpdate.Length)
                    {
                        Debug.LogWarning("Ground Contact buffer out of space");
                    }
                    else
                    {
                        groundContactsThisUpdate[groundContactLength] = surfaceContacts[i];
                        groundContactLength++;
                    }
                }
            }
            grounded = true;
        }
    }

    void UpdateSurfaceContacts()
    {
        if (!runPhysics) return;

        // No contacts this update
        if (surfaceContactLength == 0)
        {
            grounded = false;
            return;
        }

        Vector3 groundTotal = Vector3.zero;
        Vector3 surfaceTotal = Vector3.zero;
        for (var i = 0; i < surfaceContactLength; i++)
        {
            if (i < groundContactLength) groundTotal += groundContactsThisUpdate[i].normal;
            surfaceTotal += surfaceContactsThisUpdate[i].normal;
        }

        if (groundContactLength > 0)
        {
            saveSurfaceNormal = groundTotal.normalized;
        }

        contactNormal = surfaceTotal.normalized;

        groundContactLength = 0;
        surfaceContactLength = 0;
    }

    public Vector3 KartForward
    {
        get => surfaceForward;
    }
    public Vector3 KartUp
    {
        get => surfaceNormal;
    }
    public Vector3 GroundUp
    {
        get => saveSurfaceNormal;
    }
    public bool Grounded
    {
        get => grounded;
    }

    public bool InsideDrift
    {
        get => insideDrift;
    }
    public bool OutsideDrift
    {
        get => !insideDrift;
    }
}
