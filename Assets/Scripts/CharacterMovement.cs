using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController charControl;
    private CapsuleCollider coll;
    private Vector3 playerVel;
    private bool grounded;
    private float playerSpeed = 3.0f;
    private float jumpHeight = 1.5f;
    private float gravity = 9.81f;

    void Awake()
    {
        charControl = gameObject.GetComponent<CharacterController>();
        coll = gameObject.GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        grounded = IsGrounded();

        if(playerVel.y < 0 && grounded)
        {
            playerVel.y = -gravity * Time.deltaTime;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }


        float xDir = Input.GetAxis("Horizontal");
        float yDir = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(xDir, 0.0f, yDir);

        charControl.Move(moveDir * Time.deltaTime * playerSpeed);

        if(moveDir != Vector3.zero)
        {
            gameObject.transform.forward = moveDir;
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Debug.Log("Jump button pressed");
            playerVel.y += Mathf.Sqrt(jumpHeight * -3.0f * -gravity);
        }

        playerVel.y -= 0.01f;
        charControl.Move(playerVel * Time.deltaTime);
    }

    /// <summary>
    /// Checks for if player collider is on the ground
    /// </summary>
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, coll.bounds.extents.y + 0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            GameController.GameCon.UpdateScore(other.gameObject.GetComponent<Pickup>().GetPickedUp());
        }
    }
}
