using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.HID.HID;

public class PlayerController: MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 0.05f;
    private float jumpHeight = 0.01f;
    private float gravityValue = -9.81f;
    private PlayerInput playerInput;
    private Transform cameraTransform;
    private bool jumped = false;
    Rigidbody rigidbody;
    private float jumpAmount = 1f;
    private GameObject camera;
    [SerializeField]
    private GameObject wonPrefab;
    private GameObject stage;
    private GameObject player;
    public int currentStage;
    [SerializeField]
    private AudioClip victorySong;

    #region "Stage3"
    public List<int> buttonSequence; // List used to keep the order in which the player pressed the buttons
    private bool firstTimeTriggeredB1 = true;   // Booleans to only trigger the collider once
    private bool firstTimeTriggeredB2 = true;
    private bool firstTimeTriggeredB3 = true;
    private bool firstTimeTriggeredB4 = true;
    private Animator Button1Animator;   // The animator compenent of each buttons
    private Animator Button2Animator;
    private Animator Button3Animator;
    private Animator Button4Animator;
    [SerializeField]
    private GameObject arrivalPointPrefab;  // Prefab to spawn when getting the correct button order
    #endregion

    private void Start()
    {
        //controller = gameObject.AddComponent<CharacterController>();
        if (SceneManager.GetActiveScene().name== "stage1_Scene") {
            currentStage = 1;
            stage = GameObject.Find("Stage_1_Corrected");
        }
        if (SceneManager.GetActiveScene().name == "stage2_Scene")
        {
            currentStage = 2;
            stage = GameObject.Find("Stage_2_Corrected");
        }
        if (SceneManager.GetActiveScene().name == "stage3_Scene")
        {
            currentStage = 3;
            stage = GameObject.Find("Stage_3_Corrected");
        }
        camera = GameObject.Find("AR Camera");
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        
        player = GameObject.Find("Player");
        
    }

    void OnTriggerEnter(Collider collider) // le type de la variable est Collider
    {
        if(collider.gameObject.name=="ArrivalPoint")
        {

            AudioSource audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.clip = victorySong;
            audioSource.Play();

            Transform currentTransform = collider.gameObject.transform;
            Destroy(collider.gameObject);
            
            Instantiate(wonPrefab, currentTransform.position, Quaternion.identity);
            
            player.SetActive(false);
            Destroy(stage);
            Invoke("NextStage", 5);

        }

        // We spawn the complete player body with the victory song without destroying the stage
        if (collider.gameObject.name == "ArrivalPointPhysic(Clone)")
        {

            AudioSource audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.clip = victorySong;
            audioSource.Play();

            Transform currentTransform = collider.gameObject.transform;
            Destroy(collider.gameObject);

            Instantiate(wonPrefab, currentTransform.position, Quaternion.identity);

            player.SetActive(false);
            //Destroy(stage);
            //Invoke("NextStage", 5);


        }

        if (collider.gameObject.name=="DeadEnd") 
        {
            Respawn();
        }
        // Condition true if we collide with a button and only once
        if (collider.gameObject.name=="Button1" && firstTimeTriggeredB1)
        {
            buttonSequence.Add(1);  // We add to our list the number corresponding to the button pressed
            Button1Animator = collider.gameObject.GetComponent<Animator>(); // We start the button pressed animation
            Button1Animator.SetTrigger("Button1triggered");
            firstTimeTriggeredB1 = false;
        }
        if (collider.gameObject.name == "Button2" && firstTimeTriggeredB2)
        {
            buttonSequence.Add(2);
            Button2Animator = collider.gameObject.GetComponent<Animator>();
            Button2Animator.SetTrigger("Button2triggered");
            firstTimeTriggeredB2 = false;
        }
        if (collider.gameObject.name == "Button3" && firstTimeTriggeredB3)
        {
            buttonSequence.Add(3);
            Button3Animator = collider.gameObject.GetComponent<Animator>();
            Button3Animator.SetTrigger("Button3triggered");
            firstTimeTriggeredB3 = false;
        }
        if (collider.gameObject.name == "Button4" && firstTimeTriggeredB4)
        {
            buttonSequence.Add(4);
            Button4Animator = collider.gameObject.GetComponent<Animator>();
            Button4Animator.SetTrigger("Button4triggered");
            firstTimeTriggeredB4 = false;
        }
        else
        {
            groundedPlayer = true;
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        groundedPlayer = false;
    }

    void NextStage() 
    
    {
        
        if (currentStage==1) 
        {
            
            SceneManager.LoadScene("stage2_Scene");
        }
        if (currentStage == 2)
        {
            
            SceneManager.LoadScene("stage3_Scene");
        }
    }

    void Respawn() 
    {
        rigidbody.isKinematic = true;
        //Debug.Log(rigidbody.isKinematic);
        this.transform.position = new Vector3(0, 0, 0);
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        if (currentStage==1) 
        {
            this.transform.position = new Vector3(0.05984f, 0.05916f, -0.05236f);
        }
        if (currentStage == 2)
        {
            this.transform.position = new Vector3(0.06567f, 0.23833f, -0.052f);
        }
        if (currentStage == 3)
        {
            this.transform.position = new Vector3(0, 0.0541f, 0);
        }

        rigidbody.isKinematic = false;
    }
        void Update()
    {
        if(this.transform.position.y<0)
        {

            Respawn();

        }
        //Debug.Log(groundedPlayer);

        /*groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }*/
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        //Debug.Log(input.x + ", " + input.y);
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * camera.transform.right + move.z * camera.transform.forward;
        this.transform.Translate(move * Time.deltaTime * playerSpeed);
        
        /*controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }*/

        // Changes the height position of the player..
        float jumpInput = playerInput.actions["Jump"].ReadValue<float>();
        //Debug.Log(jumpInput);
        
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

        // Condition true if the player pressed the four buttons
        if (buttonSequence.Count == 4)
        {
            // If the correct sequence was entered, 4-2-3-1, the condition is true 
            if(buttonSequence[0] == 4 && buttonSequence[1] == 2 && buttonSequence[2] == 3 && buttonSequence[3] == 1)
            {
                buttonSequence.Clear(); // We clear the list
                GameObject bodySpawn = GameObject.Find("BodySpawnPosition");    // We get the position where to spawn the Body and instantiate the body
                Vector3 position = bodySpawn.transform.position;
                position.y = 15;
                Instantiate(arrivalPointPrefab, bodySpawn.transform.position, Quaternion.identity);
            }
            else
            {
                // If the player got the sequence wrong we reset the bool value to only trigger the button once
                // and play the animation to get the button at there starting position
                buttonSequence.Clear();
                firstTimeTriggeredB1 = true;
                firstTimeTriggeredB2 = true;
                firstTimeTriggeredB3 = true;
                firstTimeTriggeredB4 = true;
                Button1Animator.SetTrigger("FalseSequence");
                Button2Animator.SetTrigger("FalseSequence");
                Button3Animator.SetTrigger("FalseSequence");
                Button4Animator.SetTrigger("FalseSequence");
            }
        }

    }
}