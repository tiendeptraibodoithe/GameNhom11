using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttack : MonoBehaviour
{
    PlayerHealth target;
    [SerializeField] float damage = 40f;

    [Header("Screen Flash Settings")]
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashInDuration = 0.05f; // Thời gian fade in ngắn hơn
    [SerializeField] private float flashOutDuration = 0.2f; // Thời gian fade out dài hơn
    [SerializeField] private float maxAlpha = 0.4f; // Độ trong suốt tối đa
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Đường cong animation

    [Header("Audio Settings")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField][Range(0f, 1f)] private float volumeScale = 0.7f;
    private AudioSource audioSource;

    private Coroutine currentFlashRoutine;

    void Start()
    {
        target = FindObjectOfType<PlayerHealth>();
        InitializeDamageFlash();
        InitializeAudio();
    }

    private void InitializeDamageFlash()
    {
        if (damageFlashImage != null)
        {
            Color color = damageFlashImage.color;
            color.a = 0;
            damageFlashImage.color = color;
        }
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Cấu hình AudioSource để phản hồi nhanh hơn
        audioSource.playOnAwake = false;
        audioSource.priority = 0; // Ưu tiên cao nhất
        audioSource.volume = volumeScale;
    }

    public void AttackHitEvent()
    {
        if (target == null) return;

        // Gọi damage trước
        target.PlayerTakeDamge(damage);

        // Phát âm thanh ngay lập tức
        PlayAttackSound();

        // Chạy hiệu ứng flash
        TriggerDamageFlash();

        Debug.Log("Bang Bang");
    }

    private void TriggerDamageFlash()
    {
        if (damageFlashImage == null) return;

        // Hủy coroutine đang chạy (nếu có)
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        // Bắt đầu coroutine mới
        currentFlashRoutine = StartCoroutine(FlashScreenImproved());
    }

    private IEnumerator FlashScreenImproved()
    {
        Color flashColor = damageFlashImage.color;
        float elapsedTime = 0f;

        // Fade In nhanh
        while (elapsedTime < flashInDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / flashInDuration;
            float curveValue = flashCurve.Evaluate(normalizedTime);
            flashColor.a = curveValue * maxAlpha;
            damageFlashImage.color = flashColor;
            yield return null;
        }

        // Đảm bảo đạt giá trị max
        flashColor.a = maxAlpha;
        damageFlashImage.color = flashColor;

        // Fade Out từ từ
        elapsedTime = 0f;
        while (elapsedTime < flashOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / flashOutDuration;
            float curveValue = flashCurve.Evaluate(1 - normalizedTime);
            flashColor.a = curveValue * maxAlpha;
            damageFlashImage.color = flashColor;
            yield return null;
        }

        // Đảm bảo kết thúc ở alpha = 0
        flashColor.a = 0;
        damageFlashImage.color = flashColor;
    }

    private void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // Thêm chút biến đổi
            audioSource.PlayOneShot(attackSound, volumeScale);
        }
    }

    // Thêm method để cleanup
    private void OnDisable()
    {
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        // Reset flash image
        if (damageFlashImage != null)
        {
            Color color = damageFlashImage.color;
            color.a = 0;
            damageFlashImage.color = color;
        }
    }
}