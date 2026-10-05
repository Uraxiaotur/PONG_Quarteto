using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int P1Points;
    public int P2Points;
    public player pToStart;

    private bool gameStarted;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameOM.OnPlayerScored += IncreasePoints;
    }

    void OnDisable()
    {
        GameOM.OnPlayerScored -= IncreasePoints;
    }
    
    public void StartGame()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        {
            GameOM.StartGame();
        }
    }

    public void IncreasePoints(player p)
    {
        if (P1Points == 11)
        {
            P1Points = 0;
            P2Points = 0;
            GameOM.GameOver(0);
        }
        else if (P2Points == 11)
        {
            P1Points = 0;
            P2Points = 0;
            GameOM.GameOver(0);
        }

        if (p == player.P1)
        {
            pToStart = player.P2;
            P2Points += 1;
            GameOM.ScoreChanged(player.P2, P2Points);
        }
        else if (p == player.P2)
        {
            pToStart = player.P1;
            P1Points += 1;
            GameOM.ScoreChanged(player.P1, P1Points);       
        }
    }

    public player GetScoredPlayer()
    {
        return pToStart;
    }
}
