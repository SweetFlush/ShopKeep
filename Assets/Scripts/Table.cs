using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private Item.ItemType item;

    private int index;  //?ڱ? ???̺? ??ȣ
    private int myPotion;   //???Ǻ?????
    private int potionQuarter;  //???? 3???? ???? ??
    private int potionRemain;   //???? ??????

    public Inventory inventory;

    public GameObject[] potionModels;

    private void Awake()
    {
        potionModels = new GameObject[3];
        potionModels[0] = transform.Find("Potions").Find("0").gameObject;
        potionModels[1] = transform.Find("Potions").Find("1").gameObject;
        potionModels[2] = transform.Find("Potions").Find("2").gameObject;

        inventory = FindObjectOfType<Player>().GetInventory();
        index = int.Parse(gameObject.name);
    }

    private void Update()
    {
        RefreshVisual();
    }

    public void RefreshVisual()
    {
        myPotion = inventory.GetAmount(Item.ItemType.Potion);

        potionQuarter = myPotion / 3;
        potionRemain = myPotion % 3;

        //Ź?ڹ?ȣ > ???Ǹ? = ???Ǹ?+1?̸? ????????, ??+1???ٸ????? ????.
        //Ź?ڹ?ȣ == ???Ǹ? = Ź?ڿ? ?????̰?????
        //Ź?ڹ?ȣ < ???Ǹ? = ???? ?̹? ??????

        if (index+1 <= potionQuarter)
        {
            potionModels[0].SetActive(true);
            potionModels[1].SetActive(true);
            potionModels[2].SetActive(true);
        }
        else if (index == potionQuarter)
        {
            switch (potionRemain)
            {
                default:
                    break;
                case 0:
                    potionModels[0].SetActive(false);
                    potionModels[1].SetActive(false);
                    potionModels[2].SetActive(false);
                    break;
                case 1:
                    potionModels[0].SetActive(true);
                    potionModels[1].SetActive(false);
                    potionModels[2].SetActive(false);
                    break;
                case 2:
                    potionModels[0].SetActive(true);
                    potionModels[1].SetActive(true);
                    potionModels[2].SetActive(false);
                    break;
            }
        }
    }
}
