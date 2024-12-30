using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera FPCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject hitEffect;

    [Header("Melee Settings")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeDamage = 50f;
    [SerializeField] private float timeBetweenMeleeAttacks = 1f;
    [SerializeField] private float comboResetTime = 1.5f;

    private bool canMelee = true;
    private int attackIndex = 0;
    private float lastAttackTime = 0f;

    // Cache animator parameter hashes
    private readonly int attackIndexHash = Animator.StringToHash("attackIndex");
    private readonly int attackTriggerHash = Animator.StringToHash("attack");

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component is missing! Please assign it in the inspector.");
            enabled = false;
            return;
        }

        // Ensure we start in idle state
        animator.SetInteger(attackIndexHash, 0);
    }

    private void Update()
    {
        // Reset combo if too much time has passed
        if (Time.time - lastAttackTime > comboResetTime)
        {
            if (attackIndex != 0)
            {
                attackIndex = 0;
                animator.SetInteger(attackIndexHash, attackIndex);
                Debug.Log("Combo reset due to timeout");
            }
        }

        // Check for attack input
        if (Input.GetButtonDown("Fire1") && canMelee)
        {
            PerformMeleeAttack();
        }

        // Debug log to monitor state
        Debug.Log($"Current State - Attack Index: {attackIndex}, Can Melee: {canMelee}, Time since last attack: {Time.time - lastAttackTime}");
    }

    private void PerformMeleeAttack()
    {
        canMelee = false;
        lastAttackTime = Time.time;

        // Increment attack index
        attackIndex++;
        if (attackIndex > 3)
        {
            attackIndex = 1;
        }

        Debug.Log($"Performing Attack {attackIndex}");

        // Set animator parameters - Order is important!
        animator.SetInteger(attackIndexHash, attackIndex);
        animator.SetTrigger(attackTriggerHash);

        // Check for hits
        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, meleeRange))
        {
            Debug.Log($"Hit target: {hit.transform.name}");
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                target.TakeDamage(meleeDamage);
            }
            CreateHitImpact(hit);
        }

        StartCoroutine(ResetAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        // Wait for the current attack animation to finish
        yield return new WaitForSeconds(timeBetweenMeleeAttacks);
        canMelee = true;
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        impact.transform.parent = hit.transform;
        Destroy(impact, 1f);
    }

    // Animation Event callback - Add this to your animation clips
    public void OnAttackComplete()
    {
        Debug.Log($"Attack {attackIndex} animation completed");
        // You can add additional logic here that needs to run when an attack animation completes
    }
}