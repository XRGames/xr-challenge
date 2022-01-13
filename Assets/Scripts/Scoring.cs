using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scoring : MonoBehaviour
{
    public GameObject ScoreText;
    public static int Score;

    // Handles score update on screen
    void Update()
    {
        ScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + Score;
    }
}
