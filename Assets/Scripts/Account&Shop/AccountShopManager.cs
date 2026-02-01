using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AccountShopManager : MonoBehaviour
{
    public static AccountShopManager Instance;

    [Header("--- CẤU HÌNH SERVER ---")]
    public string baseUrl = "https://your-render-app.onrender.com"; // Giữ nguyên link của bạn

    [Header("--- UI: AUTH LAYOUT ---")]
    [Tooltip("Kéo cái GameObject 'Layout' (nằm trong AuthLayout) vào đây")]
    public GameObject loggedInLayout; // <--- CÁI MỚI THÊM VÀO

    [Tooltip("Kéo cái Text (TMP) nằm trong AuthLayout > Layout vào đây")]
    public TextMeshProUGUI txtUsernameDisplay;

    [Tooltip("Kéo GameObject cha PopupAuth vào để tắt nó khi đăng nhập xong")]
    public GameObject popupAuthObject;

    [Header("--- UI: INPUT LOGIN ---")]
    public TMP_InputField inpLoginUser;
    public TMP_InputField inpLoginPass;

    [Header("--- UI: INPUT REGISTER ---")]
    public TMP_InputField inpRegisterUser;
    public TMP_InputField inpRegisterPass;
    public TMP_InputField inpRegisterConfirm;

    [Header("--- UI: SHOP ---")]
    public TextMeshProUGUI txtCoinDisplay;

    private string currentUserId = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (txtUsernameDisplay) txtUsernameDisplay.text = "Login / Register";
        if (txtCoinDisplay) txtCoinDisplay.text = "0";

        // Mặc định ẩn cái Layout chứa tên đi (nếu muốn)
        // if(loggedInLayout) loggedInLayout.SetActive(false); 
    }

    // --- CÁC HÀM SỰ KIỆN NÚT BẤM ---

    public void OnClickBtnLogin()
    {
        StartCoroutine(LoginRoutine(inpLoginUser.text, inpLoginPass.text));
    }

    public void OnClickBtnRegister()
    {
        StartCoroutine(RegisterRoutine(inpRegisterUser.text, inpRegisterPass.text, inpRegisterConfirm.text));
    }

    public void OnClickBtnBuyCoin()
    {
        // Kiểm tra xem đã đăng nhập trong game chưa
        if (!string.IsNullOrEmpty(currentUserId))
        {
            string urlWithToken = baseUrl + "/?quickLogin=" + currentUserId;

            Debug.Log("Đang mở Web Shop với Token đăng nhập nhanh: " + urlWithToken);
            Application.OpenURL(urlWithToken);
        }
        else
        {
            // Nếu chưa đăng nhập trong game thì mở trang login bình thường
            Debug.LogWarning("Chưa đăng nhập trong game!");
            Application.OpenURL(baseUrl + "/login");
        }
    }

    // --- LOGIC XỬ LÝ ---

    void HandleLoginSuccess(UserData user)
    {
        currentUserId = user._id;

        // 1. Bật cái Layout chứa Text lên (QUAN TRỌNG)
        if (loggedInLayout != null)
        {
            loggedInLayout.SetActive(true); // <--- BẬT NÓ LÊN Ở ĐÂY
        }

        // 2. Cập nhật Text
        if (txtUsernameDisplay != null)
        {
            txtUsernameDisplay.text = "Hi " + user.username;
        }

        // 3. Cập nhật Coin
        if (txtCoinDisplay != null)
        {
            txtCoinDisplay.text = "Coin: " + user.coin.ToString("N0");
        }

        // 4. Tắt Popup Auth
        if (popupAuthObject) popupAuthObject.SetActive(false);

        Debug.Log($"✅ Đã đổi tên hiển thị thành: Hi {user.username}");
    }

    // --- GIỮ NGUYÊN PHẦN API ---
    IEnumerator LoginRoutine(string username, string password)
    {
        string url = baseUrl + "/api/login";
        var data = new { username = username, password = password };
        string jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = CreateRequest(url, jsonData))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);
                if (response.success) HandleLoginSuccess(response.data);
                else Debug.LogError(response.message);
            }
            else Debug.LogError("Lỗi mạng: " + request.error + " " + request.downloadHandler.text);
        }
    }

    IEnumerator RegisterRoutine(string username, string password, string confirm)
    {
        string url = baseUrl + "/api/register";
        var data = new { username = username, password = password, passwordConfirm = confirm };
        string jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = CreateRequest(url, jsonData))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);
                if (response.success) HandleLoginSuccess(response.data);
                else Debug.LogError(response.message);
            }
            else Debug.LogError("Lỗi mạng: " + request.error);
        }
    }

    UnityWebRequest CreateRequest(string url, string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    // Hàm được gọi khi quay lại game để cập nhật coin
    public void RefreshUserData()
    {
        if (!string.IsNullOrEmpty(currentUserId))
        {
            StartCoroutine(GetUserInfoRoutine(currentUserId));
        }
    }

    // Coroutine gọi API lấy thông tin user mới nhất
    IEnumerator GetUserInfoRoutine(string userId)
    {
        string url = baseUrl + "/api/user/" + userId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    // Cập nhật lại số coin mới nhất lên màn hình
                    if (txtCoinDisplay)
                    {
                        txtCoinDisplay.text = "Coin: " + response.data.coin.ToString("N0");
                    }
                }
            }
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        // hasFocus = true nghĩa là người chơi vừa quay lại cửa sổ game
        if (hasFocus)
        {
            Debug.Log("Người chơi đã quay lại game. Đang cập nhật Coin...");
            RefreshUserData();
        }
    }

    public class ApiResponse { public bool success; public string message; public UserData data; }
    public class UserData { public string _id; public string username; public int coin; }
}