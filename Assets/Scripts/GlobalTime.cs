using Firebase.Firestore;
using TMPro;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{
    private float totalTime = SetPlayTime.TimeSet.timerVal;//the time for which the game is allowed to run for
    public float timeSpentBeforeLogin;
    public bool gameOver;
    int min;
    int sec;
    private string dataPath;

    //ref to the timer
    public GameObject timerCountdown;


    //time up panel
    public GameObject timeUpPanel;
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        if (timeUpPanel)
            timeUpPanel.SetActive(false);
        timeSpentBeforeLogin = RegisterLoginScreen.timeSpentDuringLogin;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            totalTime -= Time.deltaTime;
            SetPlayTime.TimeSet.timerVal=totalTime;
            min = Mathf.FloorToInt(totalTime / 60);
            sec = Mathf.FloorToInt(totalTime % 60);
            timerCountdown.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", min, sec);
            //Debug.Log(min.ToString() + ":" + sec.ToString());

        }
        //Time up panel
        float  timeVal = Mathf.Round(totalTime);
        if ( timeVal<= 0)
        {
            gameOver = true;
            UpdateUserTimeData(min,sec);
            timeUpPanel.SetActive(true);
            timerCountdown.GetComponent<TextMeshProUGUI>().text = "--";
        }

    }

    public void QuitGameOnTimeOver()
    {
        UpdateUserTimeData(0, 0);
        Application.Quit();
    }
    public void ExitGame()
    {
        UpdateUserTimeData(min, sec);
        Application.Quit();
    }
    void OnApplicationQuit()
    {
        UpdateUserTimeData(min, sec);
    }

    public void UpdateUserTimeData(int _min, int _sec)
    {
        var databaseReference = FirebaseFirestore.DefaultInstance;

        var _userId = RegisterLoginScreen.firebaseUser;
        dataPath = $"users/{_userId.UserId}";
        var characterData = new CharacterData
        {
            min = _min,
            sec = _sec
        };
        databaseReference.Document(dataPath).SetAsync(characterData, SetOptions.MergeFields("min","sec"));
        dataPath = "";//reset data path
    }
}
