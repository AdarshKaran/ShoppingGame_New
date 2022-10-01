using Firebase.Auth;
using Firebase;
using TMPro;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;

public class GameManager : MonoBehaviour
{
    [Header("Navigation")]
    //to store all panels to be opened and closed 
    public GameObject[] AllPanels;
    /*
     Index |    Panel
        0     SHOP_PANEL  
        1     CART_PANEL  
        2     LIST_PANEL  
        3     WALLET_PANEL 
        4     BILL_PANEL  
        5     BACK_PANEL    
        6     COMPLETED_PANEL    
        7     ERROR_PANEL    
    */
    public Button[] bottomButtons;

    [Header("Level Details")]
    public LevelDetailsObject CurrentLevel;
    public SpecialLevelDetails SpecialLevel;
    public GameObject LevelName;
    public GameObject LevelNameEndScreen;

    [Header("GUI")]

    public GameObject listElementPrefab;//the prefab of the list element
    public GameObject listPanel;//the ref to the list panel
    public InventorySlot cart;
    public GameObject constraintPanel;
    public Button walletButton;// to enable or disable wallet button for special levels

    public TMP_Text errorMssg;
    public TMP_Text constraintPanelText;
    public TMP_Text availableCashDisplay;    //to display cash availabe
    public Button[] withdrawValues;
    public TMP_Text withdrawnAmountDisplay;
    public Color blueAccent;
    public Color yellowSecondary;

    public Scrollbar[] allScrollbars;



    [Header("Variables")]

    public int finalAmnt = 0;
    public int availableAmnt = 0;
    public int withdrawnAmnt = 0;
    public bool listMatches;
    public bool amntMatches;
    public bool list_AND_amntMatch;

    int indexQuant = 0;

    private string dataPath;



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //set wallet options for special levels
        if (CurrentLevel.walletWithdrawAvailable)
        {
            walletButton.interactable = true;
            withdrawnAmnt = 0;

            //set the values of withdraw buttons
            for (int i = 0; i < 4; i++)
            {
                withdrawValues[i].GetComponentInChildren<TMP_Text>().text = "Rs."+SpecialLevel.walletValuesAvailable[i];
            }
        }
        else
            walletButton.interactable = false;

        //setting the initial states
        availableAmnt = CurrentLevel.cashAvaiable;//set the cash available in this level
        availableCashDisplay.text = "Rs." + availableAmnt;
        
        
        bottomButtons[0].Select();
        SetButonColor(0);//to set shop button selected

