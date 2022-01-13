using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Speed variable along with inputs for WASD for movement
    public float Speed;
    public float StrafeInput, ForwardInput;
    public int Score = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player Move Initialized");
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        StrafeInput = Input.GetAxis("Horizontal");
        ForwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * Speed * ForwardInput);
        transform.Translate(Vector3.right * Time.deltaTime * Speed * StrafeInput);
    }
}
