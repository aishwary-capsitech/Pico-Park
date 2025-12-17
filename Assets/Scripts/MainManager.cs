using UnityEngine;
using UnityEngine.SceneManagement;
public class MainManager : MonoBehaviour
{
     public void GoToTheLobbyScene(string sceneName)
    {
        SceneManager.LoadScene("Lobby");
    }
    
    
}
