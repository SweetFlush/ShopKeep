using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class Warrior : MonoBehaviour
{
    public enum State
    {
        Idle,
        GoHome,
        Shopping,
        Chasing,
        Attack,
        Knocked,
        Dead,
    }

    public int maxHealth;
    public int health;
    public int gold;
    private bool isDamage;
    public bool basic;
    public bool advance;
    public bool emerald;

    public Player player;
    public BoxCollider meleeArea;
    public MeleeAreaScript meleeAreaScript;
    public GameObject cashierZone;
    public GameObject homeZone;

    public State state;
    public TrailRenderer trail;
    private Rigidbody rigid;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private NavMeshAgent nav;
    private Material mat;
    private Animator anim;
    private MeshRenderer[] meshes;

    public GameObject updatingTarget;
    public GameObject target;
    public List<GameObject> attackTarget;
    public List<float> distanceList;
    private float shortDis;

    private Item basicSword;
    private Item advancedSword;
    private Item emeraldSword;
    private Item potion;
    private List<GameObject> weaponList;
    public List<Item> pocket;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        distanceList = new List<float>();
        weaponList = new List<GameObject>();
        pocket = new List<Item>();

        maxHealth = 200;
        health = 200;

        basicSword = new Item { itemType = Item.ItemType.BasicSword, amount = 1 };
        advancedSword = new Item { itemType = Item.ItemType.AdvancedSword, amount = 1 };
        emeraldSword = new Item { itemType = Item.ItemType.EmeraldSword, amount = 1 };
        potion = new Item {itemType = Item.ItemType.Potion, amount = 1 };

        weaponList.Add(transform.Find("bodyBone").Find("Arm_R").Find("Tool_R").Find("BasicSword").gameObject);
        weaponList.Add(transform.Find("bodyBone").Find("Arm_R").Find("Tool_R").Find("AdvancedSword").gameObject);
        weaponList.Add(transform.Find("bodyBone").Find("Arm_R").Find("Tool_R").Find("EmeraldSword").gameObject);

        //테스트 초깃값 설정
        EquipWeapon();
        gold = 700;
    }

    private void FullArmed()
    {
        pocket.Add(emeraldSword);
        pocket.Add(new Item { itemType = Item.ItemType.Potion, amount = 10});
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.Chasing:
                if (target != null)
                {
                    UpdateTarget();
                    anim.SetBool("isWalk", true);
                }
                break;
            case State.Shopping:
                anim.SetBool("isWalk", true);
                nav.SetDestination(cashierZone.transform.position);
                break;
            case State.GoHome:  //집으로 간다
                anim.SetBool("isWalk", true);
                if(nav.isActiveAndEnabled == true)
                    nav.SetDestination(homeZone.transform.position);
                break;
            case State.Dead:
                anim.SetBool("isWalk", false);
                StopChasing();
                Dead();
                break;
            case State.Attack:
                anim.SetBool("isWalk", false);
                StopChasing();
                break;
            
        }

    }
    
    private void FixedUpdate()
    {
        if (state != State.Dead)
            Targeting();
        FreezeVelocity();
    }

    private void Targeting()
    {
        float targetRadius = 4f;
        float targetRange = 6f;

        RaycastHit[] rayHits =  //전방에 구형 레이저 그리기
            Physics.SphereCastAll(transform.position,
                                  targetRadius,
                                  transform.forward,
                                  targetRange,
                                  LayerMask.GetMask("Evil"));
        if(rayHits.Length > 0 && state != State.Attack) //사거리 내에 있고 공격 중이 아니라면 공격 실행
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        state = State.Attack;
        trail.enabled = true;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        trail.enabled = false;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        anim.SetBool("isAttack", false);

        if(state != State.Dead)
            ChaseStart();
    }

    private IEnumerator OnDamage()
    {
        isDamage = true;
        if (health > 0)     //피가 0보다 많으면 피격액션
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.2f);

            isDamage = false;
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.gray;
            }
        }
    }

    private IEnumerator HealedEffect()  //체력회복이펙트
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
    }

    private void FreezeVelocity()
    {
        rigid.angularVelocity = Vector3.zero;   //회전 속도 멈춤
        rigid.velocity = Vector3.zero;
    }

    private void ChaseStart()   //추격시작하면 Stopped false하고 추격상태
    {
        nav.isStopped = false;
        state = State.Chasing;
    }

    private void StopChasing()  //Stopped만 false로
    {
        nav.isStopped = true;
    }

    private void Idle()
    {
        nav.isStopped = true;
        anim.SetBool("isWalk", false);
        anim.SetBool("isAttack", false);
        state = State.Idle;
    }


    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "EnemyBullet")   //피격판정
        {
            Monster monster = collider.GetComponentInParent<Monster>();
            if (!isDamage)
            {
                StartCoroutine(OnDamage());
            }
            health -= monster.GetDamage();

            if (health <= 50)
                DrinkPotion();

            if (health <= 0) 
            {
                Dead();
            } 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CashierZone")
        {
            Shopping();
        }

        else if (collision.gameObject.tag == "HomeZone" && state == State.GoHome)
        {
            state = State.Idle;
            gameObject.SetActive(false);
            nav.enabled = false;
            
        }
    }

    private void Shopping()               //쇼핑하기
    {
        if (!CheckItem(Item.ItemType.EmeraldSword) && gold >= 5000 && emerald && player.isItemAvailable(Item.ItemType.EmeraldSword))
            StartCoroutine(BuyItem(Item.ItemType.EmeraldSword, 1, 5000));

        else if (!CheckItem(Item.ItemType.AdvancedSword) && gold >= 2000 && advance && player.isItemAvailable(Item.ItemType.AdvancedSword))
            StartCoroutine(BuyItem(Item.ItemType.AdvancedSword, 1, 2000));

        else if (!CheckItem(Item.ItemType.BasicSword) && gold >= 500 && basic && player.isItemAvailable(Item.ItemType.BasicSword))
            StartCoroutine(BuyItem(Item.ItemType.BasicSword, 1, 500));

        while (CheckPotion())
        {
            if (!CheckPotion() || !player.isItemAvailable(Item.ItemType.Potion) || gold < 100)
                break;
            StartCoroutine(BuyItem(Item.ItemType.Potion, 1, 100));
        }


        EquipWeapon();

        nav.enabled = true;
        state = State.GoHome;
        anim.SetBool("isWalk", true);
    }

    private bool CheckPotion()
    {
        Item potions = new Item { itemType = Item.ItemType.Potion, amount = 0};
        foreach (Item pocketItem in pocket)
        {
            if (pocketItem.itemType == Item.ItemType.Potion)
            {
                potions = pocketItem;
            }
        }
        if (potions.amount < 10)
            return true;
        else
            return false;
    }

    private bool CheckItem(Item.ItemType searchingType)
    {
        Item searchItem = new Item { itemType = searchingType, amount = 0 };
        foreach (Item pocketItem in pocket)
        {
            if (pocketItem.itemType == searchingType)
            {
                searchItem = pocketItem;
            }
        }
        if (searchItem.amount > 0)
            return true;
        else 
            return false;
    }

    private IEnumerator BuyItem(Item.ItemType boughtItemType, int amount, int price)
    {
        anim.SetBool("isWalk", false);
        bool hasPotion = false;
        if (boughtItemType == Item.ItemType.Potion)  //포션이면 스택해야하니까 true
        {
            foreach (Item pocketItem in pocket)
            {
                if(pocketItem.itemType == Item.ItemType.Potion)
                    hasPotion = true;
            }
        }
        else
            pocket.Add(new Item { itemType = boughtItemType, amount = amount });    //무기면 추가

        if(hasPotion)   //포션스택해야되면
        {
            foreach (Item pocketItem in pocket)
            {
                if (pocketItem.itemType == Item.ItemType.Potion)
                {
                    pocketItem.amount++;    //스택쌓기
                }
            }
        }
        else
            pocket.Add(new Item { itemType = boughtItemType, amount = amount });    //아니면 새로 생성

        gold -= price;
        player.ObtainGold(price);
        player.RemoveItem(new Item { itemType = boughtItemType, amount = amount });

        string itemName = "아이템";
        switch (boughtItemType)
        {
            case Item.ItemType.BasicSword:
                itemName = "베이직 소드";
                break;
            case Item.ItemType.AdvancedSword:
                itemName = "어드밴스 소드";
                break;
            case Item.ItemType.EmeraldSword:
                itemName = "에메랄드 소드";
                break;
            case Item.ItemType.Potion:
                itemName = "포션";
                break;
        }
        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", true);
    }

    private void Dead() //죽는 액션
    {
        state = State.Dead;
        meleeArea.enabled = false;
        meleeArea.tag = "Dead";
        anim.SetTrigger("doDie");
        capsuleCollider.enabled = false;
        gameObject.layer = 13;
        gameObject.tag = "Dead";
    }

    private IEnumerator Revive(float reviveTime)   //부활
    {
        UtilsClass.CreateWorldTextPopup("쓰러짐!", transform.position);
        yield return new WaitForSeconds(1f);
        UtilsClass.CreateWorldTextPopup(reviveTime + "초 뒤에 부활", transform.position);
        yield return new WaitForSeconds(reviveTime - 1f);
        transform.position = homeZone.transform.position;
        health = 200;
        gameObject.layer = 9;
        gameObject.tag = "Good";
        ChaseStart();
    }

    private void DrinkPotion()  //포션 마시기
    {
        if(TryDrinkPotion())
        {
            Item itemInPocket = null;
            foreach (Item pocketItem in pocket)
            {
                if (pocketItem.itemType == Item.ItemType.Potion && pocketItem.amount >= 1)
                {
                    pocketItem.amount--;
                    health = 200;
                    itemInPocket = pocketItem;

                    StartCoroutine(HealedEffect());
                    UtilsClass.CreateWorldTextPopup("포션을 마심", transform.position);
                }
            }
            if (itemInPocket == null && itemInPocket.amount <= 0)
                pocket.Remove(potion);
        }
    }

    private bool TryDrinkPotion()
    {
        bool available = false;
        foreach (Item pocketItem in pocket)
        {
            if (pocketItem.itemType == Item.ItemType.Potion && pocketItem.amount >= 1)
            {
                available = true;
            }
        }

        return available;
    }

    private void EquipWeapon()  //무기 장착, 바꿔끼기
    {
        bool isEmerald = false;
        bool isAdvance = false;
        bool isBasic = false;

        foreach (Item itemInPocket in pocket)
        {
            switch (itemInPocket.itemType)
            {
                case Item.ItemType.EmeraldSword:
                    isEmerald = true;
                    break;
                case Item.ItemType.AdvancedSword:
                    isAdvance = true;
                    break;
                case Item.ItemType.BasicSword:
                    isBasic = true;
                    break;
            }
        }
        if (isEmerald)
        {
            weaponList[0].SetActive(false);
            weaponList[1].SetActive(false);
            weaponList[2].SetActive(true);
            meleeAreaScript.SetDamage(120);
        }
        else if (isAdvance)
        {
            weaponList[0].SetActive(false);
            weaponList[1].SetActive(true);
            meleeAreaScript.SetDamage(50);
        }
        else if (isBasic)
        {
            weaponList[0].SetActive(true);
            meleeAreaScript.SetDamage(20);
        }
        else    //맨손
        {
            weaponList[0].SetActive(false);
            weaponList[1].SetActive(false);
            weaponList[2].SetActive(false);
            meleeAreaScript.SetDamage(5);
        }
    }

    public void ObtainGold(int amount)
    {
        UtilsClass.CreateWorldTextPopup("+ " + amount.ToString(), transform.position);
        gold += amount;
    }

    public void AddTarget(GameObject gameObject)
    {
        attackTarget.Add(gameObject);
    }

    public void FindTarget()
    {
        attackTarget = new List<GameObject>(GameObject.FindGameObjectsWithTag("Monster"));
        foreach (GameObject g in attackTarget)
            distanceList.Add(0f);
        shortDis = Vector3.Distance(gameObject.transform.position, attackTarget[0].transform.position); //첫번째를 ShortDistance로 잡아주기
        distanceList[0] = shortDis;

        target = attackTarget[0];
        nav.SetDestination(target.transform.position);
    }

    public void UpdateTarget()
    {
        int n = 0;
        foreach (GameObject found in attackTarget)  //어택 타겟들의 게임오브젝트들에서
        {
            if (found == null || found.layer == 12)
            {
                distanceList.Remove(distanceList[attackTarget.IndexOf(found)]);
                attackTarget.Remove(found);
            }
            //현재의 거리를 계산하여 저장
            else
                distanceList[attackTarget.IndexOf(found)] = Vector3.Distance(gameObject.transform.position, found.transform.position);
        }

        foreach (float distance in distanceList)    //모든 거리값을 확인하여 최소거리 구한다
        {
            if(distance == 0)
            {
                distanceList.Remove(distance);
                continue;
            }
            if (distance < shortDis)
            {
                int index = distanceList.IndexOf(distance);
                shortDis = distance;    //최소거리를 distance로
                updatingTarget = attackTarget[index];
            }
            else
                n++;
        }

        if(n == attackTarget.Count) //카운트 다했는데도 가까운게 없으면
        {
            shortDis = 100000f;
            n = 0;
        }
        if (updatingTarget != null)
        {
            target = updatingTarget;
            nav.SetDestination(target.transform.position);
        }
        else
        {
            foreach (float distance in distanceList)
            {
                if (distance == 0)
                {
                    distanceList.Remove(distance);
                    continue;
                }
            }
        }
    }

    public void ReadyToFight(Vector3 position)
    {
        gameObject.SetActive(true);
        anim.SetBool("isWalk", false);
        nav.enabled = false;
        state = State.Idle;
        transform.position = position;

    }

    public void Fight()
    {
        nav.enabled = true;
        FindTarget();
        state = State.Chasing;
    }

    public void RestInHome()
    {
        nav.enabled = false;
        state = State.Idle;
        health = 200;
        gameObject.SetActive(false);
        distanceList.Clear();
    }

    public void GoShopping(bool basic, bool advance, bool emerald)
    {
        this.basic = basic;
        this.advance = advance;
        this.emerald = emerald;

        transform.position = new Vector3(homeZone.transform.position.x,
                                        homeZone.transform.position.y,
                                        homeZone.transform.position.z-20);
        gameObject.SetActive(true);
        nav.enabled = true;
        nav.SetDestination(cashierZone.transform.position);
        state = State.Shopping;

        
    }

    public int GetPotionAmount()
    {
        Item potions = new Item { itemType = Item.ItemType.Potion, amount = 0 };
        foreach (Item pocketItem in pocket)
        {
            if (pocketItem.itemType == Item.ItemType.Potion)
            {
                potions = pocketItem;
            }
        }
        return potions.amount;
    }
}
