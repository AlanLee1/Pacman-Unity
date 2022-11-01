using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;
    public int CurrentScore { get => _currentScore; }
    public int HighScore { get => _highScore; }
    private int _currentScore;
    private int _highScore;
    private int _count;
    public List<GhostAI> ListGhostAI;

    private void Awake()
    {
        _highScore = PlayerPrefs.GetInt("high-score", 0);
    }

    private void Start()
    {
        var allCollectibles = FindObjectsOfType<Collectible>();
        var allGhost = FindObjectsOfType<GhostCollectible>();

        foreach (var ghost in allGhost)
        {
            ghost.OnCollectedGhost += Ghost_OnCollectedGhost;
        }

        foreach (var collectible in allCollectibles)
        {
            collectible.OnCollected += Collectible_OnCollected;
        }
    }

    private void Update()
    {
        CheckState();
    }

    private void Ghost_OnCollectedGhost(int score, GhostCollectible ghostCollectible)
    {
        if (ghostCollectible.GetComponent<GhostAI>()._ghostState == (GhostState.Vulnerable | GhostState.VulnerabilityEnding))
        {
            _count++;
            _currentScore += score * _count;
            OnScoreChanged?.Invoke(_currentScore);

            if (_currentScore >= _highScore)
            {
                _highScore = _currentScore;
                OnHighScoreChanged?.Invoke(_highScore);
            }
        }
    }

    private void CheckState()
    {
        int x = 0;
        for (int i = 0; i < ListGhostAI.Count; i++)
        {
            if (ListGhostAI[i]._ghostState == GhostState.Active)
            {
                x++;
            }
        }

        if (x == 4)
        {
            _count = 0;
        }
    }

    private void Collectible_OnCollected(int score, Collectible collectible)
    {
        _currentScore += score;
        OnScoreChanged?.Invoke(_currentScore);

        if (_currentScore >= _highScore)
        {
            _highScore = _currentScore;
            OnHighScoreChanged?.Invoke(_highScore);
        }

        collectible.OnCollected -= Collectible_OnCollected;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("high-score", _highScore);
    }
}
