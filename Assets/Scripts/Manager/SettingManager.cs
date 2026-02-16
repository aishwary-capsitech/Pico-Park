//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;

//public class SettingManager : MonoBehaviour
//{
//    public static SettingManager Instance;

//    public GameObject settingPanel;
//    public Button toggleAudioButton, toggleHapticButton, toggleChattingButton;

//    public bool isChattingEnabled = false;
//    public bool isHapticEnabled = false;
//    public bool isSettingPanelActive = false;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        int chatInt = PlayerPrefs.GetInt("IsChattingEnabled");
//        isChattingEnabled = chatInt == 1 ? true : false;
//        ToggleChatting();

//        int hapticInt = PlayerPrefs.GetInt("IsHapticEnabled");
//        isHapticEnabled = hapticInt == 1 ? true : false;
//        ToggleHaptic();
//    }

//    private void Start()
//    {
//        settingPanel.SetActive(isSettingPanelActive);
//    }

//    public void ToggleChatting()
//    {
//        isChattingEnabled = !isChattingEnabled;
//        Debug.Log("Chatting Enabled: " + isChattingEnabled);

//        toggleChattingButton.GetComponent<Image>().color = isChattingEnabled ? Color.white : Color.black;
//        var onOffImg = toggleChattingButton.transform.Find("ToggleRadioIcon").GetComponent<Image>();
//        var onOffImgPos = toggleChattingButton.transform.Find("ToggleRadioIcon").localPosition;
//        onOffImgPos = isChattingEnabled ? new Vector3(32f, onOffImgPos.y, 0) : new Vector3(-32f, onOffImgPos.y, 0);
//        onOffImg.transform.localPosition = onOffImgPos;
//        Debug.Log("ToggleChattingIcon: " + onOffImgPos);

//        PlayerPrefs.SetInt("IsChattingEnabled", isChattingEnabled ? 1 : 0);
//    }

//    public void ToggleHaptic()
//    {
//        isHapticEnabled = !isHapticEnabled;
//        Debug.Log("Haptic Enabled: " + isHapticEnabled);

//        toggleHapticButton.GetComponent<Image>().color = isHapticEnabled ? Color.white : Color.black;
//        var onOffImg = toggleHapticButton.transform.Find("ToggleRadioIcon").GetComponent<Image>();
//        var onOffImgPos = toggleHapticButton.transform.Find("ToggleRadioIcon").localPosition;
//        onOffImgPos = isHapticEnabled ? new Vector3(32f, onOffImgPos.y, 0) : new Vector3(-32f, onOffImgPos.y, 0);
//        onOffImg.transform.localPosition = onOffImgPos;
//        Debug.Log("ToggleHapticIcon: " + onOffImgPos);

//        PlayerPrefs.SetInt("IsHapticEnabled", isHapticEnabled ? 1 : 0);
//    }

//    public void ToggleSettingPanel()
//    {
//        isSettingPanelActive = !isSettingPanelActive;
//        settingPanel.SetActive(isSettingPanelActive);
//    }
//}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    public GameObject settingPanel;
    public Button toggleAudioButton, toggleHapticButton, toggleChattingButton;

    public bool isChattingEnabled = false;
    public bool isHapticEnabled = false;
    public bool isSettingPanelActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load from PlayerPrefs
        isChattingEnabled = PlayerPrefs.GetInt("IsChattingEnabled", 0) == 1;
        isHapticEnabled = PlayerPrefs.GetInt("IsHapticEnabled", 0) == 1;

        // Apply UI without toggling
        ApplyChattingUI();
        ApplyHapticUI();
    }

    private void Start()
    {
        settingPanel.SetActive(isSettingPanelActive);
    }

    // BUTTON CLICK FUNCTIONS

    public void ToggleChatting()
    {
        isChattingEnabled = !isChattingEnabled;

        PlayerPrefs.SetInt("IsChattingEnabled", isChattingEnabled ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Chating Enabled : " + isChattingEnabled);

        ApplyChattingUI();
    }

    public void ToggleHaptic()
    {
        isHapticEnabled = !isHapticEnabled;

        PlayerPrefs.SetInt("IsHapticEnabled", isHapticEnabled ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Haptic Enabled : " + isHapticEnabled);

        ApplyHapticUI();
    }

    public void ToggleSettingPanel()
    {
        isSettingPanelActive = !isSettingPanelActive;
        settingPanel.SetActive(isSettingPanelActive);
    }

    // UI APPLY METHODS

    private void ApplyChattingUI()
    {
        toggleChattingButton.GetComponent<Image>().color =
            isChattingEnabled ? Color.white : Color.black;

        var icon = toggleChattingButton.transform.Find("ToggleRadioIcon");
        Vector3 pos = icon.localPosition;

        pos.x = isChattingEnabled ? 32f : -32f;
        icon.localPosition = pos;
    }

    private void ApplyHapticUI()
    {
        toggleHapticButton.GetComponent<Image>().color =
            isHapticEnabled ? Color.white : Color.black;

        var icon = toggleHapticButton.transform.Find("ToggleRadioIcon");
        Vector3 pos = icon.localPosition;

        pos.x = isHapticEnabled ? 32f : -32f;
        icon.localPosition = pos;
    }
}
