using UnityEngine;
using UnityEngine.UI;

public class MainMenuPopupShopController : MonoBehaviour
{
    [Header("--- UI References ---")]
    [Tooltip("Kéo GameObject 'PopupShop' vào đây")]
    public GameObject popupShop;

    [Tooltip("Kéo nút 'BtnClose' nằm trong PopupShop vào đây")]
    public Button btnClose;

    [Tooltip("Kéo nút 'Shop' (nằm ở MainLayout hoặc nơi bạn đặt nút mở shop) vào đây")]
    public Button btnOpenShop;

    private void Start()
    {
        // 1. Đảm bảo khi game bắt đầu thì PopupShop ẩn đi
        if (popupShop != null)
        {
            popupShop.SetActive(false);
        }

        // 2. Tự động gán sự kiện click cho nút Đóng (nếu đã kéo vào)
        if (btnClose != null)
        {
            btnClose.onClick.AddListener(CloseShop);
        }

        // 3. Tự động gán sự kiện click cho nút Mở Shop (nếu đã kéo vào)
        if (btnOpenShop != null)
        {
            btnOpenShop.onClick.AddListener(OpenShop);
        }
    }

    // Hàm này dùng để Mở Shop
    public void OpenShop()
    {
        if (popupShop != null)
        {
            popupShop.SetActive(true);
            // Sau này bạn có thể thêm code lấy dữ liệu Coin ở đây
            Debug.Log("Đã mở Shop");
        }
    }

    // Hàm này dùng để Đóng Shop
    public void CloseShop()
    {
        if (popupShop != null)
        {
            popupShop.SetActive(false);
            Debug.Log("Đã đóng Shop");
        }
    }
}