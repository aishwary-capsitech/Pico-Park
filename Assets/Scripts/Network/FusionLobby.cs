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
    public GameObject tryAgainPanel;
    public TMP_Text tryAgainText1;
    public TMP_Text tryAgainText2;

    private NetworkRunner runner;
    private bool isConnecting;

    void Awake()
    {
        loaderPanel.SetActive(false);
        tryAgainPanel.SetActive(false);
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

        runnerObj.AddComponent<NetworkSceneManagerDefault>(); // Commented Modified

        // Rebind runner to NetworkManager
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SetRunner(runner);
        }
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
            //GameMode = GameMode.Shared,// Modified
            SessionName = createRoom.text,
            //Scene = SceneRef.FromIndex(1),
            PlayerCount = 6,
            IsVisible = true,
            IsOpen = true
        };

        var result = await runner.StartGame(args);

        if(result.Ok && runner.IsServer)
        {
            runner.LoadScene(SceneRef.FromIndex(1));
        }

        if (!result.Ok)
        {
            Debug.LogError($"Create Room Failed: {result.ShutdownReason}");

            if(result.ShutdownReason == ShutdownReason.ServerInRoom)
            {
                tryAgainText1.text = $"Room name '{createRoom.text}' is already taken!.";
                tryAgainText2.text = "Try another room name.";
            }
            else if (result.ShutdownReason == ShutdownReason.PhotonCloudTimeout)
            {
                tryAgainText1.text = $"Connection to server timed out!.";
                tryAgainText2.text = "Please try again.";
            }
            else
            {
                tryAgainText1.text = $"Failed to create room '{createRoom.text}'.";
                tryAgainText2.text = "Please try again.";
            }

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
            //GameMode = GameMode.Shared,// Modified
            SessionName = joinRoom.text,
        };

        var result = await runner.StartGame(args);

        if (!result.Ok)
        {
            Debug.LogError($"Join Room Failed: {result.ShutdownReason}");

            switch (result.ShutdownReason)
            {
                case ShutdownReason.GameNotFound:
                    // This can happen due to timing issues as well
                    tryAgainText1.text = $"Unable to find room '{joinRoom.text}'.";
                    tryAgainText2.text = "Make sure the host is online and try again.";
                    break;

                case ShutdownReason.PhotonCloudTimeout:
                    tryAgainText1.text = "Connection to server timed out!";
                    tryAgainText2.text = "Please try again.";
                    break;

                case ShutdownReason.Ok:
                    // Should never hit here, but safe guard
                    tryAgainText1.text = $"Failed to join room '{joinRoom.text}'.";
                    tryAgainText2.text = "Please try again.";
                    break;

                default:
                    tryAgainText1.text = $"Failed to join room '{joinRoom.text}'.";
                    tryAgainText2.text = "Please try again.";
                    break;
            }

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
            //Destroy(runner.gameObject); // Modified
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
        tryAgainPanel.SetActive(true);
    }

    void LoaderAnimation()
    {
        if (loader != null && loaderPanel.activeSelf)
        {
            loader.transform.Rotate(0f, 0f, -300f * Time.deltaTime);
        }
    }
    
    public void OnTryAgain()
    {
        tryAgainPanel.SetActive(false);
        joinRoom.text = "";
        createRoom.text = "";
    }
}