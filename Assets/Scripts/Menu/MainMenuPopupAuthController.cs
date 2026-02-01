using UnityEngine;
using UnityEngine.UI;

public class MainMenuPopupAuthController : MonoBehaviour
{
    [SerializeField] private Button _btnRegister;
    [SerializeField] private Button _btnLogin;
    [SerializeField] private Button _btnClose;

    [SerializeField] private GameObject _registerLayout;
    [SerializeField] private GameObject _loginLayout;
    [SerializeField] private GameObject _authPopup;

    private void Start()
    {
        _btnRegister.onClick.AddListener(OnBtnRegisterClick);
        _btnLogin.onClick.AddListener(OnBtnLoginClick);
        _btnClose.onClick.AddListener(OnBtnCloseClick);
        ShowLoginTab();
    }

    private void OnBtnRegisterClick()
    {
        if (!_registerLayout.activeSelf)
        {
            ShowRegisterTab();
        }
        else
        {
            Debug.Log("Đang gửi đăng ký...");
            // Gọi hàm bên AccountShopManager
            if (AccountShopManager.Instance != null)
            {
                AccountShopManager.Instance.OnClickBtnRegister();
            }
        }
    }

    private void OnBtnLoginClick()
    {
        if (!_loginLayout.activeSelf)
        {
            ShowLoginTab();
        }
        else
        {
            Debug.Log("Đang gửi đăng nhập...");
            if (AccountShopManager.Instance != null)
            {
                AccountShopManager.Instance.OnClickBtnLogin();
            }
        }
    }

    private void ShowRegisterTab()
    {
        _registerLayout.SetActive(true);
        _loginLayout.SetActive(false);
    }

    private void ShowLoginTab()
    {
        _registerLayout.SetActive(false);
        _loginLayout.SetActive(true);
    }

    private void OnBtnCloseClick()
    {
        _authPopup.SetActive(false);
    }
}