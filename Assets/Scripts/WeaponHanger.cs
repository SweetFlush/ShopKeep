using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHanger : MonoBehaviour
{
    public int index;  //자기 테이블 번호
    private Item.ItemType item;

    public int totalSwordAmount;
    public int basicSwordAmount;
    public int advancedSwordAmount;
    public int emeraldSwordAmount;

    private int currentBasicSwordAmount;
    private int currentAdvancedSwordAmount;
    private int currentEmeraldSwordAmount;

    public Inventory inventory;

    public GameObject[] swordModels;

    private void Awake()
    {
        swordModels = new GameObject[3];
        swordModels[0] = transform.Find("Weapons").Find("BasicSword").gameObject;
        swordModels[1] = transform.Find("Weapons").Find("AdvancedSword").gameObject;
        swordModels[2] = transform.Find("Weapons").Find("EmeraldSword").gameObject;

        inventory = FindObjectOfType<Player>().GetInventory();
        index = int.Parse(gameObject.name) + 1;
    }

    private void Update()
    {
        RefreshVisual();
    }

    public void RefreshVisual()
    {
        basicSwordAmount = inventory.GetAmount(Item.ItemType.BasicSword);
        advancedSwordAmount = inventory.GetAmount(Item.ItemType.AdvancedSword);
        emeraldSwordAmount = inventory.GetAmount(Item.ItemType.EmeraldSword);
        totalSwordAmount = basicSwordAmount + advancedSwordAmount + emeraldSwordAmount;

        //basicSword보유량보다 인덱스가 크면 advance로 넘어간다.
        //basicSword보유량보다 작으면 basicSword를 표시한다.
        //basicSword보유량과 Advanced보유량을 합한값보다 인덱스가 크면 emeraldSword로 넘어간다.

        if (index <= totalSwordAmount)
        {
            if (index <= basicSwordAmount) //index = 1, 2; basicSwordAmount = 3;
            {
                swordModels[0].SetActive(true);
                swordModels[1].SetActive(false);
                swordModels[2].SetActive(false);
            }
            else if (index > basicSwordAmount && index <= basicSwordAmount + advancedSwordAmount)
            {
                swordModels[0].SetActive(false);
                swordModels[1].SetActive(true);
                swordModels[2].SetActive(false);
            }
            else if (index > basicSwordAmount + advancedSwordAmount && index <= totalSwordAmount)
            {
                swordModels[0].SetActive(false);
                swordModels[1].SetActive(false);
                swordModels[2].SetActive(true);
            }
        } 
    }

    public void HidebasicSword()
    {
        //if(swordModels[0].isActiveAndEnabled)
        swordModels[0].SetActive(false);
        currentBasicSwordAmount--;
    }

    public void HideAdvancedSword()
    {
        swordModels[1].SetActive(false);
        currentAdvancedSwordAmount--;
    }

    public void HideEmeraldSword()
    {
        swordModels[2].SetActive(false);
        currentEmeraldSwordAmount--;
    }
}
