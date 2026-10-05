using System;
using UnityEngine;

public enum player
{
    None,
    P1,
    P2
}

public class GameOM
{
    public static event Action OnGameStart;

    public static void StartGame()
    {
        OnGameStart?.Invoke();
    }
    
    public static event Action<player> OnPlayerScored;

    public static void PlayerScored(player p)
    {
        OnPlayerScored?.Invoke(p);
    }
    
    public static event Action<int> OnGameOver;
    public static void GameOver(int resetScore)
    {
        OnGameOver?.Invoke(resetScore);
    }

    public static event Action<player, int> OnScoreChanged;
    public static void ScoreChanged(player p,int newScore)
    {
        OnScoreChanged?.Invoke(p, newScore);
    }

}
