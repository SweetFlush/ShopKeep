using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private Item.ItemType item;

    private int index;  //자기 테이블 번호
    private int myPotion;   //포션보유량
    private int potionQuarter;  //포션 3으로 나눈 몫
    private int potionRemain;   //포션 나머지

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

        //탁자번호 > 포션몫 = 포션몫+1이면 생성결정, 몫+1보다많으면 없음.
        //탁자번호 == 포션몫 = 탁자에 포션이가득함
        //탁자번호 < 포션몫 = 포션 이미 가득함

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
