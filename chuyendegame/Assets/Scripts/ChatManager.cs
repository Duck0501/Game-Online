using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public ChatUI chatUI;
    ChatClient chatClient;
    public static ChatManager Instance;
    public GameObject chat;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Đặt nickname ngẫu nhiên
        PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 1000);
        chatClient = new ChatClient(this);
        chatClient.Connect(
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion,
            new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName)
        );
    }

    void Update()
    {
        chatClient.Service(); // Cập nhật trạng thái chat
        if (Input.GetKeyDown(KeyCode.C))
        {
            chat.SetActive(!chat.activeSelf);
        }
    }

    public void SendChatMessage(string message)
    {
        chatClient.PublishMessage("General", message);
    }

    // Implement IChatClientListener
    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { "General" });
    }

    public void OnDisconnected() { }

    public void OnChatStateChange(ChatState state) { }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            chatUI.chatContent.text += $"{senders[i]}: {messages[i]}\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) { }

    public void OnSubscribed(string[] channels, bool[] results) { }

    public void OnUnsubscribed(string[] channels) { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void DebugReturn(DebugLevel level, string message) { }

    // Add the missing methods
    public void OnUserSubscribed(string channel, string user)
    {
        // Xử lý khi một người dùng tham gia kênh
        Debug.Log($"{user} subscribed to {channel}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        // Xử lý khi một người dùng rời kênh
        Debug.Log($"{user} unsubscribed from {channel}");
    }

    private bool CanvasActive
    {
        get => chat.activeSelf;
        set => chat.SetActive(value);
    }
}