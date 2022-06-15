using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAreaScript : MonoBehaviour
{
    public int damage;  //베이직 20, 어드밴스 50, 에메랄드 120

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