        listMatches = false;
        amntMatches = false;
        for (int i = 0; i < allScrollbars.Length; i++)
        {
            allScrollbars[i].GetComponent<Scrollbar>().value = 1;
        }
        //all panels are hidden except SHOP Panel when the level starts
        AllPanels[0].SetActive(true);
        for (int i = 1; i < AllPanels.Length; i++)
        {
            AllPanels[i].SetActive(false);
        }
        LevelName.GetComponent<TextMeshProUGUI>().text = CurrentLevel.nameOfLevel;
        LevelNameEndScreen.GetComponent<TextMeshProUGUI>().text = CurrentLevel.nameOfLevel;
        #region CREATE LIST 
        //-----------------------------------------------------------------------------------------------------------------
        //to create the required items list dynamically
        for (int i = 0; i < CurrentLevel.requirementsList.Count; i++)
        {
            GameObject _listElement = Instantiate(listElementPrefab, listPanel.transform);

            //set the serial number of list item
            var _Sno = _listElement.GetComponentInChildren<Transform>().Find("Sno");
            _Sno.GetComponent<TextMeshProUGUI>().text = (i + 1).ToString() + ".";

            //set the item name number of list item
            var _ItemName = _listElement.GetComponentInChildren<Transform>().Find("ItemName");
            _ItemName.GetComponent<TextMeshProUGUI>().text = CurrentLevel.requirementsList[i].itemName;

            //set the quantity and type of list item
            var _Quantity = _listElement.GetComponentInChildren<Transform>().Find("Quantity");
            _Quantity.GetComponent<TextMeshProUGUI>().text = CurrentLevel.requirementsList[i].quantity.ToString() + " " + CurrentLevel.requirementsList[i].type;
        }
        #endregion
        //set the constraint text in constraint panel
        constraintPanel.SetActive(true);
        constraintPanelText.text= CurrentLevel.constraintText;
    }

    #region WITHDRAW OPERATIONS
    public void WithdrawAmount(int i)
    {
        
        withdrawnAmnt = SpecialLevel.walletValuesAvailable[i];
        withdrawnAmountDisplay.text = "Rs." + withdrawnAmnt;
        availableAmnt += withdrawnAmnt;
        availableCashDisplay.text = "Rs." + availableAmnt;
        for (int j = 0; j < 4; j++)
        {
            withdrawValues[j].interactable = false;
        }


    }
    public void ResetWithdrawAmount()
    {
        withdrawnAmountDisplay.text = "0";
        withdrawnAmnt = 0;
        availableAmnt = CurrentLevel.cashAvaiable;
        availableCashDisplay.text = "Rs." + availableAmnt;
        for (int i = 0; i < 4; i++)
        {
            withdrawValues[i].interactable = true;
        }
    }

    #endregion



    #region GUI/ NAVIGATION
    public void OpenSelectedPanel(int x)
    {
        for (int i = 0; i < AllPanels.Length; i++)
        {
            if (i != x)
            {
                AllPanels[i].SetActive(false);
            }
            else
            {
                AllPanels[i].SetActive(true);
            }
        }
    }

    public void SetButonColor(int x)
    {
        for (int i = 0; i < bottomButtons.Length; i++)
        {
            ColorBlock theColor = bottomButtons[i].colors;
            if (i != x)
            {
                theColor.normalColor = blueAccent;
                bottomButtons[i].colors = theColor;
            }
            else
            {
                theColor.normalColor = yellowSecondary;
                bottomButtons[i].colors=theColor;


            }
        }
    }

    public void BackPanel()
    {
        AllPanels[5].SetActive(true);
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ContinueGame()
    {
        AllPanels[5].SetActive(false);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RetryLvl()
    {
        AllPanels[7].SetActive(false);
    }
    public void OkConstraintPanel()
    {
        constraintPanel.SetActive(false);
    }





    #endregion


    public void SetFinalAmount(float _sum)
    {
        finalAmnt = Mathf.FloorToInt(_sum);
    }

    public void CompareAmounts()
    {
        amntMatches = false;

        for (indexQuant = 0; indexQuant < CurrentLevel.finalAmount.Length; indexQuant++)
        {
            //for special level condition
            if (CurrentLevel.walletWithdrawAvailable && SpecialLevel != null  )
            {
                //to check if optimal withdraw option is chosen
                if (finalAmnt == CurrentLevel.finalAmount[indexQuant])
                {
                    if (finalAmnt > availableAmnt)
                    {
                        AllPanels[7].SetActive(true);
                        errorMssg.text = "Not enough Cash Available";
                    }
                    else if(withdrawnAmnt != SpecialLevel.optimalWithdrawAmnt)
                    {
                        AllPanels[7].SetActive(true);
                        errorMssg.text = "Withdraw only the Optimal Amount";
                    }
                    else if(withdrawnAmnt == SpecialLevel.optimalWithdrawAmnt)
                    {
                        amntMatches = true;
                        break;
                    }
                    
                }
                else
                {
                    amntMatches = false;
                    AllPanels[7].SetActive(true);
                    errorMssg.text = "Final Amount is Wrong";
                }

                
            }


            else if (!CurrentLevel.walletWithdrawAvailable && finalAmnt == CurrentLevel.finalAmount[indexQuant])
            {
                amntMatches = true;
                break;
            }
            else
            {
                amntMatches = false;
                AllPanels[7].SetActive(true);
                errorMssg.text = "Final Amount is Wrong";
            }
        }
        if (amntMatches)
            ConfirmFinalAmnt();
    }


    public void ConfirmFinalAmnt()
    {
        int x = CompareList();
        if (x != indexQuant && listMatches)
        {
            AllPanels[7].SetActive(true);
            errorMssg.text = "Amount Entered is different from Cart Total Amount";

        }
        else if (x == indexQuant && listMatches)
        {
            Debug.Log("Level update");
            var currLvl = RegisterLoginScreen.currentLvl;
            if (!(currLvl >CurrentLevel.levelNo))
                currLvl += 1;
            UpdateUserLevelData(currLvl);
            RegisterLoginScreen.currentLvl=currLvl; 
                AllPanels[7].SetActive(false);
                AllPanels[6].SetActive(true);
            
        }
        else
        {
            AllPanels[7].SetActive(true);
            errorMssg.text = "List Items are Wrong";
        }
    }

    public int CompareList()//to compare cartList and requirementsList along with constraints
    {
        int i = 0;
        //if items match
        listMatches = false;


        //this checks if constraints are full filled and compares list on that basis
        #region COMPLEX CONSTRAINT  

        //for each item in the cart list I have to check with the requirementsList

        //first check if count of lists is equal
        if (CurrentLevel.requirementsList.Count == cart.cartList.Count)
        {
            for (i = 0; i < CurrentLevel.allPossibleListsObject.allPossibleLists.Count; i++)
            {
                foreach (var item in cart.cartList)
                {

                    foreach (var listItem in CurrentLevel.allPossibleListsObject.allPossibleLists[i].possibleSolns)
                    {
                        Debug.Log(item.itemDetails.itemName + "," + item.quantity + " " + "compared to " + listItem.possibleItem + "," + listItem.possibleQuantity);
                        if (item.itemDetails.itemName == listItem.possibleItem && item.quantity == listItem.possibleQuantity)
                        {
                            listMatches = true;
                            break;
                        }
                        listMatches = false;
                    }
                    if (!listMatches)
                        break;
                }
                if (listMatches)
                    break;
            }
            if (listMatches)
                Debug.Log("List matches");
            else
                Debug.Log("ERROR");


        }
        else//if lists are unequal
            Debug.Log("ERROR List Unequal Count");


        return i;
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    public void UpdateUserLevelData(int _newLevel)
    {
        var databaseReference = FirebaseFirestore.DefaultInstance;

        var _userId = RegisterLoginScreen.firebaseUser;
        dataPath = $"users/{_userId.UserId}";
        var characterData = new CharacterData
        { 
            currentLevel = _newLevel,
        };
        databaseReference.Document(dataPath).SetAsync(characterData, SetOptions.MergeFields("currentLevel"));
        dataPath = "";//reset data path
    }


}
