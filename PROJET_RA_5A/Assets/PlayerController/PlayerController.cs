using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.HID.HID;

public class PlayerController: MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 0.1f;
    private float jumpHeight = 0.01f;
    private float gravityValue = -9.81f;
    private PlayerInput playerInput;
    private Transform cameraTransform;
    private bool jumped = false;
    Rigidbody rigidbody;
    private float jumpAmount = 1f;



    private void Start()
    {
        //controller = gameObject.AddComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        
    }

    void OnTriggerEnter(Collider collider) // le type de la variable est Collider
    {
        groundedPlayer = true;
    }

    void OnTriggerExit(Collider collider)
    {
        groundedPlayer = false;
    }

        void Update()
    {
        Debug.Log(groundedPlayer);

        /*groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }*/
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Debug.Log(input.x + ", " + input.y);
        Vector3 move = new Vector3(input.x, 0, input.y);
        this.transform.Translate(move * Time.deltaTime * playerSpeed);
        
        /*controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }*/

        // Changes the height position of the player..
        float jumpInput = playerInput.actions["Jump"].ReadValue<float>();
        Debug.Log(jumpInput);
        
        if (jumpInput==1 && jumped==false) 
        {
            jumped = true;
            
            rigidbody.AddForce(Vector2.up * jumpAmount, ForceMode.Impulse);
            //this.transform.position.Set(this.transform.position.x, this.transform.position.y + jumpHeight * -3.0f * gravityValue, this.transform.position.z);
            //this.transform.Translate(new Vector3(0f, jumpHeight, 0f));
            
        }

        if (groundedPlayer) 
        {
            jumped = false;
        }
        /*if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }*/
        
        //this.transform.position.y += gravityValue * Time.deltaTime;
        //controller.Move(playerVelocity * Time.deltaTime);
    }
}