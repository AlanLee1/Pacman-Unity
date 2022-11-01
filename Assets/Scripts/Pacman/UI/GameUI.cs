using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject ReadyMessage;
    public GameObject GameOverMessage;
    public AudioSource AudioSource;
    public AudioClip BeginningMusic;
    public GameManager _gameManager;
    public BlinkTilemapColor BlinkTilemap;

    private void Start()
    {
        //pega o script vinculado ao objeto
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnGameStarted += GameManager_OnGameStarted;
        _gameManager.OnVictory += GameManager_OnVictory;
        _gameManager.OnGameOver += GameManager_OnGameOver;
        AudioSource.PlayOneShot(BeginningMusic);

    }

    private void GameManager_OnGameStarted()
    {
        ReadyMessage.SetActive(false);
    }

    private void GameManager_OnGameOver()
    {
        GameOverMessage.SetActive(true);
        BlinkTilemap.enabled = true;
    }

    private void GameManager_OnVictory()
    {
        BlinkTilemap.enabled = true;
    }

}
