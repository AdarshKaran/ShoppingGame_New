using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text usernameDisplay;
    public GameObject quitPanel;
    // Start is called before the first frame update
    void Start()
    {
        usernameDisplay.text = "Hi "+RegisterLoginScreen.username;
        quitPanel.SetActive(false);
    }



    public void Startgame()
    {
        SceneManager.LoadScene(2);
    }

    public void BackPanel()
    {
        quitPanel.SetActive(true);
    }
    public void ContinueGame()
    {
        quitPanel.SetActive(false);
    }
}
