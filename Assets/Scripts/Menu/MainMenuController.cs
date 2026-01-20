using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Button _buttonContinue;

    [SerializeField]
    private Button _buttonNewGame;

    [SerializeField]
    private Button _buttonShop;

    [SerializeField]
    private Button _buttonOptions;

    [SerializeField]
    private Button _buttonExit;

    [SerializeField]
    private Button _buttonOnlineCoop;

    [SerializeField]
    private Button[] _lsButtonBack;

    [SerializeField]
    private GameObject _mainLayout;

    [SerializeField]
    private GameObject _lobbyLayout;

    private const float ButtonHoverTargetX = 25;
    private const float ButtonHoverTransitionDuration = 0.25f;

    private void Awake()
    {
        if (!_buttonContinue.interactable)
        {
            TMP_Text btnText = _buttonContinue.GetComponentInChildren<TMP_Text>();
            btnText.color = Color.grey;
        }
        else
        {
            AddHoverEffect(_buttonContinue);
        }

        AddHoverEffect(_buttonNewGame);
        AddHoverEffect(_buttonShop);
        AddHoverEffect(_buttonOptions);
        AddHoverEffect(_buttonExit);
        AddHoverEffect(_buttonOnlineCoop);

        _buttonNewGame.onClick.AddListener(OnNewGameButtonClick);

        foreach (Button btnBack in _lsButtonBack)
        {
            AddHoverEffect(btnBack);
            btnBack.onClick.AddListener(OnBackButtonClick);
        }
    }

    private void OnNewGameButtonClick()
    {
        _mainLayout.SetActive(false);
        _lobbyLayout.SetActive(true);

        foreach (TMP_Text text in _mainLayout.GetComponentsInChildren<TMP_Text>())
        {
            DOTween.Kill(text);
            text.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void OnBackButtonClick()
    {
        _mainLayout.SetActive(true);
        _lobbyLayout.SetActive(false);

        foreach (TMP_Text text in _lobbyLayout.GetComponentsInChildren<TMP_Text>())
        {
            DOTween.Kill(text);
            text.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void AddHoverEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        TMP_Text btnText = button.GetComponentInChildren<TMP_Text>();

        EventTrigger.Entry pointerEnterEntry = new()
        {
            eventID = EventTriggerType.PointerEnter,
        };
        pointerEnterEntry.callback.AddListener((_) => OnButtonHoverIn(btnText));

        EventTrigger.Entry pointerExitEntry = new()
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExitEntry.callback.AddListener((_) => OnButtonHoverOut(btnText));

        trigger.triggers.Add(pointerEnterEntry);
        trigger.triggers.Add(pointerExitEntry);
    }

    private void OnButtonHoverIn(TMP_Text btnText)
    {
        btnText.rectTransform.DOAnchorPosX(ButtonHoverTargetX, ButtonHoverTransitionDuration).SetId(btnText);
    }

    private void OnButtonHoverOut(TMP_Text btnText)
    {
        btnText.rectTransform.DOAnchorPosX(0, ButtonHoverTransitionDuration).SetId(btnText);
    }
}
