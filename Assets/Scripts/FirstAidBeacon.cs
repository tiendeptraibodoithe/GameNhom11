using UnityEngine;

public class FirstAidBeacon : MonoBehaviour
{
    [Header("Beacon Settings")]
    [SerializeField] private GameObject beaconArea;
    [SerializeField] private float beaconRadius = 5f; // Bán kính của khu vực beacon
    [SerializeField] private float beaconHeight = 10f; // Chiều cao của khu vực beacon
    [SerializeField] private Color beaconColor = new Color(0f, 1f, 0f, 0.5f); // Màu xanh lá semi-transparent
    [SerializeField] private float rotateSpeed = 50f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeBeacon = 120f; // 2 phút trước khi hiện khu vực beacon

    private float timer = 0f;
    private bool isBeaconActive = false;

    private void Start()
    {
        // Tạo khu vực beacon
        CreateBeaconArea();
        // Ẩn khu vực beacon khi bắt đầu
        beaconArea.SetActive(false);
    }

    private void CreateBeaconArea()
    {
        // Tạo vùng beacon hình trụ
        beaconArea = new GameObject("BeaconArea");
        beaconArea.transform.parent = transform;

        // Tạo cylinder để làm vùng sáng
        GameObject beaconCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beaconCylinder.transform.parent = beaconArea.transform;

        // Định vị vị trí và kích thước của cylinder
        beaconCylinder.transform.localPosition = new Vector3(0, beaconHeight / 2f, 0);
        beaconCylinder.transform.localScale = new Vector3(beaconRadius, beaconHeight / 2f, beaconRadius);

        // Tạo material cho khu vực beacon (không phát sáng)
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

        // Gán material cho cylinder
        beaconCylinder.GetComponent<MeshRenderer>().material = beaconMaterial;

        // Xóa collider vì không cần
        Destroy(beaconCylinder.GetComponent<CapsuleCollider>());
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
            // Xoay khu vực beacon
            beaconArea.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }

    private void ActivateBeacon()
    {
        isBeaconActive = true;
        beaconArea.SetActive(true);
    }

    public void DeactivateBeacon()
    {
        isBeaconActive = false;
        beaconArea.SetActive(false);
    }

    private void OnDestroy()
    {
        // Đảm bảo cleanup khi object bị hủy
        if (beaconArea != null)
        {
            Destroy(beaconArea);
        }
    }
}
