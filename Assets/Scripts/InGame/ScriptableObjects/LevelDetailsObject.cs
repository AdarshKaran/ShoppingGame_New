using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelNumber")]
public class LevelDetailsObject : ScriptableObject
{
    public string nameOfLevel;
    public int levelNo;
    public int itemsCountNeeded;
    public int cashAvaiable;
    public int[] finalAmount;
    public bool walletWithdrawAvailable;


    [TextArea(6, 12)]
    public string constraintText;







    [Header("LIST OF ITEMS NEEDED")]
    //LIST OF REQUIRED ITEMS
    public List<ListDetailsObj> requirementsList = new List<ListDetailsObj>();

    [System.Serializable]
    public class ListDetailsObj
    {
        public int sno;
        public string itemName;
        public float quantity;
        public string type;


        //constructor to create list details
        public ListDetailsObj(int _sno, string _itemName, float _quantity, string _type)
        {
            _sno = sno;
            _itemName = itemName;
            _quantity = quantity;
            _type = type;
        }

    }


    [System.Serializable]
    //class for creating Required items list
    public class AllSolnList
    {
        [Header("ALL POSSIBLE SOLUTIONS")]
        //list of complex constraints
        public List<PossibleSoln> possibleSolns = new List<PossibleSoln>();
        //class for Possible solution items for complex constraints
        [System.Serializable]
        public class PossibleSoln
        {
            public float possibleQuantity;
            public string possibleItem;

            public PossibleSoln(float _possibleQuantity, string _possibleItem)
            {
                possibleQuantity = _possibleQuantity;
                possibleItem = _possibleItem;
            }
        }
    }




    [System.Serializable]
    public class AllPossibleLists
    {
        public List<AllSolnList> allPossibleLists = new List<AllSolnList>();

    }
    public AllPossibleLists allPossibleListsObject = new AllPossibleLists();















}
