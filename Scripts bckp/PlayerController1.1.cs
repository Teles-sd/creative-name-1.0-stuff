using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour
{
    
    // PUBLIC VARIABLES
    
    // "public" gives you fields in the editor to fill
    // The "[Range(10f, 50f)] [SerializeField]" makes a slider field from 10 to 50
    // "[HideInInspector]" doesn't give hides field for public variables
    
    
    
    [Range(1f, 10f)] public int movementSpeed = 5;
    [Range(0, .3f)] public float movementSmoothing = .05f;
    
    // [HideInInspector] public Animator playerAnimator;
    
    public TextMeshProUGUI debugTextObject;
    
    
    // PRIVATE VARIABLES
    
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
        
    private bool isMovable;
    
    private Vector3 movementInput;
    
    private float deltaTime;
    
    private Vector3 targetVelocity;
    private Vector3 zeroVelocity = new Vector3(0f, 0f, 0f);
    private Vector3 smoothVelocity;
    private Vector3 playerFacingVel;
    
    private float facingAngle;
    private Transform playerTransform;
    
    private Vector3 prbVelocity;
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        
        isMovable = true;
        
        // playerAnimator.SetFloat("Horizontal", 0);
        // playerAnimator.SetBool("Walking", false);
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
        
        deltaTime = Time.fixedDeltaTime;
        
        targetVelocity = movementInput.normalized * deltaTime * movementSpeed * 50f;
        
        playerFacingVel = Quaternion.AngleAxis(facingAngle, Vector3.up) * targetVelocity;
        
        prbVelocity = playerRigidbody.velocity;
        prbVelocity.x = playerFacingVel.x;
        prbVelocity.z = playerFacingVel.z;
        playerRigidbody.velocity = prbVelocity;
    }
}
