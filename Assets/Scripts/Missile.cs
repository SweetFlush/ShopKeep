using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Missile : MonoBehaviour
{
    public int damage;

    private GameObject target;
    public List<GameObject> attackTarget;   //공격 대상들 리스트
    private List<float> distanceList;       //대상들까지의 거리
    private GameObject updatingTarget;      //거리에 따라 공격 대상 업데이트
    private float shortDis;                 //짧은 거리 담는 변수

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject, 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        FindTarget();
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

    public Transform UpdateTarget()
    {
        int n = 0;
        foreach (GameObject found in attackTarget)  //어택 타겟들의 게임오브젝트들에서
        {
            if (found == null)
            {
                distanceList.Remove(attackTarget.IndexOf(found));
                attackTarget.Remove(found);
            }

            //현재의 거리를 계산하여 저장
            distanceList[attackTarget.IndexOf(found)] = Vector3.Distance(gameObject.transform.position, found.transform.position);
        }

        foreach (float distance in distanceList)    //모든 거리값을 확인하여 최소거리 구한다
        {
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

        if(updatingTarget == null)
        {
            Destroy(gameObject);
            return null;
        }
        else
            return updatingTarget.transform;
    }
}
