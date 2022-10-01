using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public TMP_Text level;
    public TMP_Text levelOutOfTotal;
    public Slider progress;
    public LevelNameObject levelsArrayVar;
    // Start is called before the first frame update
    void Start()
    {   
        int progressVal = RegisterLoginScreen.currentLvl;
        level.text = levelsArrayVar.levelsArray[progressVal - 1];
        progress.value = progressVal;
        levelOutOfTotal.text = progressVal + "/" + levelsArrayVar.levelsArray.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
