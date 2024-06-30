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
            Monster monster = collider.GetComponentInParent<Monster>(); //몬스터하고 충돌시
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
        if (health > 0)     //피가 0보다 많으면 피격액션
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
