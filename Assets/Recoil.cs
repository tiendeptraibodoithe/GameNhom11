using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    // Các giá trị độ giật cho từng loại súng
    [SerializeField] private float pistolRecoilX;
    [SerializeField] private float pistolRecoilY;
    [SerializeField] private float pistolRecoilZ;

    [SerializeField] private float rifleRecoilX;
    [SerializeField] private float rifleRecoilY;
    [SerializeField] private float rifleRecoilZ;

    [SerializeField] private float shotgunRecoilX;
    [SerializeField] private float shotgunRecoilY;
    [SerializeField] private float shotgunRecoilZ;

    [SerializeField] private float sniperRecoilX;
    [SerializeField] private float sniperRecoilY;
    [SerializeField] private float sniperRecoilZ;

    void Start()
    {

    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                targetRotation += new Vector3(pistolRecoilX, Random.Range(-pistolRecoilY, pistolRecoilY), Random.Range(-pistolRecoilZ, pistolRecoilZ));
                break;
            case WeaponType.Rifle:
                targetRotation += new Vector3(rifleRecoilX, Random.Range(-rifleRecoilY, rifleRecoilY), Random.Range(-rifleRecoilZ, rifleRecoilZ));
                break;
            case WeaponType.Shotgun:
                targetRotation += new Vector3(shotgunRecoilX, Random.Range(-shotgunRecoilY, shotgunRecoilY), Random.Range(-shotgunRecoilZ, shotgunRecoilZ));
                break;
            case WeaponType.Sniper:
                targetRotation += new Vector3(sniperRecoilX, Random.Range(-sniperRecoilY, sniperRecoilY), Random.Range(-sniperRecoilZ, sniperRecoilZ));
                break;
        }
    }
}