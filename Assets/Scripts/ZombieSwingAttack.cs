using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using System.Collections;
using System.Collections.Generic;
public class ZombieSwingAttack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("hand collision object")]
    public Transform zombieHand;
    [Tooltip("The target transform controlled by TwoBoneIK")]
    public Transform handTarget;
    [Tooltip("The zombie's root transform (for position reference)")]
    public Transform zombieTransform;

    [Header("Attack Settings")]
    [Tooltip("Duration of the swing attack in seconds")]
    public float attackDuration = 0.5f;
    [Tooltip("Radius of the swing arc")]
    public float attackRadius = 1.5f;
    [Tooltip("Angle range for the swing arc (degrees)")]
    public Vector2 swingAngles = new Vector2(-45f, 45f);
    [Tooltip("Curve to control swing motion easing")]
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Blending")]
    public TwoBoneIKConstraint armIKConstraint;
    [Range(0,1)] public float ikWeightDuringSwing = 0.7f;

    // Internal state
    private Vector3 originalHandPosition;
    private bool isAttacking = false;

    void Start()
    {
        // Store initial position for resetting
        if (handTarget != null)
        {
            originalHandPosition = handTarget.localPosition;
        }
    }

    void Update()
    {
        // Example trigger - replace with your game's logic
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformSwingAttack());
        }
    }

    System.Collections.IEnumerator PerformSwingAttack()
    {
        isAttacking = true;
        float timer = 0f;
        
        // Calculate direction based on zombie's forward
        Vector3 attackDirection = zombieTransform.forward;

        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / attackDuration;
            
            // Apply easing curve
            float easedTime = swingCurve.Evaluate(normalizedTime);
            armIKConstraint.weight = Mathf.Lerp(0, ikWeightDuringSwing, easedTime);
            
            // Calculate angle in radians
            float currentAngle = Mathf.Lerp(
                swingAngles.x * Mathf.Deg2Rad,
                swingAngles.y * Mathf.Deg2Rad,
                easedTime
            );

            // Calculate position on arc (XZ plane)
            Vector3 arcOffset = new Vector3(
                Mathf.Sin(currentAngle) * attackRadius,
                0,
                Mathf.Cos(currentAngle) * attackRadius
            );

            // Convert to world space and apply to target
            handTarget.position = zombieHand.position + arcOffset;

            yield return null;
        }

        // Return to original position
        armIKConstraint.weight = 0;
        handTarget.localPosition = originalHandPosition;
        isAttacking = false;
    }

    // Editor safety check
    void OnValidate()
    {
        if (handTarget == null)
        {
            Debug.LogWarning("Hand Target not assigned!", this);
        }
        if (zombieTransform == null)
        {
            zombieTransform = transform;
        }
    }
}
