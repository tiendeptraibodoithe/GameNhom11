using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VaccineCollectionSystem : MonoBehaviour
{
    [Header("Vaccine Mission")]
    [SerializeField] private int requiredVaccines = 3;
    private int collectedVaccines = 0;
    private HashSet<int> collectedVaccineIds = new HashSet<int>();

    [Header("Zombie Mission")]
    [SerializeField] private int requiredZombieKills = 3;
    private int zombieKillCount = 0;

    [Header("UI Elements")]
    [SerializeField] private Text vaccineCountText;
    [SerializeField] private Text zombieCountText;
    [SerializeField] private GameObject missionCompleteUI;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    [Header("Player Settings")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerCamera; // Reference to player camera controller if you have one

    [SerializeField] private PlayerHealth playerHealth;
    public Ammo ammo;

    private bool isMissionComplete = false;

    private void Awake()
    {
        // Đảm bảo TimeScale được set đúng ngay khi object được tạo
        ResetGameState();
    }

    private void Start()
    {
        // Khởi tạo UI và game state
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Reset game state để đảm bảo
        ResetGameState();

        // Setup UI
        missionCompleteUI.SetActive(false);
        UpdateVaccineUI();
        UpdateZombieUI();

        // Setup button listeners
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(LoadNextLevel);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        // Lock cursor
        SetCursorState(false);

        Debug.Log($"Game initialized with TimeScale: {Time.timeScale}");
    }

    private void Update()
    {
        // Chỉ cho phép pickup khi game đang chạy
        if (!isMissionComplete && Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckForItemPickup();
            }
        }

        // Escape chỉ hoạt động khi mission complete
        if (Input.GetKeyDown(KeyCode.Escape) && isMissionComplete)
        {
            QuitGame();
        }
    }

    private void CheckForItemPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f); // Tăng phạm vi nếu cần
        Debug.Log($"Số vật phẩm trong phạm vi: {hitColliders.Length}");
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Phát hiện object: {hitCollider.name}, Tag: {hitCollider.tag}");

            if (hitCollider.CompareTag("Vaccine"))
            {
                // Kiểm tra ID của vaccine
                int vaccineId = hitCollider.gameObject.GetInstanceID();
                if (!collectedVaccineIds.Contains(vaccineId))
                {
                    CollectVaccine(hitCollider.gameObject);
                    collectedVaccineIds.Add(vaccineId);
                    Debug.Log($"Collected vaccine ID: {vaccineId}, Total collected: {collectedVaccines}");
                }
                else
                {
                    Debug.Log($"Vaccine ID: {vaccineId} đã được thu thập trước đó");
                }
            }
            else if (hitCollider.CompareTag("FirstAid"))
            {
                FirstAidPickup firstAid = hitCollider.GetComponent<FirstAidPickup>();
                if (firstAid != null)
                {
                    firstAid.Collect(playerHealth);
                }
            }
            else if (hitCollider.CompareTag("AmmoBox")) // Kiểm tra tag AmmoBox
            {
                AmmoBox ammoBox = hitCollider.GetComponent<AmmoBox>();
                if (ammoBox != null) // Kiểm tra ammoBox không phải null
                {
                    ammoBox.Collect(); // Gọi phương thức Collect của AmmoBox mà không truyền tham số
                }
                else
                {
                    Debug.LogWarning("AmmoBox is null!"); // Thông báo nếu ammoBox là null
                }
            }
        }
    }

    private void CollectVaccine(GameObject vaccineObject)
    {
        VaccineBeacon beacon = vaccineObject.GetComponent<VaccineBeacon>();
        if (beacon != null)
        {
            beacon.DeactivateBeacon();
        }

        collectedVaccines++;
        UpdateVaccineUI();
        Debug.Log($"Collected vaccine. Total: {collectedVaccines}");

        Destroy(vaccineObject);
        CheckMissionCompletion();
    }

    public void OnZombieKilled()
    {
        zombieKillCount++;
        UpdateZombieUI();
        CheckMissionCompletion();
    }

    private void UpdateVaccineUI()
    {
        if (vaccineCountText != null)
        {
            vaccineCountText.text = $"Vaccines: {collectedVaccines}/{requiredVaccines}";
        }
    }

    private void UpdateZombieUI()
    {
        if (zombieCountText != null)
        {
            zombieCountText.text = $"Zombies Killed: {zombieKillCount}/{requiredZombieKills}";
        }
    }

    private void CheckMissionCompletion()
    {
        if (collectedVaccines >= requiredVaccines && zombieKillCount >= requiredZombieKills)
        {
            CompleteMission();
        }
    }

    private void CompleteMission()
    {
        isMissionComplete = true;

        // Show UI
        if (missionCompleteUI != null)
        {
            missionCompleteUI.SetActive(true);
        }

        // Show cursor
        SetCursorState(true);

        // Stop game
        Time.timeScale = 0;

        // Disable player controls
        DisablePlayerControls();

        Debug.Log("Mission Complete - TimeScale set to 0");
    }
    private void ResetVaccineCollection()
    {
        collectedVaccines = 0;
        collectedVaccineIds.Clear();
        UpdateVaccineUI();
    }
    private void LoadNextLevel()
    {
        ResetGameState();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex >= totalScenes - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    private void QuitGame()
    {
        ResetGameState();
        SceneManager.LoadScene(0);
    }

    private void ResetGameState()
    {
        Time.timeScale = 1f;
        isMissionComplete = false;
        SetCursorState(false);
        EnablePlayerControls();
        ResetVaccineCollection(); // Thêm reset vaccine collection
        Debug.Log($"Game state reset - TimeScale: {Time.timeScale}");
    }

    private void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void EnablePlayerControls()
    {
        if (playerController != null) playerController.enabled = true;
        if (playerCamera != null) playerCamera.enabled = true;
    }

    private void DisablePlayerControls()
    {
        if (playerController != null) playerController.enabled = false;
        if (playerCamera != null) playerCamera.enabled = false;
    }

    private void OnDestroy()
    {
        // Cleanup khi object bị hủy
        ResetGameState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}