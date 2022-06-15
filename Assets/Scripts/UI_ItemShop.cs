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
        CreateItemButton(Item.ItemType.BasicSword, Item.GetSprite(Item.ItemType.BasicSword), "강철 소드", Item.GetCost(Item.ItemType.BasicSword), Item.GetBuyerCost(Item.ItemType.BasicSword), 0);
        CreateItemButton(Item.ItemType.AdvancedSword, Item.GetSprite(Item.ItemType.AdvancedSword), "고급 소드", Item.GetCost(Item.ItemType.AdvancedSword), Item.GetBuyerCost(Item.ItemType.AdvancedSword), 1);
        CreateItemButton(Item.ItemType.EmeraldSword, Item.GetSprite(Item.ItemType.EmeraldSword), "에메랄드 소드", Item.GetCost(Item.ItemType.EmeraldSword), Item.GetBuyerCost(Item.ItemType.EmeraldSword), 2);
        CreateItemButton(Item.ItemType.Potion, Item.GetSprite(Item.ItemType.Potion), "체력 물약", Item.GetCost(Item.ItemType.Potion), Item.GetBuyerCost(Item.ItemType.Potion), 3);
        CreateItemButton(Item.ItemType.Table, Item.GetSprite(Item.ItemType.Table), "탁자", Item.GetCost(Item.ItemType.Table), Item.GetBuyerCost(Item.ItemType.Table), 4);
        CreateItemButton(Item.ItemType.WeaponHanger, Item.GetSprite(Item.ItemType.WeaponHanger), "무기 거치대", Item.GetCost(Item.ItemType.WeaponHanger), Item.GetBuyerCost(Item.ItemType.WeaponHanger), 5);

        Hide();
    }

    private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite, string itemName, int itemCost, int itemBuyerCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container); //생성하기
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, 350 -shopItemHeight * positionIndex);   //배치

        shopItemTransform.Find("Text").GetComponent<Text>().text = itemName;                        //이름
        shopItemTransform.Find("Price").GetComponent<Text>().text = itemCost.ToString();            //가격

        switch (itemBuyerCost) 
        {
            default:
                shopItemTransform.Find("buyerPrice").GetComponent<Text>().text = itemBuyerCost.ToString();
                break;
            case 0:
                shopItemTransform.Find("buyerPrice").GetComponent<Text>().text = "비매품";
                break;
        }
        
        shopItemTransform.Find("Image").GetComponent<Image>().sprite = itemSprite;                  //그림
        shopItemTransform.gameObject.SetActive(true);

        shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
        { //아이템 구매 버튼 클릭
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
