using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAnimator : WeaponAnimator
{
    // Tham chiếu đến Sniper Overlay Canvas và Camera
    public GameObject sniperOverlayCanvas;
    public GameObject sniperOverlayCamera;

    // Thời gian trì hoãn để bật/tắt canvas và camera
    public float zoomDelay = 0.3f;

    private Coroutine enableCanvasCoroutine;
    private Coroutine disableWeaponCoroutine;
    private Coroutine enableWeaponCoroutine;

    protected new void Start()
    {
        base.Start();
        // Đảm bảo canvas ban đầu bị tắt
        sniperOverlayCanvas.SetActive(false);
        sniperOverlayCamera.gameObject.SetActive(true);
    }

    public override void ZoomIn(bool zoomIn)
    {
        base.ZoomIn(zoomIn);
        if (zoomIn)
        {
            // Hủy các coroutine đang chạy nếu có
            if (enableWeaponCoroutine != null)
            {
                StopCoroutine(enableWeaponCoroutine);
                enableWeaponCoroutine = null;
            }

            // Bắt đầu coroutine để bật canvas sau một khoảng thời gian trì hoãn
            enableCanvasCoroutine = StartCoroutine(EnableCanvasAfterDelay(zoomDelay));
            disableWeaponCoroutine = StartCoroutine(DisableWeaponAfterDelay(zoomDelay));
        }
        else
        {
            // Hủy các coroutine đang chạy nếu có
            if (enableCanvasCoroutine != null)
            {
                StopCoroutine(enableCanvasCoroutine);
                enableCanvasCoroutine = null;
            }
            if (disableWeaponCoroutine != null)
            {
                StopCoroutine(disableWeaponCoroutine);
                disableWeaponCoroutine = null;
            }

            // Bắt đầu coroutine để bật lại vũ khí sau một khoảng thời gian trì hoãn
            enableWeaponCoroutine = StartCoroutine(EnableWeaponAfterDelay(zoomDelay));
            sniperOverlayCanvas.SetActive(false);
        }
    }

    private IEnumerator EnableCanvasAfterDelay(float delay)
    {
        // Chờ trong khoảng thời gian trì hoãn
        yield return new WaitForSeconds(delay);
        // Bật canvas
        sniperOverlayCanvas.SetActive(true);
    }

    private IEnumerator DisableWeaponAfterDelay(float delay)
    {
        // Chờ trong khoảng thời gian trì hoãn
        yield return new WaitForSeconds(delay);
        // Tắt camera vũ khí
        sniperOverlayCamera.gameObject.SetActive(false);
    }

    private IEnumerator EnableWeaponAfterDelay(float delay)
    {
        // Chờ trong khoảng thời gian trì hoãn
        yield return new WaitForSeconds(delay);
        // Bật lại camera vũ khí
        sniperOverlayCamera.gameObject.SetActive(true);
    }
}