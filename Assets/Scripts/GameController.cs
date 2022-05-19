using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Camera mainCam;
    private GameObject player;
    private TMPro.TextMeshProUGUI scoreText;
    private int maxScore = 10000;
    private int score;

    public static GameController GameCon;

    private void Awake()
    {
        if (GameCon != null)
            GameObject.Destroy(GameCon);
        else
            GameCon = this;

        DontDestroyOnLoad(this);

        scoreText = GameObject.Find("ScoreText").GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Start()
    {
        score = 0;
    }

    void Update()
    {

    }

    public void UpdateScore(int scoreChange)
    {
        if(scoreChange != -1)
        {
            score += scoreChange;
            scoreText.text = "SCORE: " + score;
        }
    }
}
