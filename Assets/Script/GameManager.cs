using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int combo = 0;
    public TMP_Text scoreCountText;
    public TMP_Text comboCountText;

    public void AddScore(int amount)
    {
        score += amount;
        combo++;
        UpdateUI();
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreCountText.text = score.ToString();
        comboCountText.text = combo.ToString();
    }
}
