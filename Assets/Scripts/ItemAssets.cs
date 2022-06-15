using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite basicSwordSprite;
    public Sprite advancedSwordSprite;
    public Sprite emeraldSwordSprite;
    public Sprite potionSprite;
    public Sprite tableSprite;
    public Sprite weaponHangerSprite;
    public Sprite potionStorageSprite;

    public Mesh basicSwordModel;
    public Mesh advancedSwordModel;
    public Mesh emeraldSwordModel;
    public Mesh potionModel;
    public Mesh tableModel;
    public Mesh weaponHangerModel;
    public Mesh potionStorageModel;
}
