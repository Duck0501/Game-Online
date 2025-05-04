using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    public InputField nameInputField; // Gán InputField từ Inspector
    public Button submitNameButton; // Gán Button từ Inspector
    public Text playerNameText; // Gán Text để hiển thị tên
    public Text successMessageText; // Gán Text để hiển thị thông báo thành công (tùy chọn)
    public CanvasGroup nameInputCanvas; // Gán CanvasGroup để bật/tắt giao diện nhập tên

    void Start()
    {
        // Đảm bảo giao diện nhập tên hiển thị ban đầu
        if (nameInputCanvas != null)
        {
            nameInputCanvas.alpha = 1;
            nameInputCanvas.interactable = true;
            nameInputCanvas.blocksRaycasts = true;
        }

        // Kiểm tra các thành phần UI
        if (nameInputField == null || submitNameButton == null || playerNameText == null || nameInputCanvas == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần UI chưa được gán trong Inspector!");
            return;
        }

        // Gán sự kiện cho nút Submit
        submitNameButton.onClick.AddListener(OnSubmitName);

        // Đăng nhập vào PlayFab
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        // Kiểm tra xem đã có tên người chơi chưa
        LoadPlayerData("PlayerName");
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    void OnSubmitName()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            Debug.LogWarning("Please enter a name!");
            if (successMessageText != null)
            {
                successMessageText.text = "Please enter a name!";
            }
            else
            {
                playerNameText.text = "Please enter a name!";
            }
            StartCoroutine(ClearMessageAfterDelay(3f));
            return;
        }

        // Lưu tên người chơi
        SavePlayerData("PlayerName", nameInputField.text);
    }

    public void SavePlayerData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, value } }
        };
        PlayFabClientAPI.UpdateUserData(request, OnSaveSuccess, OnSaveFailure);
    }

    void OnSaveSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Data saved successfully!");
        // Hiển thị thông báo thành công
        string playerName = nameInputField.text;
        string message = $"Thêm player: {playerName} thành công!";
        if (successMessageText != null)
        {
            successMessageText.text = message;
        }
        else
        {
            playerNameText.text = message;
        }

        // Xóa thông báo sau 3 giây
        //StartCoroutine(ClearMessageAfterDelay(3f));

        // Cập nhật hiển thị tên trên UI
        LoadPlayerData("PlayerName");
    }

    void OnSaveFailure(PlayFabError error)
    {
        Debug.LogError("Save failed: " + error.GenerateErrorReport());
        if (successMessageText != null)
        {
            successMessageText.text = "Failed to save player name!";
        }
        else
        {
            playerNameText.text = "Failed to save player name!";
        }
        StartCoroutine(ClearMessageAfterDelay(3f));
    }

    public void LoadPlayerData(string key)
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey(key))
            {
                string playerName = result.Data[key].Value;
                //Debug.Log($"Data loaded: {key} = {playerName}");
                // Hiển thị tên trên UI nếu không có thông báo thành công
                if (playerNameText != null && successMessageText == null)
                {
                    playerNameText.text = "Player: " + playerName;
                }
            }
            else
            {
                // Hiện giao diện nhập tên nếu chưa có
                if (nameInputCanvas != null)
                {
                    nameInputCanvas.alpha = 1;
                    nameInputCanvas.interactable = true;
                    nameInputCanvas.blocksRaycasts = true;
                }
            }
        }, error =>
        {
            Debug.LogError("Load failed: " + error.GenerateErrorReport());
            // Hiện giao diện nhập tên trong trường hợp lỗi
            if (nameInputCanvas != null)
            {
                nameInputCanvas.alpha = 1;
                nameInputCanvas.interactable = true;
                nameInputCanvas.blocksRaycasts = true;
            }
        });
    }

    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (successMessageText != null)
        {
            successMessageText.text = "";
        }
        else if (playerNameText != null)
        {
            // Khôi phục tên người chơi nếu có
            LoadPlayerData("PlayerName");
        }
    }
}