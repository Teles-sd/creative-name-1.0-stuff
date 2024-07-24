using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour {
    
    // PUBLIC VARIABLES
    
    
    [Range(1, 10)] public int movementSpeed = 5;
    // [Range(5f, 15f)] public float jumpForce = 7f;
    
    [Range(3, 13)] public int jumpForce = 7;
    [Range(0f, 1f)] public float maxGlideTime = 0.75f;
    [Range(1f, 5f)] public float gravityScale = 2f;
    // [Range(2f, 5f) public float lowJumpMultiplier = 2.5f;
    public LayerMask footLayerMask;
    
    public TextMeshProUGUI debugTextObject;
    
    
    // PRIVATE VARIABLES
    
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Animator playerAnimator;
    
    private CapsuleCollider playerBody;
    private BoxCollider playerFeet;
    
    private Vector3 movementInput;
    
    private bool isMovable;
    
    private Vector3 inputVelocity;
    private float facingAngle;
    private Vector3 horizontalVelocity;
    private Vector3 prbVelocity;
    
    private Collider[] footColliders;
    // private RaycastHit[] footColliders;
    private bool isFooted;
    // private float jumpThreshold = 0.05f;
    private bool jumpSeqnc;
    private bool jumpStart;
    private bool jumpHoldn;
    private bool jumpGlide;
    private float glideTime = 0f;
    
    // private Vector3 prbJumpVelocity;
    // private Vector3 prbLerpJumpVelocity;
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        
        playerBody = GetComponent<CapsuleCollider>();
        playerFeet = GetComponent<BoxCollider>();
        
        isMovable = true;
        
        isFooted = false;
        jumpSeqnc = false;
        jumpStart = false;
        jumpHoldn = false;
        jumpGlide = false;
        
        Physics.IgnoreCollision(playerBody, playerFeet);
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
            
            // try boxcast (not all) from bottom up, then check if collider is not null and is not the player
            
            // footColliders = Physics.BoxCastAll(center=playerBody.center,
            //                                    halfExtents=new Vector3(playerBody.radius*0.5,
            //                                                            playerBody.height*0.55,
            //                                                            playerBody.radius*0.5),
            //                                    direction=Vector3.down,
            //                                    maxDistance=playerBody.height*0.55,
            //                                    queryTriggerInteraction=false);
            
            // footColliders = Physics.OverlapBox(center:playerBody.center,
            //                                    halfExtents:new Vector3(playerBody.radius*0.5f,
            //                                                            playerBody.height*0.55f,
            //                                                            playerBody.radius*0.5f),
            //                                    orientation:Quaternion.identity,
            //                                    layerMask:footLayerMask,
            //                                    queryTriggerInteraction:false);
            
            // footColliders = Physics.OverlapBox(playerTransform.position + Vector3.up * -2.2f,
            // footColliders = Physics.OverlapBox(playerTransform.position + playerFeet.center,
            //                                    playerFeet.size*0.5f);
            footColliders = Physics.OverlapBox(center:playerTransform.position + new Vector3(0,-2.2f,0),
                                               halfExtents:new Vector3(0.35f,0.12f,0.35f),
                                               orientation:Quaternion.identity,
                                               layerMask:footLayerMask,
                                               queryTriggerInteraction:QueryTriggerInteraction.Ignore);
            
            // isFooted = footColliders.Any(fc => fc.collider != playerBody)
            isFooted = false;
            
            foreach (var fc in footColliders){
                if ( fc != playerBody ){
                    isFooted = true;
                }
            }
            
            
            
            
            // conditions to initiate jump sequence
            if (Input.GetButtonDown("Jump") && isFooted && !jumpSeqnc){
            // if (Input.GetButtonDown("Jump") && Mathf.Abs(prbVelocity.y) < jumpThreshold && !jumpSeqnc){
                jumpSeqnc = true;
                jumpStart = true;
                jumpHoldn = true;
                jumpGlide = false;
                glideTime = 0f;
            }
            
            // conditions to continue the jump
            if (jumpSeqnc && !jumpStart){
                
                if (jumpHoldn){
                    
                    if ( !Input.GetButton("Jump") ){
                        jumpHoldn = false;
                        jumpGlide = false;
                    }else if ( prbVelocity.y < 0 ) {
                        jumpHoldn = false;
                        jumpGlide = true;
                    }
                }
                
                // still holding after jump hit apex
                if (jumpGlide){
                    
                    if ( !Input.GetButton("Jump") || glideTime > maxGlideTime){
                        jumpGlide = false;
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
            playerAnimator.SetBool("jumpGlide", jumpGlide);
            playerAnimator.SetBool("isFooted", isFooted);
        }
        
        
        
        // debug stuff
        debugTextObject.text = string.Format(
            "jumpSeqnc {0}\n" +
            "jumpStart {1}\n" +
            "jumpHoldn {2}\n" +
            "jumpGlide {3}\n" +
            "prbVelocity {4}\n" +
            "isFooted {5}\n" +
            "",
            new object[] {
                jumpSeqnc,
                jumpStart,
                jumpHoldn,
                jumpGlide,
                prbVelocity,
                isFooted,
            }
        );
        // debugTextObject.text = string.Format(
        //     "inputVelocity {1}\n" +
        //     "facingAngle {0}\n" +
        //     "horizontalVelocity {2}\n" +
        //     "prbVelocity {3}\n" +
        //     "",
        //     new object[] {
        //         inputVelocity,
        //         facingAngle,
        //         horizontalVelocity,
        //         prbVelocity,
        //     }
        // );
    }
    
    void OnDrawGizmos() {
        
        if (isFooted){
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.red;
        }
        
        // Gizmos.DrawWireCube(center:playerTransform.position + playerBody.center,
        //                     size:new Vector3(playerBody.radius,
        //                                      playerBody.height*0.55f*2,
        //                                      playerBody.radius));
        if (playerTransform){
            Gizmos.DrawWireCube(center:playerTransform.position + new Vector3(0,-2.2f,0),
                                size:new Vector3(0.35f,0.12f,0.35f) * 2);
        }

        // //Check if there has been a hit yet
        // if (isFooted)
        // {
        //     Gizmos.DrawRay(transform.position, transform.forward * m_Hit.distance);
        //     //Draw a cube that extends to where the hit exists
        //     Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit.distance, transform.localScale);
        // }
        // //If there hasn't been a hit yet, draw the ray at the maximum distance
        // else
        // {
        //     //Draw a Ray forward from GameObject toward the maximum distance
        //     Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);
        //     //Draw a cube at the maximum distance
        //     Gizmos.DrawWireCube(transform.position + transform.forward * m_MaxDistance, transform.localScale);
        // }
    }
    
    
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
        
        prbVelocity = playerRigidbody.velocity;
        
        inputVelocity = movementInput.normalized * movementSpeed;
        
        horizontalVelocity = Quaternion.AngleAxis(facingAngle, Vector3.up) * inputVelocity;
        
        prbVelocity.x = horizontalVelocity.x;
        prbVelocity.z = horizontalVelocity.z;
        
        if (jumpStart){
            prbVelocity.y = jumpForce;
            jumpStart = false;
        }
        
        if (jumpSeqnc){
            // if (jumpHoldn) {
            //     prbVelocity.y = jumpForce;
            // } else 
            if (jumpGlide) {

                prbVelocity.y = 0.4f * Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
                
                glideTime += Time.fixedDeltaTime;
                
            } else if ( !jumpHoldn ) {
                jumpSeqnc = false;
            }
            
        }
        
        // extra gravity for snappy jump
        if ( !jumpSeqnc ){
            // prbVelocity.y += Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime;
            playerRigidbody.AddForce(Physics.gravity * (gravityScale - 1), ForceMode.Acceleration);
        }

        playerRigidbody.velocity = prbVelocity;
        

        
        
        
        
        


//         // update
// 
//         if (Input.GetButtonDown("Jump")){
//             // playerRigidbody.velocity = Vector2.up * jumpForce;
//             playerRigidbody.velocity = Vector3.up * jumpForce;
//         }
// 
//         if (playerRigidbody.velocity.y < 0){
//             playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime;
//         } else if (playerRigidbody.velocity.y > 0 && !Input.GetButton("Jump")) {
//             playerRigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
//         }
//         
//         
//         
//         
//         
//         
//         // I used this in the opposite, to reduce the effect of gravity. This lets the player glide a bit after the jump.
// 
// 
//         rb.velocity += new Vector3(0, rb.velocity.y * Physics.gravity.y * 0.5f * Time.deltaTime, 0);
        



        
        // if (jumpHoldn && prbVelocity.y == 0) {
        //     prbVelocity = playerRigidbody.velocity;
        //     prbVelocity.y = jumpForce;
        //     playerRigidbody.velocity = prbVelocity;
        //     jumpHoldn = false;
        // }
    }
}
