using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType { //아이템 타입은 대문자로 시작한다
        BasicSword,
        AdvancedSword,
        EmeraldSword,
        Potion,
        Material,
        Coin,
        Table,
        WeaponHanger,
        PotionStorage,

    }//열거형 타입

    public ItemType itemType;
    public int amount;

    public static int GetCost(ItemType itemType) {      //도매값 리턴 static
        switch (itemType)
        {
            default:
            case ItemType.BasicSword: return 250;
            case ItemType.AdvancedSword: return 900;
            case ItemType.EmeraldSword: return 3500;
            case ItemType.Potion: return 50;
            case ItemType.Table: return 600;
            case ItemType.WeaponHanger: return 1000;
            case ItemType.PotionStorage: return 400;

        }
    }

    public static int GetBuyerCost(ItemType itemType)        //소비자값 리턴
    {      //가격값 리턴 static
        switch (itemType)
        {
            default:
            case ItemType.BasicSword: return 500;
            case ItemType.AdvancedSword: return 2000;
            case ItemType.EmeraldSword: return 5000;
            case ItemType.Potion: return 100;
            case ItemType.Table: return 0;
            case ItemType.WeaponHanger: return 0;
            case ItemType.PotionStorage: return 0;

        }
    }

    public static Sprite GetSprite(ItemType itemType)       //아이템에 해당하는 스프라이트값 리턴 static
    {
        switch (itemType) {
            default:
            case ItemType.BasicSword: return ItemAssets.Instance.basicSwordSprite;
            case ItemType.AdvancedSword: return ItemAssets.Instance.advancedSwordSprite;
            case ItemType.EmeraldSword: return ItemAssets.Instance.emeraldSwordSprite;
            case ItemType.Potion: return ItemAssets.Instance.potionSprite;
            case ItemType.Table: return ItemAssets.Instance.tableSprite;
            case ItemType.WeaponHanger: return ItemAssets.Instance.weaponHangerSprite;
            case ItemType.PotionStorage: return ItemAssets.Instance.potionStorageSprite;

        }
    }

    public static Mesh GetMesh(ItemType itemType)       //메쉬 모델 리턴 static
    {
        switch (itemType)
        {
            default:
            case ItemType.BasicSword: return ItemAssets.Instance.basicSwordModel;
            case ItemType.AdvancedSword: return ItemAssets.Instance.advancedSwordModel;
            case ItemType.EmeraldSword: return ItemAssets.Instance.emeraldSwordModel;
            case ItemType.Potion: return ItemAssets.Instance.potionModel;
            case ItemType.Table: return ItemAssets.Instance.tableModel;
            case ItemType.WeaponHanger: return ItemAssets.Instance.weaponHangerModel;
            case ItemType.PotionStorage: return ItemAssets.Instance.potionStorageModel;

        }
    }

    public Sprite GetSprite()       //아이템에 해당하는 스프라이트값 리턴
    {
        switch (itemType)
        {
            default:
            case ItemType.BasicSword: return ItemAssets.Instance.basicSwordSprite;
            case ItemType.AdvancedSword: return ItemAssets.Instance.advancedSwordSprite;
            case ItemType.EmeraldSword: return ItemAssets.Instance.emeraldSwordSprite;
            case ItemType.Potion: return ItemAssets.Instance.potionSprite;
            case ItemType.Table: return ItemAssets.Instance.tableSprite;
            case ItemType.WeaponHanger: return ItemAssets.Instance.weaponHangerSprite;
            case ItemType.PotionStorage: return ItemAssets.Instance.potionStorageSprite;

        }
    }

    public Mesh GetMesh()
    {
        switch (itemType)
        {
            default:
            case ItemType.BasicSword:       return ItemAssets.Instance.basicSwordModel;
            case ItemType.AdvancedSword:    return ItemAssets.Instance.advancedSwordModel;
            case ItemType.EmeraldSword:     return ItemAssets.Instance.emeraldSwordModel;
            case ItemType.Potion:           return ItemAssets.Instance.potionModel;
            case ItemType.Table:            return ItemAssets.Instance.tableModel;
            case ItemType.WeaponHanger:     return ItemAssets.Instance.weaponHangerModel;
            case ItemType.PotionStorage:    return ItemAssets.Instance.potionStorageModel;

        }
    }

    public Color GetColor()
    {
        switch (itemType)
        {
            default:
            case ItemType.BasicSword:       return new Color(1, 1, 1);
            case ItemType.AdvancedSword:    return new Color(1, 1, 1);
            case ItemType.EmeraldSword:     return new Color(0, 1, 0);
            case ItemType.Potion:           return new Color(1, 0, 0);
            case ItemType.Table:            return new Color(1, 0.92f, 0.016f);
            case ItemType.WeaponHanger:     return new Color(1, 0.92f, 0.016f);
            case ItemType.PotionStorage:    return new Color(1, 0.92f, 0.016f);
        }
    }

    public bool isStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Potion:
                return true;
            case ItemType.BasicSword:
            //case ItemType.AdvancedSword: return ;
            //case ItemType.EmeraldSword: return ;
            case ItemType.Table:
                return true;
            case ItemType.WeaponHanger:
                return true;
            case ItemType.PotionStorage:
                return false;
        }
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

}
