using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour {
    
    // PUBLIC VARIABLES
    
    
    [Range(1, 10)] public int movementSpeed = 5;
    [Range(5f, 15f)] public float jumpForce = 7f;
    [Range(0.1f, 1f)] public float maxJumpTime = 0.5f;
    
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
    
    private bool jumpSeqnc;
    private bool isJumping;
    private bool hoverJump;
    
    private Vector3 prbJumpVelocity;
    private Vector3 prbLerpJumpVelocity;
    private float jumpTimer = 0f;
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        
        isMovable = true;
        
        jumpSeqnc = false;
        isJumping = false;
        hoverJump = false;
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
            
            // conditions to initiate jump sequence
            if (Input.GetButtonDown("Jump") && Mathf.Abs(prbVelocity.y) < 0.05 && !jumpSeqnc){
                jumpSeqnc = true;
                isJumping = true;
                jumpTimer = 0f;
            }
            
            if (jumpSeqnc){
                
                // conditions to continue the jump
                if (isJumping){
                    
                    if ( !Input.GetButton("Jump") || jumpTimer > maxJumpTime){
                        isJumping = false;
                        if (Input.GetButton("Jump")) {
                            hoverJump = true;
                            jumpTimer = 0f;
                        }
                    }
                }
                
                // still holding after jump timer run out
                if (hoverJump){
                    
                    if ( !Input.GetButton("Jump") || jumpTimer > maxJumpTime){
                        hoverJump = false;
                    }
                }
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
            
            playerAnimator.SetBool("jumpSeqnc", jumpSeqnc);
            playerAnimator.SetBool("isJumping", isJumping);
            playerAnimator.SetFloat("Yvelocity", prbVelocity.y);
        }
        
        
        
        // debug stuff
        debugTextObject.text = string.Format(
            "facingAngle {0}\n" +
            "targetVelocity {1}\n" +
            "playerFacingVel {2}\n" +
            "prbVelocity {3}\n" +
            "deltaTime {4}\n" + 
            "",
            new object[] {
                facingAngle,
                targetVelocity,
                playerFacingVel,
                prbVelocity,
                deltaTime,
            }
        );
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
        
        deltaTime = Time.fixedDeltaTime;
        
        targetVelocity = movementInput.normalized * movementSpeed;
        
        playerFacingVel = Quaternion.AngleAxis(facingAngle, Vector3.up) * targetVelocity;
        
        prbVelocity = playerRigidbody.velocity;
        prbVelocity.x = playerFacingVel.x;
        prbVelocity.z = playerFacingVel.z;
        
        if (jumpSeqnc){
            if (isJumping) {
                prbVelocity.y = jumpForce;
            } else if (hoverJump) {
                prbVelocity.y = -jumpForce * 0.2f;
            } else {
                prbVelocity.y = -jumpForce * 0.2f;
                jumpSeqnc = false;
            }
            jumpTimer += deltaTime;
        }
        
        playerRigidbody.velocity = prbVelocity;
        
        // if (isJumping && prbVelocity.y == 0) {
        //     prbVelocity = playerRigidbody.velocity;
        //     prbVelocity.y = jumpForce;
        //     playerRigidbody.velocity = prbVelocity;
        //     isJumping = false;
        // }
    }
}
