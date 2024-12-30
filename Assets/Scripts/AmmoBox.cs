using UnityEngine;
public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 30; // Số lượng đạn cơ bản khi nhặt
    private Ammo playerAmmo;

    void Start()
    {
        playerAmmo = FindObjectOfType<Ammo>();
        if (playerAmmo == null)
        {
            Debug.LogWarning("Player's Ammo component not found.");
        }
    }

    public void Collect()
    {
        if (playerAmmo == null)
        {
            Debug.LogWarning("Ammo system is not found!");
            return;
        }

        // Thêm đạn cho tất cả các loại súng
        playerAmmo.AddAmmoToAllTypes(ammoAmount);
        Debug.Log($"Collected ammo box - Added ammo to all weapon types");
        Destroy(gameObject);
    }
}