using UnityEngine;

public class VaccineBeacon : MonoBehaviour
{
    [Header("Beacon Settings")]
    [SerializeField] private GameObject beaconLight;
    [SerializeField] private float beaconHeight = 10f;
    [SerializeField] private Color beaconColor = new Color(0f, 1f, 0f, 0.5f); // Màu xanh lá semi-transparent
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float beaconRadius = 2f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeBeacon = 120f; // 2 phút trước khi hiện cột sáng

    private float timer = 0f;
    private bool isBeaconActive = false;

    private void Start()
    {
        // Tạo cột sáng
        CreateBeacon();
        // Ẩn cột sáng khi bắt đầu
        beaconLight.SetActive(false);
    }

    private void CreateBeacon()
    {
        // Tạo cylinder để làm cột sáng
        beaconLight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beaconLight.transform.parent = transform;

        // Định vị cột sáng
        beaconLight.transform.localPosition = new Vector3(0, 0, 0);
        beaconLight.transform.localScale = new Vector3(beaconRadius, beaconHeight / 2f, beaconRadius);

        // Tạo material phát sáng cho cột sáng
        Material beaconMaterial = new Material(Shader.Find("Standard"));
        beaconMaterial.SetFloat("_Mode", 3); // Transparent mode
        beaconMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        beaconMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        beaconMaterial.SetInt("_ZWrite", 0);
        beaconMaterial.DisableKeyword("_ALPHATEST_ON");
        beaconMaterial.EnableKeyword("_ALPHABLEND_ON");
        beaconMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        beaconMaterial.renderQueue = 3000;

        // Thiết lập màu và độ trong suốt
        beaconMaterial.SetColor("_Color", beaconColor);
        beaconMaterial.EnableKeyword("_EMISSION");
        beaconMaterial.SetColor("_EmissionColor", beaconColor * 2f);

        // Gán material cho cột sáng
        beaconLight.GetComponent<MeshRenderer>().material = beaconMaterial;

        // Xóa Collider vì không cần
        Destroy(beaconLight.GetComponent<CapsuleCollider>());
    }

    private void Update()
    {
        // Đếm thời gian
        if (!isBeaconActive)
        {
            timer += Time.deltaTime;
            if (timer >= delayBeforeBeacon)
            {
                ActivateBeacon();
            }
        }
        else
        {
            // Xoay cột sáng
            beaconLight.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }

    private void ActivateBeacon()
    {
        isBeaconActive = true;
        beaconLight.SetActive(true);
    }

    public void DeactivateBeacon()
    {
        isBeaconActive = false;
        beaconLight.SetActive(false);
    }

    private void OnDestroy()
    {
        // Đảm bảo cleanup khi object bị hủy
        if (beaconLight != null)
        {
            Destroy(beaconLight);
        }
    }
}