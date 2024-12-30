using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class WeaponZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float zoomedInFieldOfView = 20.0f;
    [SerializeField] private float sniperZoomedInFieldOfView = 10.0f;
    [SerializeField] private float zoomedMouseSensitivityFactor = 0.1f;

    private WeaponAnimator weaponAnimator;
    private FirstPersonController fpsController;
    private float baseFieldOfView;
    private bool isZoomed = false;

    private WeaponSwitcher weaponSwitcher;

    private void Start()
    {
        baseFieldOfView = virtualCamera.m_Lens.FieldOfView;
        fpsController = FindObjectOfType<FirstPersonController>();

        weaponSwitcher = FindObjectOfType<WeaponSwitcher>();
        if (weaponSwitcher != null)
        {
            weaponSwitcher.OnWeaponChanged += UpdateActiveWeapon; // Đăng ký sự kiện
        }

        UpdateActiveWeapon(weaponSwitcher != null ? weaponSwitcher.currentWeapon : 0);
    }

    private void OnDestroy()
    {
        if (weaponSwitcher != null)
        {
            weaponSwitcher.OnWeaponChanged -= UpdateActiveWeapon; // Hủy đăng ký sự kiện
        }
    }

    private void UpdateActiveWeapon(int weaponIndex)
    {
        Transform weaponsTransform = weaponSwitcher.transform;
        Debug.Log("Updating active weapon for index: " + weaponIndex);

        if (weaponsTransform != null)
        {
            foreach (Transform weapon in weaponsTransform)
            {
                if (weapon.gameObject.activeInHierarchy)
                {
                    weaponAnimator = weapon.GetComponent<WeaponAnimator>();
                    Debug.Log("WeaponAnimator found: " + (weaponAnimator != null));
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (weaponAnimator == null)
        {
            UpdateActiveWeapon(weaponSwitcher != null ? weaponSwitcher.currentWeapon : 0);
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            StopAllCoroutines();
            StartCoroutine(ZoomAnim(true));
            isZoomed = true;
            AdjustMouseSensitivity();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopAllCoroutines();
            StartCoroutine(ZoomAnim(false));
            isZoomed = false;
            AdjustMouseSensitivity();
        }
    }

    private IEnumerator ZoomAnim(bool zoomIn)
    {
        weaponAnimator.ZoomIn(zoomIn);
        float animTime = weaponAnimator.ZoomAnimTime;
        float elapsed = 0f;

        float startFieldOfView = virtualCamera.m_Lens.FieldOfView;
        float endFieldOfView = zoomIn ? GetZoomedInFieldOfView() : baseFieldOfView;

        while (elapsed < animTime)
        {
            float currentFieldOfView = Mathf.Lerp(startFieldOfView, endFieldOfView, elapsed / animTime);
            virtualCamera.m_Lens.FieldOfView = currentFieldOfView;
            elapsed += Time.deltaTime;
            yield return null;
        }
        virtualCamera.m_Lens.FieldOfView = endFieldOfView;
    }

    private float GetZoomedInFieldOfView()
    {
        if (weaponAnimator is SniperAnimator)
        {
            return sniperZoomedInFieldOfView;
        }
        return zoomedInFieldOfView;
    }

    private void AdjustMouseSensitivity()
    {
        if (fpsController != null)
        {
            float currentSensitivityFactor = isZoomed ? zoomedMouseSensitivityFactor : 1.0f;
            fpsController.SetMouseSensitivityFactor(currentSensitivityFactor);
        }
    }
}
