using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] private AmmoType ammoType;  // Giữ SerializeField để có thể set trong Inspector
    public AmmoType AmmoType => ammoType;
    [SerializeField] float timeBetweenShots;
    [SerializeField] int pellets = 10;
    [SerializeField] float spreadAngle = 5f;
    [SerializeField] bool isShotgun = false;
    [SerializeField] Recoil recoil;
    [SerializeField] bool isAutomatic = false;
    [SerializeField] WeaponType weaponType;
    [SerializeField] GameObject fleshHitEffect;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shootSound;

    bool canShoot = true;
    bool isReloading = false;
 
    void Update()
    {
        // Thêm debug log để kiểm tra
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log($"{gameObject.name} - Current Ammo: {ammoSlot.GetCurrentAmmo(ammoType)}, Reserve Ammo: {ammoSlot.GetReserveAmmo(ammoType)}");
        }

        // Đơn giản hóa logic nạp đạn
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            // Kiểm tra xem còn đạn dự trữ không
            if (ammoSlot.GetReserveAmmo(ammoType) > 0)
            {
                StartCoroutine(ReloadWeapon());
                return;
            }
            else
            {
                Debug.Log($"{gameObject.name} - No reserve ammo!");
            }
        }

        // Xử lý bắn
        if (!isReloading)
        {
            if (isAutomatic)
            {
                if (Input.GetButton("Fire1") && canShoot && ammoSlot.GetCurrentAmmo(ammoType) > 0)
                {
                    StartCoroutine(Shoot());
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && canShoot && ammoSlot.GetCurrentAmmo(ammoType) > 0)
                {
                    StartCoroutine(Shoot());
                }
            }
        }
    }

    IEnumerator ReloadWeapon()
    {
        isReloading = true;
        Debug.Log($"Reloading {gameObject.name}...");

        yield return new WaitForSeconds(reloadTime);

        ammoSlot.ReloadAmmo(ammoType);
        //ammoSlot.UpdateAmmoUI(ammoType);

        isReloading = false;
        Debug.Log($"Reload complete for {gameObject.name}! Current Ammo: {ammoSlot.GetCurrentAmmo(ammoType)}");
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0)
        {
            PlayMuzzleFlash();
            PlayShootSound();
            ProcessRaycast();
            recoil.RecoilFire(weaponType);
            ammoSlot.ReduceCurrentAmmo(ammoType);
           // ammoSlot.UpdateAmmoUI(ammoType);

            // Thêm debug log sau khi bắn
            Debug.Log($"{gameObject.name} - Ammo remaining: {ammoSlot.GetCurrentAmmo(ammoType)}");
        }
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }




    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRaycast()
    {
        if (isShotgun)
        {
            for (int i = 0; i < pellets; i++)
            {
                Vector3 direction = FPCamera.transform.forward;
                direction.x += Random.Range(-spreadAngle, spreadAngle);
                direction.y += Random.Range(-spreadAngle, spreadAngle);
                RaycastHit hit;
                if (Physics.Raycast(FPCamera.transform.position, direction, out hit, range))
                {
                    CreateHitImpact(hit);
                    Debug.Log("I hit this thing: " + hit.transform.name);
                    EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                    if (target != null)
                    {
                        target.TakeDamage(damage);
                    }
                }
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
            {
                CreateHitImpact(hit);
                Debug.Log("I hit this thing: " + hit.transform.name);
                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            }
        }
    }

    private void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact;
        if (hit.transform.GetComponent<EnemyHealth>() != null)
        {
            impact = fleshHitEffect;
        }
        else
        {
            impact = hitEffect;
        }
        GameObject instantiatedImpact = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
        instantiatedImpact.transform.parent = hit.transform;
        Destroy(instantiatedImpact, 1);
    }
}