using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private int level;

    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    public bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        isGameActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLevel()
    {
        level += 1;

        levelText.text = "Level: " + level;
    }

    public void GameOver()
    {        
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

}
