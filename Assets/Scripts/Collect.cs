using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public AudioSource CollectSound;
    // Sets Score to 0
    void Start()
    {
        Scoring.Score = 0;
    }

    // When something has collided with the star, adds 100 to score
    void OnTriggerEnter(Collider other)
    {
        CollectSound.Play();
        Scoring.Score += 100;
        GetComponent<Pickup>().GetPickedUp();
        Destroy(GetComponent<CapsuleCollider>());
        //Delays the destruction of pickup by 3 seconds
        Invoke("DestroyPickup", 1);
    }

    // Destorys pickup
    void DestroyPickup()
    {
        Destroy(gameObject);
        Debug.Log(gameObject.name +" has been destroyed");
    }
}
