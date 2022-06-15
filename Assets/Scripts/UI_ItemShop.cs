using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class UI_ItemShop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer shopCustomer;

    public Player player;
    public Inventory Inventory;

    private void Awake()
    {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(Item.ItemType.BasicSword, Item.GetSprite(Item.ItemType.BasicSword), "��ö �ҵ�", Item.GetCost(Item.ItemType.BasicSword), Item.GetBuyerCost(Item.ItemType.BasicSword), 0);
        CreateItemButton(Item.ItemType.AdvancedSword, Item.GetSprite(Item.ItemType.AdvancedSword), "��� �ҵ�", Item.GetCost(Item.ItemType.AdvancedSword), Item.GetBuyerCost(Item.ItemType.AdvancedSword), 1);
        CreateItemButton(Item.ItemType.EmeraldSword, Item.GetSprite(Item.ItemType.EmeraldSword), "���޶��� �ҵ�", Item.GetCost(Item.ItemType.EmeraldSword), Item.GetBuyerCost(Item.ItemType.EmeraldSword), 2);
        CreateItemButton(Item.ItemType.Potion, Item.GetSprite(Item.ItemType.Potion), "ü�� ����", Item.GetCost(Item.ItemType.Potion), Item.GetBuyerCost(Item.ItemType.Potion), 3);
        CreateItemButton(Item.ItemType.Table, Item.GetSprite(Item.ItemType.Table), "Ź��", Item.GetCost(Item.ItemType.Table), Item.GetBuyerCost(Item.ItemType.Table), 4);
        CreateItemButton(Item.ItemType.WeaponHanger, Item.GetSprite(Item.ItemType.WeaponHanger), "���� ��ġ��", Item.GetCost(Item.ItemType.WeaponHanger), Item.GetBuyerCost(Item.ItemType.WeaponHanger), 5);

        Hide();
    }

    private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite, string itemName, int itemCost, int itemBuyerCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container); //�����ϱ�
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, 350 -shopItemHeight * positionIndex);   //��ġ

        shopItemTransform.Find("Text").GetComponent<Text>().text = itemName;                        //�̸�
        shopItemTransform.Find("Price").GetComponent<Text>().text = itemCost.ToString();            //����

        switch (itemBuyerCost) 
        {
            default:
                shopItemTransform.Find("buyerPrice").GetComponent<Text>().text = itemBuyerCost.ToString();
                break;
            case 0:
                shopItemTransform.Find("buyerPrice").GetComponent<Text>().text = "���ǰ";
                break;
        }
        
        shopItemTransform.Find("Image").GetComponent<Image>().sprite = itemSprite;                  //�׸�
        shopItemTransform.gameObject.SetActive(true);

        shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
        { //������ ���� ��ư Ŭ��
            TryBuyItem(itemType);
        };
    }

    private void TryBuyItem(Item.ItemType itemType)
    {
        if( shopCustomer.TrySpendGoldAmount(Item.GetCost(itemType))) {
            shopCustomer.BoughtItem(itemType);
        }
    }

    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
