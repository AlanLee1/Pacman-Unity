using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Starting,
        Playing,
        LifeLost,
        GameOver,
        Victory
    }

    public float StartupTime;
    public float LifeLostTimer;
    public event Action OnGameStarted;
    public event Action OnVictory;
    public event Action OnGameOver;

    private GhostAI[] _allGhosts;
    private CharacterMotor _pacmanMotor;
    private GhostHouse _ghostHouse;
    private GameState _gameState;
    private int _victoryCount;
    private float _lifeLostTimer;
    private bool _isGameOver;

    private void Start()
    {
        var allCollectibles = FindObjectsOfType<Collectible>();

        _victoryCount = 0;

        foreach (var collectible in allCollectibles)
        {
            _victoryCount++;
            collectible.OnCollected += Collectible_OnCollected;
        }


        var pacman = GameObject.FindWithTag("Player");
        _pacmanMotor = pacman.GetComponent<CharacterMotor>();
        _allGhosts = FindObjectsOfType<GhostAI>();
        StopAllCharacters();

        pacman.GetComponent<Life>().OnLifeRemoved += Pacman_OnLifeRemoved;

        _gameState = GameState.Starting;
        _ghostHouse = FindObjectOfType<GhostHouse>();
        _ghostHouse.enabled = false;
    }

    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Starting:
                StartupTime -= Time.deltaTime;

                if (StartupTime <= 0)
                {
                    _gameState = GameState.Playing;
                    _ghostHouse.enabled = true;
                    StartAllCharacters();
                    OnGameStarted?.Invoke();
                }
                break;

            case GameState.LifeLost:
                _lifeLostTimer -= Time.deltaTime;
                if (_lifeLostTimer <= 0)
                {
                    if (_isGameOver)
                    {
                        _gameState = GameState.GameOver;
                        OnGameOver?.Invoke();
                    } else
                    {
                        _gameState = GameState.Playing;
                        ResetAllCharacters();
                    }
                }
                break;

            case GameState.GameOver:
            case GameState.Victory:
                if (Input.anyKey)
                {
                    SceneManager.LoadScene(0);
                }
                break;

            default:
                break;
        }
    }

    private void Pacman_OnLifeRemoved(int remainingLives)
    {
        StopAllCharacters();

        _lifeLostTimer = LifeLostTimer;
        _gameState = GameState.LifeLost;
        _isGameOver = remainingLives <= 0;
    }

    //_ -> variavel de descarte
    private void Collectible_OnCollected(int _, Collectible collectible)
    {
        _victoryCount--;

        if (_victoryCount <= 0)
        {
            _gameState = GameState.Victory;
            StopAllCharacters();
            OnVictory?.Invoke();
        }
        collectible.OnCollected -= Collectible_OnCollected;
    }

    private void StartAllCharacters()
    {
        _pacmanMotor.enabled = true;

        foreach (var ghost in _allGhosts)
        {
            ghost.StartMoving();
        }
    }

    private void StopAllCharacters()
    {
        _pacmanMotor.enabled = false;

        foreach (var ghost in _allGhosts)
        {
            ghost.StopMoving();
        }
    }

    private void ResetAllCharacters()
    {
        _pacmanMotor.ResetPosition();

        foreach (var ghost in _allGhosts)
        {
            ghost.Reset();
        }
        StartAllCharacters();
    }
}
