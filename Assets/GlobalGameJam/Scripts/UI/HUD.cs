using TMPro;
using UnityEngine;
using DG.Tweening;


public class HUD : MonoBehaviour
{

    public TMP_Text ScoreLabel;

    public void HandleScoreChanged(int score)
    {
        ScoreLabel.text = $"points: {score}";
        ScoreLabel.transform.DOKill(true);
        ScoreLabel.transform.DOPunchScale(Vector3.one * 1.1f, .1f);
    }
    
}