using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPoints : MonoBehaviour
{
    [SerializeField] private Text P1PointsText;
    [SerializeField] private Text P2PointsText;
    void Start()
    {
        
    }

    void OnEnable()
    {
        GameOM.OnScoreChanged += ChangeText;
        GameOM.OnGameOver += ResetCounter;
    }
    
    void OnDisable()
    {
        GameOM.OnScoreChanged -= ChangeText;
        GameOM.OnGameOver -= ResetCounter;
    }

    // Update is called once per frame
    void ChangeText(player p, int newScore)
    {
        if (p == player.P2)
        {
            int P1Points = newScore;
            P1PointsText.text = P1Points.ToString();
        }
        else if (p == player.P1)
        {
            int P2Points = newScore;
            P2PointsText.text = P2Points.ToString();
        }
    }

    void ResetCounter(int resetScore)
    {
        P1PointsText.text = "0";
        P2PointsText.text = "0";
    }
}
