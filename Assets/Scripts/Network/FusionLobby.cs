using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

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

    void CreateRunner()
    {
        if (runner != null) return;

        GameObject runnerObj = new GameObject("FusionRunner");
        DontDestroyOnLoad(runnerObj);

        runner = runnerObj.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        runnerObj.AddComponent<NetworkSceneManagerDefault>();
    }

    async void CreateRoom()
    {
        if (isConnecting || string.IsNullOrEmpty(createRoom.text))
            return;

        isConnecting = true;
        ShowLoading(true);

        await ClearRunnner();
        CreateRunner();

        var args = new StartGameArgs
        {
            GameMode = GameMode.Host,
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

        await ClearRunnner();
        CreateRunner();

        var args = new StartGameArgs
        {
            GameMode = GameMode.Client,
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

    async Task ClearRunnner()
    {
        if (runner != null)
        {
            await runner.Shutdown();
            Destroy(runner.gameObject);
            runner = null;
        }
    }

    void ShowLoading(bool show)
    {
        loaderPanel.SetActive(show);
        createButton.interactable = !show;
        joinButton.interactable = !show;
        createRoom.interactable = !show;
        joinRoom.interactable = !show;
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