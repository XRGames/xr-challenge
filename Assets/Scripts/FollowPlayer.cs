using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 startOffset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + startOffset;
    }
}
