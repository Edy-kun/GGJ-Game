using TMPro;
using UnityEngine;



public class HUD : MonoBehaviour
{

    public TMP_Text ScoreLabel;

    public void HandleScoreChanged(int score)
    {
        ScoreLabel.text = $"points: {score}";
    }

}