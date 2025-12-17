using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FusionLobby : MonoBehaviour
{

    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;
    public Button createButton;
    public Button joinButton; 

   

    private NetworkRunner runner;

    
    
    void Start()
    {
   
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

 
     public async void CreateRoom()
    {
        string roomName = createRoom.text;

        if(string.IsNullOrEmpty(roomName))
        {
            Debug.Log("please eneter a room name");
            return;
        }
        runner = gameObject.AddComponent<NetworkRunner>();
        await runner.StartGame(new StartGameArgs()
          {

             GameMode = GameMode.Host,
             SessionName =  roomName,
             Scene = SceneRef.FromIndex(1),
             SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            
             SessionProperties = new()
        {
            {"MaxPlayers",4}
        }
        });

        Debug.Log("Host has create a room to play");
        
 }

     public async void  JoinRoom()
    {
        string roomName = joinRoom.text;

        if(string.IsNullOrEmpty(roomName))
        {
            Debug.Log("please eneter a room name");
            return;
        }
         runner = gameObject.AddComponent<NetworkRunner>();
         await runner.StartGame(new StartGameArgs()
          {
             GameMode = GameMode.Client,
             SessionName =  roomName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            
             
    });

    Debug.Log("Client has entered in the room");
}
}

