using System;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] public int currentWeapon = 0;
    public event Action<int> OnWeaponChanged;
    private Weapon[] weapons;
    private Ammo ammoManager;

    void Start()
    {
        // Lấy các components
        weapons = new Weapon[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i).GetComponent<Weapon>();
        }

        // Lấy reference đến Ammo component
        ammoManager = GetComponentInParent<Ammo>();

        // Set vũ khí ban đầu và cập nhật UI
        SetWeaponActive();

        // Set loại đạn hiện tại cho Ammo system
        if (weapons != null && weapons[currentWeapon] != null)
        {
            ammoManager.SetCurrentAmmoType(weapons[currentWeapon].AmmoType);
        }
    }

    private void SetWeaponActive()
    {
        int weaponIndex = 0;
        foreach (Transform weapon in transform)
        {
            bool isActive = weaponIndex == currentWeapon;
            weapon.gameObject.SetActive(isActive);
            weaponIndex++;
        }
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        if (weapons != null && weapons[currentWeapon] != null)
        {
            var currentWeaponComponent = weapons[currentWeapon];
            // Set loại đạn hiện tại và cập nhật UI
            ammoManager.SetCurrentAmmoType(currentWeaponComponent.AmmoType);
        }
    }

    void Update()
    {
        int previousWeapon = currentWeapon;
        ProcessKeyInput();

        if (previousWeapon != currentWeapon)
        {
            SetWeaponActive();
            OnWeaponChanged?.Invoke(currentWeapon);
        }
    }

    private void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeapon = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentWeapon = 3;

        // Giới hạn currentWeapon trong phạm vi hợp lệ
        currentWeapon = Mathf.Clamp(currentWeapon, 0, weapons.Length - 1);
    }
}