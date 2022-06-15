using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHanger : MonoBehaviour
{
    public int index;  //�ڱ� ���̺� ��ȣ
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

        //basicSword���������� �ε����� ũ�� advance�� �Ѿ��.
        //basicSword���������� ������ basicSword�� ǥ���Ѵ�.
        //basicSword�������� Advanced�������� ���Ѱ����� �ε����� ũ�� emeraldSword�� �Ѿ��.

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
