using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public InventorySlot inventory;

    //ref to the itemDetailsObject to be displayed
    public ItemDetailsObject item;

    //quantity of item chosen
    public float quantity;

    //ref to drop down to choose quantity


    //flag to check if item had been bought



    public GameObject buyButton;
    public GameObject cancelButton;
    public GameObject dropDownObject;
    public GameObject itemName;
    public GameObject itemCost;
    public GameObject itemCostPerTypeText;
    public Image itemImage;



    // Start is called before the first frame update
    void Start()
    {
        buyButton.SetActive(true);
        cancelButton.SetActive(false);
        dropDownObject.SetActive(false);
        itemName.GetComponent<TextMeshProUGUI>().text = item.itemName;
        itemCost.GetComponent<TextMeshProUGUI>().text = "Rs." + item.cost.ToString();
        itemCostPerTypeText.GetComponent<TextMeshProUGUI>().text = "  Cost per " + item.type;
        itemImage.sprite=item.image;

        //set the dropdown menu values

        List<string> newOptions = new List<string>();
        for (int i = 0; i < item.quantityOptions.Length; i++)
        {
            newOptions.Add(item.quantityOptions[i] + " " + item.type);
        }
        dropDownObject.GetComponent<TMP_Dropdown>().AddOptions(newOptions);

        //---------------------------------------------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuyItemGUI()
    {
        //GUI update

        buyButton.SetActive(false);
        cancelButton.SetActive(true);
        dropDownObject.SetActive(true);
        // Debug.Log("bought");
        //----------------------


        BuyItem();
        //add new item to cart


    }
    public void BuyItem()
    {
        //the item bought is added to a list called cartList in InventorySlot script
        {
            dropDownObject.GetComponent<TMP_Dropdown>().value = 0;
            quantity = item.quantityOptions[0];
            inventory.AddListItem(item, quantity);
            // Debug.Log("new item added- " + quantity);

        }
    }


    public void CancelitemGUI()
    {
        //GUI update
        buyButton.SetActive(true);
        cancelButton.SetActive(false);
        dropDownObject.SetActive(false);

        //----------------------

        CancelItem();
        //remove item from cart
    }

    public void CancelItem()
    {

        inventory.RemoveListItem(item);

    }
    public void UpdateItemQuantity()
    {
        //update the quantity of items when quantity is changed
        int index = dropDownObject.GetComponent<TMP_Dropdown>().value;//get the index value of the drop down menu
        quantity = item.quantityOptions[index];// find the corresponding quantity value from ItemDetailsobkect
        inventory.UpdateListItem(item, quantity);
        // Debug.Log("quantity updated- "+quantity);
    }
}


