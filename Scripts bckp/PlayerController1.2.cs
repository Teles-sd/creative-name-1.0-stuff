using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour {
    
    // PUBLIC VARIABLES
    
    
    [Range(1, 10)] public int movementSpeed = 5;
    [Range(5f, 15f)] public float jumpForce = 7f;
    
    public TextMeshProUGUI debugTextObject;
    
    
    // PRIVATE VARIABLES
    
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Animator playerAnimator;
    
    private Vector3 movementInput;
    
    private bool isMovable;
    
    private float deltaTime;
    
    private Vector3 targetVelocity;
    private float facingAngle;
    private Vector3 playerFacingVel;
    private Vector3 prbVelocity;
    
    private bool isJumpPressed;
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        
        isMovable = true;
        
        deltaTime = Time.fixedDeltaTime;
    }
    
//     public void setImovableForSeconds(float waitTime) {
//         setIsMovable(false);
//         StartCoroutine(ReenablePlayerMovement(waitTime));
//     }
//     
//     public void setIsMovable(bool val) {
//         isMovable = val;
//         if (!val){
//             
//             playerRigidbody.velocity = new Vector3(0f, 0f, 0f);
//             movementInput.x = 0;
//             movementInput.y = 0;
//         }
//     }
//     
//     IEnumerator ReenablePlayerMovement(float waitTime){
//         yield return new WaitForSeconds(waitTime);
//         setIsMovable(true);
//     }
    
    // Update is called once per frame
    private void Update() {
        
        if (isMovable) {
            
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.z = Input.GetAxisRaw("Vertical");
            
            facingAngle = playerTransform.rotation.eulerAngles.y;
            
            if (Input.GetButtonDown("Jump")){
                isJumpPressed = true;
            }
            
            // only updates animator if there is horizontal movement
            if (movementInput.x != 0) {
                playerAnimator.SetFloat("Horizontal", movementInput.x);
            }
            
            // only updates animator if there is any movement
            if (movementInput != Vector3.zero) {
                playerAnimator.SetBool("Walking", true);
            } else {
                playerAnimator.SetBool("Walking", false);
            }
        }
        // playerAnimator.SetFloat("Speed", playerRigidbody.velocity.sqrMagnitude);
        
        
        // debug stuff
        debugTextObject.text = string.Format(
            "facingAngle {0}\n" +
            "targetVelocity {1}\n" +
            "playerFacingVel {2}\n" +
            "prbVelocity {3}\n \n",
            new object[] {
                facingAngle,
                targetVelocity,
                playerFacingVel,
                prbVelocity,
            }
        );
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
        
        targetVelocity = movementInput.normalized * deltaTime * movementSpeed * 50f;
        
        playerFacingVel = Quaternion.AngleAxis(facingAngle, Vector3.up) * targetVelocity;
        
        prbVelocity = playerRigidbody.velocity;
        prbVelocity.x = playerFacingVel.x;
        prbVelocity.z = playerFacingVel.z;
        playerRigidbody.velocity = prbVelocity;
        
        if (isJumpPressed && prbVelocity.y == 0) {
            prbVelocity = playerRigidbody.velocity;
            prbVelocity.y = jumpForce;
            playerRigidbody.velocity = prbVelocity;
            isJumpPressed = false;
        }
    }
}
