using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    //List of items in cart
    public List<CartItem> cartList = new List<CartItem>();
    public GameManager gameManager;


    //variable for updating the cart panel
    public GameObject cartPanel;
    public GameObject billCartPanel;
    public TMP_Text billSum;
    public GameObject cartElementPrefab;



    //buy and add new items to cart
    public void AddListItem(ItemDetailsObject _item, float _quantity)
    {
        cartList.Add(new CartItem(_item, _quantity));


    }

    public void RemoveListItem(ItemDetailsObject _item)
    {
        for (int i = 0; i < cartList.Count; i++)
        {
            if (cartList[i].itemDetails.name == _item.name)//item selected id found in the list
            {
                cartList.Remove(cartList[i]);//using UpdateQuantity function of CartItem class

            }
        }
    }




    //to update the quantity when it is changed in shop menu
    public void UpdateListItem(ItemDetailsObject _item, float _newQuantity)
    {
        for (int i = 0; i < cartList.Count; i++)
        {
            if (cartList[i].itemDetails.name == _item.name)//item selected id found in the list
            {
                cartList[i].UpdateQuantity(_newQuantity);//using UpdateQuantity function of CartItem class

            }
        }
    }


    public void RefreshCartPanel(string _cartORbill)
    {
        float sum = 0;
        GameObject cartORbill = null;
        if (_cartORbill == "cart")
            cartORbill = cartPanel;
        else if (_cartORbill == "bill")
            cartORbill = billCartPanel;



        //reset cart each time u close cart window
        for (int i = 0; i < cartORbill.transform.childCount; i++)
        {
            Destroy(cartORbill.transform.GetChild(i).gameObject);

        }



        //add item in cart panel
        for (int i = 0; i < cartList.Count; i++)
        {
            sum += Mathf.FloorToInt(cartList[i].itemDetails.cost * cartList[i].quantity);

            GameObject _cartElement = Instantiate(cartElementPrefab, cartORbill.transform);

            //set the serial number of list item
            var _Sno = _cartElement.GetComponentInChildren<Transform>().Find("Sno");
            _Sno.GetComponent<TextMeshProUGUI>().text = (i + 1).ToString() + ".";

            //set the item name number of list item
            var _ItemName = _cartElement.GetComponentInChildren<Transform>().Find("ItemName");
            _ItemName.GetComponent<TextMeshProUGUI>().text = cartList[i].itemDetails.itemName;

            //set the quantity and type of list item
            var _Quantity = _cartElement.GetComponentInChildren<Transform>().Find("Quantity");
            _Quantity.GetComponent<TextMeshProUGUI>().text = cartList[i].quantity.ToString() + " " + cartList[i].itemDetails.type;

            //set the cost of list item
            var _Cost = _cartElement.GetComponentInChildren<Transform>().Find("Cost");
            _Cost.GetComponent<TextMeshProUGUI>().text = "Rs." + Mathf.FloorToInt(cartList[i].itemDetails.cost * cartList[i].quantity);

           
        }
        if (_cartORbill == "bill")
        {
            billSum.text = "Rs." + sum;
            gameManager.SetFinalAmount(sum);
        }
        //Debug.Log("refreshed");
    }




















    public class CartItem
    {
        public ItemDetailsObject itemDetails;
        public float quantity;

        //need a constructor to create new CartItems so that we can add that to the list 
        public CartItem(ItemDetailsObject _item, float _quantity)
        {
            itemDetails = _item;
            quantity = _quantity;
        }

        //function to add/ update the quantity
        public void UpdateQuantity(float _quantity)
        {
            quantity = _quantity;
        }
    }

}
