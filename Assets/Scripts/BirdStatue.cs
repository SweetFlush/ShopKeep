using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdStatue : MonoBehaviour
{
    public GameManager gameManager;

    public int maxHealth;
    public int health;
    public bool isDamage;
    public bool isGameOver;

    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        isGameOver = false;

        health = 500;
        maxHealth = 500;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "EnemyBullet")
        {
            Monster monster = collider.GetComponentInParent<Monster>(); //�����ϰ� �浹��
            if (!isDamage)
            {
                StartCoroutine(OnDamage());
            }

            health -= monster.GetDamage();

            if (health <= 0)
            {
                isGameOver = true;
            }
        }
    }

    private IEnumerator OnDamage()
    {
        isDamage = true;
        if (health > 0)     //�ǰ� 0���� ������ �ǰݾ׼�
        {
            mesh.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            isDamage = false;

            mesh.material.color = Color.white;
        }
        else
        {
            mesh.material.color = Color.gray;
        }
    }
}
