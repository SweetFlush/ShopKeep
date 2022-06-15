using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class Player : MonoBehaviour, IShopCustomer
{
    public static Player Instance { get; private set; }

    [SerializeField] private UI_Inventory uiInventory;

    private Inventory inventory;

    private bool isFurniture = false;
    private bool isDamage;
    public int numOfTable;
    public int numOfWeaponHanger;

    public float speed;
    public int gold;
    public int maxHealth;
    public int health;

    public Transform respawnZone;

    public GridBuildingSystem gridBuildingSystem;

    float hAxis;
    float vAxis;

    private Vector3 moveVec;
    private Animator anim;
    private Rigidbody rigid;
    private MeshRenderer[] meshes;
    private Item.ItemType usedItemType;
   
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshes = GetComponentsInChildren<MeshRenderer>();

        inventory = new Inventory(UseItem);
        uiInventory.SetInventory(inventory);
        uiInventory.SetPlayer(this);
        gold = 1000;
    }

    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Potion:
                usedItemType = Item.ItemType.Potion;
                StartCoroutine(DrinkPotion());
                RemoveItem();
                break;

            case Item.ItemType.Table:
                gridBuildingSystem.SelectItem(Item.ItemType.Table);
                usedItemType = Item.ItemType.Table;
                break;
            case Item.ItemType.WeaponHanger:
                gridBuildingSystem.SelectItem(Item.ItemType.WeaponHanger);
                usedItemType = Item.ItemType.WeaponHanger;
                break;
            case Item.ItemType.PotionStorage:
                gridBuildingSystem.SelectItem(Item.ItemType.PotionStorage);
                usedItemType = Item.ItemType.PotionStorage;
                break;
        }
    }

    void Update()
    {
        Movement();
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StoptoWall();
    }

    private void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;   //회전 속도 멈춤
    }

    private void StoptoWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isFurniture = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Obstacle"));
    }

    private void Movement()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;//방향값이 1로 고정된 벡터 normalized

        if(!isFurniture && health > 0)
        {
            transform.position += moveVec * speed * Time.deltaTime;

            anim.SetBool("isWalk", moveVec != Vector3.zero);
        }

        if(health > 0)
        transform.LookAt(transform.position + moveVec); //방향 바라보기
    }

    private void OnTriggerEnter(Collider collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();   //아이템하고 충돌시
        if(itemWorld != null)
        {
            inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }

        else if (collider.tag == "EnemyBullet")
        {
            Monster monster = collider.GetComponentInParent<Monster>(); //몬스터하고 충돌시
            if (!isDamage)
            {
                StartCoroutine(OnDamage());
            }

            health -= monster.GetDamage();

            if (health <= 0)
            {
                Dead();
                //StartCoroutine(Revive(10f));
            }
        }
    }

    private IEnumerator Revive(float reviveTime)   //부활
    {
        yield return new WaitForSeconds(reviveTime);
        transform.position = respawnZone.position;
        health = 200;
        gameObject.layer = 9;
        gameObject.tag = "Good";
    }

    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }
    }

    private void Dead() //죽는 액션
    {
        anim.SetTrigger("doDie");
        gameObject.layer = 13;
        gameObject.tag = "Dead";
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetGoldAmount()
    {
        return gold;
    }

    public void ObtainGold(int amount)
    {
        gold += amount;
    }

    public void BoughtItem(Item.ItemType itemType)  //아이템 구매
    {
        Debug.Log("you bought" + itemType);

        switch (itemType) 
        {
            case Item.ItemType.BasicSword:      inventory.AddItem(new Item { itemType = Item.ItemType.BasicSword, amount = 1 });    break;
            case Item.ItemType.AdvancedSword:   inventory.AddItem(new Item { itemType = Item.ItemType.AdvancedSword, amount = 1 }); break;
            case Item.ItemType.EmeraldSword:    inventory.AddItem(new Item { itemType = Item.ItemType.EmeraldSword, amount = 1 });  break;
            case Item.ItemType.Potion:          inventory.AddItem(new Item { itemType = Item.ItemType.Potion, amount = 1 });        break;
            case Item.ItemType.Table:           inventory.AddItem(new Item { itemType = Item.ItemType.Table, amount = 1 });         break;
            case Item.ItemType.WeaponHanger:    inventory.AddItem(new Item { itemType = Item.ItemType.WeaponHanger, amount = 1 });  break;

        }
    }

    public void RemoveItem()
    {
        inventory.RemoveItem(new Item { itemType = usedItemType, amount = 1 });
    }

    public void RemoveItem(Item item)
    {
        inventory.RemoveItem(item);
    }

    public bool TrySpendGoldAmount(int spendGoldAmount)
    {
        if (GetGoldAmount() >= spendGoldAmount)
        {
            gold -= spendGoldAmount;
            return true;
        }
        else
            return false;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    private IEnumerator DrinkPotion()  //체력회복이펙트
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.green;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }

        UtilsClass.CreateWorldTextPopup("포션을 마심", transform.position);
        health = 200;
    }

    public bool isWeaponAvailable(Item.ItemType weaponType)
    {
        int b = inventory.GetAmount(Item.ItemType.BasicSword);
        int a = inventory.GetAmount(Item.ItemType.AdvancedSword);
        int e = inventory.GetAmount(Item.ItemType.EmeraldSword);

        switch (weaponType) 
        {
            default: return false;
            case Item.ItemType.BasicSword:
                if (numOfWeaponHanger > 0 && b > 0)
                    return true;
                else
                    return false;
            case Item.ItemType.AdvancedSword:
                if (numOfWeaponHanger > 1 && a > 0)
                    return true;
                else
                    return false;
            case Item.ItemType.EmeraldSword:
                if (numOfWeaponHanger > 2 && e > 0)
                    return true;
                else
                    return false;
        }

    }

    public bool isItemAvailable(Item.ItemType thisItemType)
    {
        int i = inventory.GetAmount(thisItemType);
        if (i > 0)
            return true;
        else
            return false;
    }
}
