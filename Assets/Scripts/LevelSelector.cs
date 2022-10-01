using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] Levels;//array of all level buttons
    public int currentLevel; //current level upto which player has progressed



    /*3 different images for progress
      completed levels- tick sign
      current lvl- open lock
      locked levels- closed lock
    */
    public Sprite completeImg;
    public Sprite currentImg;
    public Sprite lockedImg;

    private void Awake()
    {
        currentLevel=RegisterLoginScreen.currentLvl;

    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        for (int i = 0; i < Levels.Length; i++)
        {
            if (i < currentLevel - 1)
            {
                Levels[i].interactable = true;
                var image = Levels[i].GetComponentInChildren<Transform>().Find("Icon");
                image.GetComponent<Image>().sprite = completeImg;
            }
            else if (i == currentLevel - 1)
            {
                Levels[i].interactable = true;
                var image = Levels[i].GetComponentInChildren<Transform>().Find("Icon");
                image.GetComponent<Image>().sprite = currentImg;
            }
            else
            {
                Levels[i].interactable = false;
                var image = Levels[i].GetComponentInChildren<Transform>().Find("Icon");
                image.GetComponent<Image>().sprite = lockedImg;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelSelected(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }

    public void BackButton(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
