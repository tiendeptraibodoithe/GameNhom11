using UnityEngine;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    public Text totalBullet;  // Hiển thị đạn dự trữ
    public Text currentBullet; // Hiển thị đạn hiện tại
    [SerializeField] private AmmoSlot[] ammoSlots;
    private AmmoType currentActiveAmmoType;

    public void SetCurrentAmmoType(AmmoType type)
    {
        currentActiveAmmoType = type;
        UpdateAmmoUI(type); // Cập nhật UI ngay khi đổi loại đạn
    }


    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType type;
        public int ammoAmount;      // Đạn trong băng hiện tại
        public int maxAmmo = 30;    // Số đạn tối đa trong một băng đạn
        public int reserveAmmo = 90; // Đạn dự trữ
    }

    private void UpdateUI(AmmoSlot slot)
    {
        if (slot != null && currentBullet != null && totalBullet != null)
        {
            currentBullet.text = slot.ammoAmount.ToString();
            totalBullet.text = slot.reserveAmmo.ToString();
        }
    }

    public void UpdateAmmoUI(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null)
        {
            currentActiveAmmoType = ammoType; // Cập nhật loại đạn active
            UpdateUI(slot);
        }
    }

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        return slot != null ? slot.ammoAmount : 0;
    }

    public int GetMaxAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        return slot != null ? slot.maxAmmo : 0;
    }

    public int GetReserveAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        return slot != null ? slot.reserveAmmo : 0;
    }

    public void ReduceCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null)
        {
            slot.ammoAmount--;
            UpdateUI(slot);
        }
    }

    public void ReloadAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null && slot.reserveAmmo > 0)
        {
            int ammoNeeded = slot.maxAmmo - slot.ammoAmount;
            int ammoToAdd = Mathf.Min(ammoNeeded, slot.reserveAmmo);
            slot.ammoAmount += ammoToAdd;
            slot.reserveAmmo -= ammoToAdd;
            Debug.Log($"Reloaded {ammoToAdd} ammo. Remaining reserve: {slot.reserveAmmo}");
            UpdateUI(slot);
        }
    }

    public bool NeedsReload(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot == null) return false;
        return slot.ammoAmount < slot.maxAmmo && slot.reserveAmmo > 0;
    }

    public bool HasAmmoToReload(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        return slot != null && slot.reserveAmmo > 0;
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.type == ammoType) return slot;
        }
        return null;
    }
    public void AddAmmoToAllTypes(int baseAmount)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            int additionalAmmo = CalculateAmmoAmount(slot.type, baseAmount);
            int previousReserveAmmo = slot.reserveAmmo;
            slot.reserveAmmo = Mathf.Min(slot.reserveAmmo + additionalAmmo, slot.maxAmmo * 3);

            Debug.Log($"Added {additionalAmmo} ammo to {slot.type}. " +
                     $"Previous reserve: {previousReserveAmmo}, New reserve: {slot.reserveAmmo}");
        }

        // Cập nhật UI cho loại đạn đang active sau khi thêm đạn
        UpdateAmmoUI(currentActiveAmmoType);
    }

    // Phương thức mới để tính toán số đạn được thêm cho mỗi loại súng
    private int CalculateAmmoAmount(AmmoType ammoType, int baseAmount)
    {
        // Hệ số nhân cho mỗi loại súng
        float multiplier = 1.0f;

        switch (ammoType)
        {
            case AmmoType.Pistol:
                multiplier = 0.4f;  // 40% của baseAmount
                break;
            case AmmoType.Rifle:
                multiplier = 0.8f;  // 80% của baseAmount
                break;
            case AmmoType.Shotgun:
                multiplier = 0.3f;  // 30% của baseAmount
                break;
            case AmmoType.Sniper:
                multiplier = 0.2f;  // 20% của baseAmount
                break;
        }

        return Mathf.RoundToInt(baseAmount * multiplier);
    }

    // Có thể xóa hoặc đánh dấu Obsolete phương thức AddAmmo cũ
    [System.Obsolete("Use AddAmmoToAllTypes instead")]
    public void AddAmmo(AmmoType ammoType, int amount)
    {
        AddAmmoToAllTypes(amount);
    }

}