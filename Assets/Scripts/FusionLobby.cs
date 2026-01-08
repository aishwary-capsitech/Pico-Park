//using UnityEngine;
//using Fusion;
//using UnityEngine.UI;
//using TMPro;

//public class FusionLobby : MonoBehaviour
//{

//    public TMP_InputField createRoom;
//    public TMP_InputField joinRoom;
//    public Button createButton;
//    public Button joinButton;

//    private NetworkRunner runner;

//    void Start()
//    {

//        createButton.onClick.AddListener(CreateRoom);
//        joinButton.onClick.AddListener(JoinRoom);
//    }


//    public async void CreateRoom()
//    {
//        string roomName = createRoom.text;

//        if (string.IsNullOrEmpty(roomName))
//        {
//            Debug.Log("please eneter a room name");
//            return;
//        }
//        runner = gameObject.AddComponent<NetworkRunner>();
//        await runner.StartGame(new StartGameArgs()
//        {

//            GameMode = GameMode.Host,
//            SessionName = roomName,
//            Scene = SceneRef.FromIndex(1),
//            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
//            PlayerCount = 6,
//            SessionProperties = new()
//            //{
//            //    {"MaxPlayers",1}
//            //}
//        });

//        Debug.Log("Host has create a room to play");

//    }

//    public async void JoinRoom()
//    {
//        string roomName = joinRoom.text;

//        if (string.IsNullOrEmpty(roomName))
//        {
//            Debug.Log("please eneter a room name");
//            return;
//        }
//        runner = gameObject.AddComponent<NetworkRunner>();
//        await runner.StartGame(new StartGameArgs()
//        {
//            GameMode = GameMode.Client,
//            SessionName = roomName,
//            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),


//        });

//        Debug.Log("Client has entered in the room");
//    }
//}

using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class FusionLobby : MonoBehaviour
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;
    public Button createButton;
    public Button joinButton;

    public GameObject loaderPanel;
    public GameObject loader;

    private NetworkRunner runner;

    void Start()
    {
        loaderPanel.SetActive(false);

        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
    }

    void Update()
    {
        LoaderAnimation();
    }

    async void CreateRoom() 
    {
        if (string.IsNullOrEmpty(createRoom.text))
            return;

        ShowLoading(true);
        //await Task.Yield();

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = createRoom.text,
            Scene = SceneRef.FromIndex(1),
            PlayerCount = 6,
        });

        Debug.Log("Room Created");
    }

    async void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinRoom.text))
            return;

        ShowLoading(true);
        //await Task.Yield();

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = joinRoom.text
        });

        Debug.Log("Joined Room");
    }

    void ShowLoading(bool show)
    {
        loaderPanel.SetActive(show);
        createButton.interactable = !show;
        joinButton.interactable = !show;
    }

    void LoaderAnimation()
    {
        if(loader != null && loaderPanel.activeSelf == true)
        {
            loader.transform.Rotate(new Vector3(0f, 0f, -300f) * Time.deltaTime);
        }
    }
}
