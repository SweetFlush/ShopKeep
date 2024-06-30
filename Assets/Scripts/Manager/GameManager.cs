using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameCam;
    public GameObject menuCam;
    public GameObject menuScreenDeco;
    public GameObject elderMonsterCam;
    public GameObject victoryCam;
    public GameObject victoryDeco;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    public GameObject roundStartButton;
    public GameObject openShopButton;
    public GameObject respawnZone;
    public GameObject elderMonsterWarining;
    public GameObject monsterIcon;
    public GameObject agsvMonsterIcon;

    public Warrior warrior;
    public Warrior smileWarrior;
    public Warrior silNunWarrior;

    public Player player;
    public BirdStatue birdStatue;
    public GameObject monster;
    public GameObject aggressiveMonster;
    public GameObject elderMonster;
    public GameObject readyZone1;
    public GameObject readyZone2;
    public GameObject readyZone3;
    public GameObject playerReadyZone;


    public Text roundCount;
    public Text numberOfMonster;
    public Text numberOfAggressiveMonster;
    public Text playerHealthText;
    public Text playerGoldText;
    public Text warriorHealthText;
    public Text warriorGoldText;
    public Text warriorPotionText;
    public Text smileWarriorHealthText;
    public Text smileWarriorGoldText;
    public Text smileWarriorPotionText;
    public Text silNunWarriorHealthText;
    public Text silNunWarriorGoldText;
    public Text silNunWarriorPotionText;

    public RectTransform elderMonsterHealthGroup;
    public RectTransform elderMonsterHealthBar;
    public RectTransform birdStatueHealthGroup;
    public RectTransform birdStatueHealthBar;

    public Text gameOverText;

    public bool isBattle;
    public bool isPeaceful;
    public bool isBasicAvailable;
    public bool isAdvanceAvailable;
    public bool isEmeraldAvailable;
    public int round = 1;
    public int lastStatueHealth;

    public List<MonstersInThisRound> monsterSpawnList;

    private ElderMonster elderMonsterScript;

    private void Awake()
    {
        elderMonsterScript = elderMonster.GetComponent<ElderMonster>();
        monsterSpawnList = new List<MonstersInThisRound>();

        monsterSpawnList.Add(new MonstersInThisRound(5, 0));        //1
        monsterSpawnList.Add(new MonstersInThisRound(8, 0));        //2
        monsterSpawnList.Add(new MonstersInThisRound(5, 5));        //3
        monsterSpawnList.Add(new MonstersInThisRound(10, 10));        //4
        monsterSpawnList.Add(new MonstersInThisRound(10, 12));        //5

        //���� ����ȭ��
        menuCam.SetActive(true);
        gameCam.SetActive(false);
        menuScreenDeco.SetActive(true);

        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        player.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (round >= 11)    //����ǥ��
        {
            monsterIcon.SetActive(false);
            agsvMonsterIcon.SetActive(false);
            numberOfMonster.gameObject.SetActive(false);
            numberOfAggressiveMonster.gameObject.SetActive(false);
            elderMonsterWarining.SetActive(true);

        }
        else
        {
            numberOfMonster.text = monsterSpawnList[round - 1].GetMonster().ToString();
            numberOfAggressiveMonster.text = monsterSpawnList[round - 1].GetAggressiveMonster().ToString();
        }
    }

    public void GameStart()
    {
        //���ӽ���
        menuCam.SetActive(false);
        gameCam.SetActive(true);
        menuScreenDeco.SetActive(false);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        elderMonsterHealthGroup.gameObject.SetActive(false);
        birdStatueHealthGroup.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
    }

    public void RoundStart()    //���� ��ŸƮ��ư
    {
        roundStartButton.SetActive(false);
        openShopButton.SetActive(false);
        player.transform.position = playerReadyZone.transform.position;
        elderMonsterCam.SetActive(true);
        gameCam.SetActive(false);
        gamePanel.SetActive(false);

        StartCoroutine(ReadyToFight());
    }

    private IEnumerator ReadyToFight()  //���� �غ��ϴ� �ڷ�ƾ
    {
        warrior.ReadyToFight(readyZone1.transform.position);
        smileWarrior.ReadyToFight(readyZone2.transform.position);
        silNunWarrior.ReadyToFight(readyZone3.transform.position);
        warrior.gameObject.SetActive(true);
        smileWarrior.gameObject.SetActive(true);
        silNunWarrior.gameObject.SetActive(true);

        if(round != 6)
        {
            elderMonsterScript.anim.SetTrigger("doSpawnMobs");
            yield return new WaitForSeconds(2.5f);
            int monsters = monsterSpawnList[round - 1].GetMonster();
            int aggressiveMonsters = monsterSpawnList[round - 1].GetAggressiveMonster();
            elderMonsterScript.RoundStart(round, monsters, aggressiveMonsters);    //�������Ϳ��� �����϶� ���
        }
        else
        {
            elderMonsterScript.Bossfight();
            elderMonsterHealthGroup.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(4f);

        elderMonsterCam.SetActive(false);
        gameCam.SetActive(true);
        gamePanel.SetActive(true);
        yield return new WaitForSeconds(1f);

        BattleStart();
    }

    public void BattleStart()   //���� ����
    {
        roundStartButton.SetActive(false);
        openShopButton.SetActive(false);

        isBattle = true;
        isPeaceful = false;

        elderMonsterScript.BattleStart();
        warrior.Fight();
        smileWarrior.Fight();
        silNunWarrior.Fight();
    }

    public void BattleEnd() //���� ����
    {
        roundStartButton.SetActive(true);
        openShopButton.SetActive(true);

        warrior.RestInHome();
        smileWarrior.RestInHome();
        silNunWarrior.RestInHome();
        player.transform.position = respawnZone.transform.position;
        isBattle = false;
        isPeaceful = true;
        round++;

        if (round >= 6)
        {
            monsterIcon.SetActive(false);
            agsvMonsterIcon.SetActive(false);
            numberOfMonster.gameObject.SetActive(false);
            numberOfAggressiveMonster.gameObject.SetActive(false);
            elderMonsterWarining.SetActive(true);

        }
        else
        {
            numberOfMonster.text = monsterSpawnList[round - 1].GetMonster().ToString();
            numberOfAggressiveMonster.text = monsterSpawnList[round - 1].GetAggressiveMonster().ToString();
        }
    }

    public void OpenShop()
    {
        openShopButton.SetActive(false);
        isBasicAvailable = player.isWeaponAvailable(Item.ItemType.BasicSword);
        isAdvanceAvailable = player.isWeaponAvailable(Item.ItemType.AdvancedSword);
        isEmeraldAvailable = player.isWeaponAvailable(Item.ItemType.EmeraldSword);

        warrior.GoShopping(isBasicAvailable, isAdvanceAvailable, isEmeraldAvailable);
        smileWarrior.GoShopping(isBasicAvailable, isAdvanceAvailable, isEmeraldAvailable);
        silNunWarrior.GoShopping(isBasicAvailable, isAdvanceAvailable, isEmeraldAvailable);
    }

    private void LateUpdate()
    {
        roundCount.text = "Round: " + round;

        playerGoldText.text = string.Format("{0:n0}", player.GetGoldAmount());  //��差
        playerHealthText.text = player.health + " / " + player.maxHealth;       //ü�·�
        
        //���� ����â ������Ʈ
        warriorHealthText.text = warrior.health.ToString();
        smileWarriorHealthText.text = smileWarrior.health.ToString();
        silNunWarriorHealthText.text = silNunWarrior.health.ToString();
        warriorGoldText.text = warrior.gold.ToString();
        smileWarriorGoldText.text = smileWarrior.gold.ToString();
        silNunWarriorGoldText.text = silNunWarrior.gold.ToString();
        warriorPotionText.text = warrior.GetPotionAmount().ToString();
        smileWarriorPotionText.text = smileWarrior.GetPotionAmount().ToString();
        silNunWarriorPotionText.text = silNunWarrior.GetPotionAmount().ToString();

        if(round >= 6)
            elderMonsterHealthBar.localScale = new Vector3((float)elderMonsterScript.health / elderMonsterScript.maxHealth, 1, 1);
        if (elderMonsterScript.health <= 0)
            elderMonsterHealthBar.gameObject.SetActive(false);

        if(lastStatueHealth != birdStatue.health)
        {
            StartCoroutine(StatueDamaged());
            birdStatueHealthGroup.gameObject.SetActive(true);
            birdStatueHealthBar.localScale = new Vector3((float)birdStatue.health / birdStatue.maxHealth, 1, 1);
            lastStatueHealth = birdStatue.health;
            if (birdStatue.health <= 0)
                birdStatueHealthGroup.gameObject.SetActive(false);
        }

        if (isBattle == true && round < 6)
        {
            if (elderMonsterScript.isBattleEnd())
                BattleEnd();
        }
        else if (CheckReference(elderMonster))
            Victory();

        if (birdStatue.health <= 0)
            GameOver();
    }

    private IEnumerator StatueDamaged()
    {
        yield return new WaitForSeconds(3f);
        if (lastStatueHealth == birdStatue.health)
            birdStatueHealthGroup.gameObject.SetActive(false);
    }

    public class MonstersInThisRound
    {
        private int numOfMonster;
        private int numOfAggressiveMonster;

        public MonstersInThisRound(int m, int a)
        {
            this.numOfMonster = m;
            this.numOfAggressiveMonster = a;
        }

        public int GetMonster()
        {
            return numOfMonster;
        }

        public int GetAggressiveMonster()
        {
            return numOfAggressiveMonster;
        }
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

    public void Victory()
    {
        gamePanel.SetActive(false);
        gameCam.SetActive(false);
        victoryCam.SetActive(true);
        victoryPanel.SetActive(true);
        victoryDeco.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverText.text = "Ŭ������ ���� : " + round;

    }

    public void Restart()
    {
        gameOverPanel.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
