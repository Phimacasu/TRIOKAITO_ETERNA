using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text levelText;  
    public TMP_Text talentsText;

    public void UpdateLevelUI(int level, float attackDamage, float attackSpeed, float attackRange)
    {
        Debug.Log($"Updating UI: Level={level}, Damage={attackDamage}, Speed={attackSpeed}, Range={attackRange}");

        levelText.text = "Level: " + level;
        talentsText.text = $"Damage: {attackDamage}\nSpeed: {attackSpeed}\nRange: {attackRange}";

        
    }


    public void DisplayTalent(TalentType talent)
    {
        talentsText.text += $"\nNew Talent: {talent}";
    }
}
