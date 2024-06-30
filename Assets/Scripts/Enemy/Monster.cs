using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum Type    //몬스터 타입
    {
        Normal,
        Aggressive,
        Elder,
    }

    public enum State   //행동 상태 정의
    {
        Chasing,
        Dead,
        Knocked,
        Attack,
        Idle,
    }

    public int maxHealth;
    public int health;
    public int damage;
    public int gold;
    public Type monsterType;
    public bool isDead = false;

    public GameManager gameManager;
    public BoxCollider meleeArea;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Material mat;
    public State state;
    public Animator anim;
    public MeshRenderer[] meshes;

    public GameObject target;
    public List<GameObject> attackTarget;
    public List<float> distanceList;
    public GameObject updatingTarget;
    public float shortDis;
    public bool isDamage;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        state = State.Chasing;
        distanceList = new List<float>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        nav.SetDestination(new Vector3(Random.Range(30, 190), 1, Random.Range(-230, -330)));
        /*if (monsterType != Type.Elder)
            FindTarget();
        */

        switch(monsterType)
        {
            case Type.Normal:
                damage = 10;
                gold = 1000;
                break;
            case Type.Aggressive:
                damage = 25;
                gold = 2000;
                break;
            case Type.Elder:
                state = State.Idle;
                damage = 50;
                nav.enabled = false;
                break;
        }


    }

    public void Update()
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

    public void FixedUpdate()
    {
        if (state != State.Dead || state != State.Idle) 
        {
            Targeting();
            
        }
        if(state != State.Idle)
            FreezeVelocity();
    }

    public void Targeting()    //공격 범위 내에 들어오면 공격
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (monsterType)
        {
            case Type.Normal:
                targetRadius = 3f;
                targetRange = 5f;
                break;
            case Type.Aggressive:
                targetRadius = 4f;
                targetRange = 70f;
                break;
            case Type.Elder:
                targetRadius = 8f;
                targetRange = 35f;
                break;
        }


        RaycastHit[] rayHits =  //전방에 구형 레이저 그리기
        Physics.SphereCastAll(transform.position,
                                  targetRadius,
                                  transform.forward,
                                  targetRange,
                                  LayerMask.GetMask("Good"));
        if (rayHits.Length > 0 && state != State.Attack && state != State.Dead) //사거리 내에 있고 공격 중이 아니라면 공격 실행
        {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Attack()    //공격 코루틴
    {
            state = State.Attack;
        switch (monsterType)
        {
            case Type.Normal:
                anim.SetBool("isAttack", true);
                yield return new WaitForSeconds(0.615f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1.468f);
                meleeArea.enabled = false;
                anim.SetBool("isAttack", false);
                break;

            case Type.Aggressive:
                anim.SetBool("isRush", true);
                yield return new WaitForSeconds(0.8736f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1.5f);
                anim.SetBool("isRush", false);
                break;

            case Type.Elder:
                anim.SetBool("isWalk", false);
                anim.SetBool("isAttack", true);
                yield return new WaitForSeconds(0.5f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(0.5f);
                anim.SetBool("isAttack", false);
                break;
        }
        
        if(state != State.Dead)
            ChaseStart();
    }

    IEnumerator OnDamage()  //피격 액션
    {
        isDamage = true;
        if (health > 0)     //피가 0보다 많으면 피격액션
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);

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

    public void FreezeVelocity()
    {
        rigid.angularVelocity = Vector3.zero;   //회전 속도 멈춤
        rigid.velocity = Vector3.zero;
    }

    public void ChaseStart()   //추격시작하면 Stopped false하고 추격상태
    {
        nav.isStopped = false;
        state = State.Chasing;
    }

    public void StopChasing()  //Stopped만 false로
    {
        nav.isStopped = true;
    }

    public void Dead() //죽는 액션
    {
        state = State.Dead;
        boxCollider.enabled = false;
        isDead = true;
        meleeArea.enabled = false;
        meleeArea.tag = "DeadMonster";
        nav.isStopped = true;
        anim.SetTrigger("doDie");
        gameObject.layer = 12;
        gameObject.tag = "DeadMonster";

        if (monsterType != Type.Elder)
            Destroy(gameObject, 3);
        else
            Destroy(gameObject, 5);
    }


    public void OnTriggerEnter(Collider collider)  //피격 판정
    {
        if(collider.tag == "Weapon")
        {
            MeleeAreaScript meleeAreaScript = collider.GetComponent<MeleeAreaScript>();
            Warrior warriorScript = collider.gameObject.GetComponentInParent<Warrior>();

            if (!isDamage)
            {
                StartCoroutine(OnDamage());
                health -= meleeAreaScript.GetDamage();
                if (health <= 0) 
                {
                    warriorScript.ObtainGold(gold);
                    Dead();
                }  
            }
        }
    }

    public void AddTarget(GameObject gameObject)
    {
        attackTarget.Add(gameObject);
    }

    public void FindTarget()
    {
        attackTarget = new List<GameObject>(GameObject.FindGameObjectsWithTag("Good"));
        foreach (GameObject g in attackTarget)
            distanceList.Add(0f);
        shortDis = Vector3.Distance(gameObject.transform.position, attackTarget[0].transform.position); //첫번째를 ShortDistance로 잡아주기
        distanceList[0] = shortDis;

        target = attackTarget[0];
    }

    public void UpdateTarget()
    {
        int n = 0;
        foreach (GameObject found in attackTarget)  //어택 타겟들의 게임오브젝트들에서
        {
            if (found == null || found.layer == 13)
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
            if (distance == 0)
            {
                distanceList.Remove(distance);
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
        if (n == attackTarget.Count) //카운트 다했는데도 가까운게 없으면
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
            StopChasing();
        }
    }

    public int GetDamage()
    {
        return damage;
    }
}
