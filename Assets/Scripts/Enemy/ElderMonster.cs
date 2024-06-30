using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElderMonster : Monster
{
    public GameObject aggressiveMonster;
    public GameObject monster;
    public List<GameObject> monsterList;
    public List<GameObject> agsvMonsterList;
    public bool isMonsterZero = false;
    public bool isAgsvMonsterZero = false;

    private Transform mouthTransform;

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

        monsterList = new List<GameObject>();
        agsvMonsterList = new List<GameObject>();

        mouthTransform = transform.Find("Mouth");
        health = 2000;
        maxHealth = 2000;

    }

    private void Start()
    {
        //StartCoroutine(SpawnMobs());
    }

    private IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randomAction = UnityEngine.Random.Range(0, 5);  //0 1 2 3 4
        switch (randomAction)
        {
            case 0:
            case 1: 
            case 2:
            case 3: //평소
                break;
            case 4: //점프 후 미사일 발사
                break;
        }
    }

    private IEnumerator JumpShot()
    {
        yield return null;
    }

    public void RoundStart(int round, int monsters, int agrvMonsters)
    {
        StartCoroutine(SpawnMobs(monsters, agrvMonsters));
    }

    private IEnumerator SpawnMobs(int monsters, int agrvMonsters)   //몬스터 스폰
    {
        for (int i = 0; i < monsters; i++)
        {
            GameObject thisMonster = Instantiate(monster, mouthTransform.position, Quaternion.identity);
            thisMonster.tag = "Monster";
            thisMonster.transform.Find("EnemyBullet").tag = "EnemyBullet";
            thisMonster.layer = 10;
            monsterList.Add(thisMonster);

            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 0; i < agrvMonsters; i++)
        {
            GameObject thisMonster = Instantiate(aggressiveMonster, mouthTransform.position, Quaternion.identity);
            thisMonster.tag = "Monster";
            thisMonster.transform.Find("EnemyBullet").tag = "EnemyBullet";
            thisMonster.layer = 10;
            agsvMonsterList.Add(thisMonster);

            yield return new WaitForSeconds(0.05f);
        }

    }

    public void BattleStart()       //공격시작
    {
        foreach(GameObject thismonster in monsterList)
        {
            Monster monsterScript = thismonster.GetComponent<Monster>();
            monsterScript.FindTarget();
            monsterScript.state = State.Chasing;
        }
        foreach (GameObject thismonster in agsvMonsterList)
        {
            Monster monsterScript = thismonster.GetComponent<Monster>();
            monsterScript.FindTarget();
            monsterScript.state = State.Chasing;
        }
    }

    public bool isBattleEnd()   //전투 끝났는지 검사
    {
        isMonsterZero = false;
        isAgsvMonsterZero = false;
        int n = 0;
        if (monsterList.Count == 0)
            isMonsterZero = true;
        if (agsvMonsterList.Count == 0)
            isAgsvMonsterZero = true;

        foreach (GameObject monster in monsterList)
        {
            if (CheckReference(monster))
                n++;
        }
        if (n == monsterList.Count)
            isMonsterZero = true;
        n = 0;
        foreach (GameObject monster in agsvMonsterList)
        {
            if (CheckReference(monster))
                n++;
        }
        if (n == agsvMonsterList.Count)
            isAgsvMonsterZero = true;

        if (isMonsterZero && isAgsvMonsterZero)
        {
            monsterList.Clear();
            agsvMonsterList.Clear();
            return true;
        }
        else
            return false;

    }

    public bool CheckReference(GameObject reference)
    {
        try
        {
            var blarf = reference.name;
            return false;
        }
        catch (MissingReferenceException) // General Object like GameObject/Sprite etc
        {
            return true;
        }
        catch (MissingComponentException) // Specific for objects of type Component
        {
            return true;
        }
        catch (UnassignedReferenceException) // Specific for unassigned fields
        {
            return true;
        }
        catch (NullReferenceException) // Any other null reference like for local variables
        {
            return true;
        }

    }

    public void Bossfight()
    {
        gameObject.layer = 10;
        gameObject.tag = "Monster";
        state = State.Chasing;
        nav.enabled = true;
        FindTarget();
    }
}
