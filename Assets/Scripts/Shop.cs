using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private UI_ItemShop uiShop;

    private void OnTriggerEnter(Collider collider)
    {
        IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();
        if (shopCustomer != null)
        {
            uiShop.Show(shopCustomer);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();
        if (shopCustomer != null)
        {
            uiShop.Hide();
        }
    }
}
