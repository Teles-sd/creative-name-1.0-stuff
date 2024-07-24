using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    [Header("Movement")]

    [Range(1, 10)] public int movementSpeed = 5;
    [Range(3f, 15f)] public float cameraRotationSpeed = 9f;
    
    [Space(10)]
    [Range(0f, 1f)] public float footBoxWidth = 0.7f;
    [ShowOnly] [Range(3, 13)] public int jumpForce = 7;
    [Range(0f, 1f)] public float maxGlideTime = 0.75f;
    [Range(1f, 5f)] public float gravityScale = 2f;
    
    
    [Header("Tests & Debug stuff")]
    
    public bool doTestStuff = false;
    public bool activateGizmos = false;

    [Space(10)]
    public bool writeToDebugText = false;
    public TextMeshProUGUI debugTextObject;
    
    
    
    // PRIVATE VARIABLES
    
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Animator playerAnimator;
    private CapsuleCollider playerBodyCollider;
    
    private Vector3 movementInput = Vector3.zero;
    private Vector2 mouseInput = Vector2.zero;
    
    private bool isMovable;
    private bool cursorLocked;
    
    private Vector3 inputVelocity;
    private float facingAngle;
    private Vector3 horizontalVelocity;
    private Vector3 prbVelocity;
    
    private Collider[] footColliders;
    private Vector3 footBoxCenter = new Vector3(0,-2.2f,0);         // relative to player
    private Vector3 footBoxSize = new Vector3(0.7f,0.24f,0.7f);     // relative to player
    private LayerMask footLayerMask = -1; // everything
    
    private bool isFooted;
    private bool jumpSeqnc;
    private bool jumpStart;
    private bool jumpHoldn;
    private bool jumpGlide;
    private float glideTime = 0f;
    
    
    // GETTERS
    
    public Vector2 getMouseInput(){ return mouseInput; }
    
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider>();
        
        isMovable = true;
        cursorLocked = false;
        
        isFooted = false;
        jumpSeqnc = false;
        jumpStart = false;
        jumpHoldn = false;
        jumpGlide = false;
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
            
            // MOVEMENT
            
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.z = Input.GetAxisRaw("Vertical");
            
            facingAngle = playerTransform.rotation.eulerAngles.y;
            
            
            
            // JUMP
            
            footColliders = Physics.OverlapBox(center:playerTransform.position + footBoxCenter,
                                               halfExtents:footBoxSize*0.5f,
                                               orientation:Quaternion.identity,
                                               layerMask:footLayerMask,
                                               queryTriggerInteraction:QueryTriggerInteraction.Ignore);

            isFooted = false;
            
            foreach (var fc in footColliders){
                if ( fc != playerBodyCollider ){
                    isFooted = true;
                }
            }
            
            // conditions to initiate jump sequence
            if (Input.GetButtonDown("Jump") && isFooted && !jumpSeqnc){
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
            
            
            
            // CAMERA ROTATION
            
            // lock and unlock cursor when the "L" key is pressed
            if (Input.GetKeyDown( KeyCode.L )){
                
                cursorLocked = !cursorLocked;
                
                if (cursorLocked){
                    Cursor.lockState = CursorLockMode.Locked;
                } else {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            
            if (cursorLocked){
                mouseInput.x = Mathf.Clamp( Input.GetAxis("Mouse X"), -3, 3 );
                mouseInput.y = Mathf.Clamp( Input.GetAxis("Mouse Y"), -3, 3 );
                // mouseInput.y = limitMouseInput( Input.GetAxis("Mouse Y"), 1 );
            } else {
                mouseInput.x = 0f;
                mouseInput.y = 0f;
            }
            
            
            
            // ANIMATION
            
            // only updates animator direction if there is horizontal movement
            if (movementInput.x != 0) {
                playerAnimator.SetFloat("Horizontal", movementInput.x);
            }
            
            // updates animator state if there is any movement
            if (movementInput != Vector3.zero) {
                playerAnimator.SetBool("Walking", true);
            } else {
                playerAnimator.SetBool("Walking", false);
            }
            
            // updates animator jumping states
            playerAnimator.SetBool("jumpSeqnc", jumpSeqnc);
            playerAnimator.SetBool("jumpGlide", jumpGlide);
            playerAnimator.SetBool("isFooted", isFooted);
        }
        
        
        
        // DEBUG STUFF
        
        if (writeToDebugText) {
            debugTextObject.text = string.Format(
                "prbVelocity {0}\n" +
                "cursorLocked {1}\n" +
                "Input.GetAxis(\"Mouse X\") {2}\n" +
                "mouseInput.x {3}\n" +
                "",
                new object[] {
                    prbVelocity,
                    cursorLocked,
                    Input.GetAxis("Mouse X"),
                    mouseInput.x,
                }
            );
        }
        
        // if (writeToDebugText) {
        //     debugTextObject.text = string.Format(
        //         "jumpSeqnc {0}\n" +
        //         "jumpStart {1}\n" +
        //         "jumpHoldn {2}\n" +
        //         "jumpGlide {3}\n" +
        //         "prbVelocity {4}\n" +
        //         "isFooted {5}\n" +
        //         "",
        //         new object[] {
        //             jumpSeqnc,
        //             jumpStart,
        //             jumpHoldn,
        //             jumpGlide,
        //             prbVelocity,
        //             isFooted,
        //         }
        //     );
        // }
        
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
        if (activateGizmos) {
            if (isFooted){
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            
            if (playerTransform){
                Gizmos.DrawWireCube(center:playerTransform.position + footBoxCenter, size:footBoxSize);
            }
        }
    }
    
    
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
        
        // MOVEMENT
        
        prbVelocity = playerRigidbody.velocity;
        
        inputVelocity = movementInput.normalized * movementSpeed;
        
        horizontalVelocity = Quaternion.AngleAxis(facingAngle, Vector3.up) * inputVelocity;
        
        prbVelocity.x = horizontalVelocity.x;
        prbVelocity.z = horizontalVelocity.z;
        
        
        
        // JUMP
        
        if (jumpStart){
            prbVelocity.y = jumpForce;
            jumpStart = false;
        }
        
        if (jumpSeqnc){
            
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
        
        
        
        // CAMERA ROTATION
        
        playerTransform.Rotate(
            axis: Vector3.up,
            angle: mouseInput.x * cameraRotationSpeed
        );
        
    }
}
