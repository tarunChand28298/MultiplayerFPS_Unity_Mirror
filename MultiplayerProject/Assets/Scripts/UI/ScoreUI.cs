using UnityEngine.UI;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public Text AScore;
    public Text BScore;

    public void UpdateScoreForA(int value)
    {
        AScore.text = value.ToString();
    }

    public void UpdateScoreForB(int value)
    {
        BScore.text = value.ToString();
    }
}
