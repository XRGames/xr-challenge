using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    public bool WinToggle = false;
    public GameObject FinishSpace;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Scoring.Score == 500 && WinToggle == false)
        {
            WinCon();
        }
    }

    public void WinCon()
    {
        Debug.Log("All Stars Collected. You Win");
        Vector3 spawnPos = new Vector3(3.15f, 0.5f, 0f);
        Instantiate(FinishSpace, spawnPos, FinishSpace.transform.rotation);
        WinToggle = true;
    }

}
