using UnityEngine;

public class FirstAidPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f; // Lượng máu hồi

    public void Collect(PlayerHealth playerHealth)
    {
        if (playerHealth != null)
        {
            Debug.Log("Healing player");
            playerHealth.Heal(healAmount); // Hồi máu cho nhân vật
            Destroy(gameObject); // Xoá FirstAid khỏi scene
        }
        else
        {
            Debug.Log("PlayerHealth is null");
        }
    }
}