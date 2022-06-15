using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldText : MonoBehaviour
{
    public Player player;
    private int gold;
    private Text text;

    private void Awake()
    {
        gold = player.GetGoldAmount();
        text = GetComponent<Text>();
    }

    private void Update()
    {
        gold = player.GetGoldAmount();
        text.text = gold.ToString();
    }
}
