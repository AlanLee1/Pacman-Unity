using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public GameObject[] Lives;

    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        var life = player.GetComponent<Life>();
        life.OnLifeRemoved += Life_OnLifeRemoved;

        UpdateLivesSprite(life.Lives);
    }

    private void Life_OnLifeRemoved(int remaningLives)
    {
        UpdateLivesSprite(remaningLives);
    }

    private void UpdateLivesSprite(int currentLives)
    {
        for (int i = 0; i < Lives.Length; i++)
        {
            Lives[i].SetActive(i < currentLives);
        }
    }


}
