using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Missile : MonoBehaviour
{
    public int damage;

    private GameObject target;
    public List<GameObject> attackTarget;   //���� ���� ����Ʈ
    private List<float> distanceList;       //��������� �Ÿ�
    private GameObject updatingTarget;      //�Ÿ��� ���� ���� ��� ������Ʈ
    private float shortDis;                 //ª�� �Ÿ� ��� ����

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
        shortDis = Vector3.Distance(gameObject.transform.position, attackTarget[0].transform.position); //ù��°�� ShortDistance�� ����ֱ�
        distanceList[0] = shortDis;

        target = attackTarget[0];
    }

    public Transform UpdateTarget()
    {
        int n = 0;
        foreach (GameObject found in attackTarget)  //���� Ÿ�ٵ��� ���ӿ�����Ʈ�鿡��
        {
            if (found == null)
            {
                distanceList.Remove(attackTarget.IndexOf(found));
                attackTarget.Remove(found);
            }

            //������ �Ÿ��� ����Ͽ� ����
            distanceList[attackTarget.IndexOf(found)] = Vector3.Distance(gameObject.transform.position, found.transform.position);
        }

        foreach (float distance in distanceList)    //��� �Ÿ����� Ȯ���Ͽ� �ּҰŸ� ���Ѵ�
        {
            if (distance < shortDis)
            {
                int index = distanceList.IndexOf(distance);
                shortDis = distance;    //�ּҰŸ��� distance��
                updatingTarget = attackTarget[index];
            }
            else
                n++;
        }

        if (n == attackTarget.Count) //ī��Ʈ ���ߴµ��� ������ ������
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
