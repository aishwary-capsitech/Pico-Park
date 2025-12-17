using System.Collections;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public GameObject GameOverScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        StartCoroutine(OpenCloseScreen());
    }

    private IEnumerator OpenCloseScreen()
    {
        GameOverScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameOverScreen.SetActive(false);
    }
}
