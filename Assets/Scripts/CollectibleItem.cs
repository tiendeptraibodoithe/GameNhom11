using UnityEngine;
using UnityEngine.UI;

public class CollectibleItem : MonoBehaviour
{
    public Text pickupText; // Tham chiếu đến Text UI để hiển thị thông báo

    private void Start()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false); // Ẩn thông báo ban đầu
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(true); // Hiển thị thông báo khi người chơi vào phạm vi
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false); // Ẩn thông báo khi người chơi ra khỏi phạm vi
            }
        }
    }
}
