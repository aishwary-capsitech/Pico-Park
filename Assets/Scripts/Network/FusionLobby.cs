using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class FusionLobby : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;
    public Button createButton;
    public Button joinButton;

    public GameObject loaderPanel;
    public GameObject loader;

    private NetworkRunner runner;
    private bool isConnecting;

    void Awake()
    {
        loaderPanel.SetActive(false);

        // Create runner ONCE (important for fast join)
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // Required for fast & correct scene sync
        runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
    }

    void Start()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    void Update()
    {
        LoaderAnimation();
    }

    async void CreateRoom()
    {
        if (isConnecting || string.IsNullOrEmpty(createRoom.text))
            return;

        isConnecting = true;
        ShowLoading(true);

        var args = new StartGameArgs
        {
            GameMode = GameMode.Host,       // Host creates room
            SessionName = createRoom.text,
            Scene = SceneRef.FromIndex(1),
            PlayerCount = 6,
            IsVisible = true,
            IsOpen = true
        };

        var result = await runner.StartGame(args);

        if (!result.Ok)
        {
            Debug.LogError($"Create Room Failed: {result.ShutdownReason}");
            ResetUI();
            return;
        }

        Debug.Log("Room Created Successfully");
    }

    async void JoinRoom()
    {
        if (isConnecting || string.IsNullOrEmpty(joinRoom.text))
            return;

        isConnecting = true;
        ShowLoading(true);

        var args = new StartGameArgs
        {
            GameMode = GameMode.Client,     // Clients join room
            SessionName = joinRoom.text,
        };

        var result = await runner.StartGame(args);

        if (!result.Ok)
        {
            Debug.LogError($"Join Room Failed: {result.ShutdownReason}");
            ResetUI();
            return;
        }

        Debug.Log("Joined Room Successfully");
    }

    void ShowLoading(bool show)
    {
        loaderPanel.SetActive(show);
        createButton.interactable = !show;
        joinButton.interactable = !show;
    }

    void ResetUI()
    {
        isConnecting = false;
        ShowLoading(false);
    }

    void LoaderAnimation()
    {
        if (loader != null && loaderPanel.activeSelf)
        {
            loader.transform.Rotate(0f, 0f, -300f * Time.deltaTime);
        }
    }
}