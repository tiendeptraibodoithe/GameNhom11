using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    private bool isDead = false;
    private VaccineCollectionSystem missionSystem;

    private void Start()
    {
        missionSystem = FindObjectOfType<VaccineCollectionSystem>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Ngăn xử lý damage nếu đã chết

        BroadcastMessage("OnDamageTaken", damage);
        hitPoints -= damage;

        if (hitPoints <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Thông báo cho mission system khi zombie chết
        if (missionSystem != null)
        {
            missionSystem.OnZombieKilled();
        }

        // Delay destroy để đảm bảo không có collision/trigger events bị miss
        Destroy(gameObject, 0.1f);
    }
}