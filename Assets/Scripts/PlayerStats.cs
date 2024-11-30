using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    // Player stats
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;

    private int level = 1;
    private int exp = 0;
    public int maxExp = 100;

    
    public UIManager uiManager;
    public GameObject attackGameObject;

    void Start()
    {

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager is not assigned and could not be found in the scene!");
                return;
            }
        }


        if (attackGameObject == null)
        {
            Debug.LogError("Attack GameObject is not assigned in PlayerStats!");
            return;
        }

      
        attackGameObject.SetActive(false);
        if (photonView.IsMine) 
        {
            UpdateUI(); 
        }
    }

    void Update()
    {
    
        if (photonView.IsMine)
        {
            
            if (exp >= maxExp)
            {
                LevelUp();
                exp = 0; 
            }

        
            if (Input.GetMouseButtonDown(0))
            {
                attackGameObject.SetActive(true);
            }
        }
    }

    public void GainExperience(int amount)
    {
        exp += amount;

      
        if (exp >= maxExp)
        {
            LevelUp();
            exp = 0; 
        }

      
        UpdateUI();
        photonView.RPC("UpdatePlayerStats", RpcTarget.AllBuffered, level, attackDamage, attackSpeed, attackRange);
    }

    public void ApplyTalent(TalentType talent)
    {
        
        switch (talent)
        {
            case TalentType.IncreaseAttackDamage:
                attackDamage += 5f;
                Debug.Log("Player has Increased Damage");
                break;

            case TalentType.IncreaseAttackSpeed:
                attackSpeed += 0.2f;
                Debug.Log("Player has Increased Attack Speed");
                break;

            case TalentType.IncreaseAttackRange:
                attackRange += 0.5f;
                Debug.Log("Player has Increased Range");

                if (attackGameObject != null)
                {
                    Vector3 newScale = attackGameObject.transform.localScale;
                    newScale.x += 0.5f;
                    newScale.y += 0.5f;
                    attackGameObject.transform.localScale = newScale;
                }
                break;
        }

      
        UpdateUI();
        photonView.RPC("UpdatePlayerStats", RpcTarget.AllBuffered, level, attackDamage, attackSpeed, attackRange);
    }

    private void LevelUp()
    {
        level++;
        Debug.Log($"Level Up! New Level: {level}");

       
        TalentType newTalent = (TalentType)Random.Range(0, System.Enum.GetValues(typeof(TalentType)).Length);
        ApplyTalent(newTalent);

       
        uiManager.DisplayTalent(newTalent);
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLevelUI(level, attackDamage, attackSpeed, attackRange);
        }
        else
        {
            Debug.LogWarning("UIManager is not assigned, cannot update UI!");
        }
    }

    
    [PunRPC]
    public void UpdatePlayerStats(int level, float attackDamage, float attackSpeed, float attackRange)
    {
        if (photonView.IsMine)
        {
            
            this.level = level;
            this.attackDamage = attackDamage;
            this.attackSpeed = attackSpeed;
            this.attackRange = attackRange;

            
            UpdateUI();
        }
    }
}
