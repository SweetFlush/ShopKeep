using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAreaScript : MonoBehaviour
{
    public int damage;  //������ 20, ���꽺 50, ���޶��� 120

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
