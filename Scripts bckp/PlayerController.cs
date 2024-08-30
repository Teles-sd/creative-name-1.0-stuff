using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerController : MonoBehaviour {
    

    // SERIALIZED
    
    [Header("# Movement")]

    [Range(1, 10)] public int movementSpeed = 5;
    [Range(3f, 15f)] public float cameraRotationSpeed = 9f;
    
    [Space(10)]
    [Range(3, 13)] public int jumpForce = 7;
    [Range(0f, 1f)] public float jumpGlideMaxTime = 0.75f;
    [Range(1f, 5f)] public float gravityScale = 2f;
    [Range(0.1f, .5f)] public float coyoteTime = 0.25f;
    
    [Space(10)]
    [Range(2, 5)] public int dashSpeedMultplier = 4;
    [Range(0f, 1f)] public float dashTotalTime = 0.5f;
    
    
    [Space(10)]
    [Header("# Misc")]
    [Range(2, 7)] public int immunityTime = 3;
    
    
    [Space(10)]
    [Header("# Key Items")]

    public bool hasDash = false;
    public bool hasKey = false;
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    public bool doTestStuff = false;
    public bool activateGizmos = false;

    [Space(10)]
    public bool writeToDebugText = false;
    
    
    
    // GETTERS
    
    public Vector2 getMouseInput(){ return mouseInput; }

    
    
    // RESETTERS
    
    
    
    
    // NOT SERIALIZED
    
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Animator playerAnimator;
    private CapsuleCollider playerBodyCollider;
    private SpriteRenderer playerSprite;
    
    private UIController uiController;
    private TextMeshProUGUI debugTextObject;
    
    private GameController gameController;
    
    private Vector3 movementInput = Vector3.zero;
    private Vector2 mouseInput = Vector2.zero;
    
    [HideInInspector] public bool isMovable;
    private bool cursorLocked;
    
    private Vector3 prbVelocity;
    
    private float facingAngle;
    private Vector3 inputDirection;
    
    // Foot box size (relative to the player,
    // but not reflected on Gizmos if Player scale is changed).
    private Vector3 footBoxSize = new Vector3(0.707f,0.15f,0.707f);
    private Collider[] footColliders;
    private LayerMask everythingLayerMask = -1; // everything
    
    private bool isFooted = false;
    private bool coyoteJumpAllow = false;
    private float coyoteTimeCounter = 0f;
    
    private bool jumpSeqnc = false;
    private bool jumpStart = false;
    private bool jumpHoldn = false;
    private bool jumpGlide = false;
    private float glideTime = 0f;
    
    private bool dashAllow = false;
    [HideInInspector] public bool dashSeqnc = false;
    private Vector3 dashDirection;
    private float dashSpeed = 0f;
    private float dashTimer = 0f;
    
    [HideInInspector] public int health = 2;     // max: 2
    [HideInInspector] public bool immune = false;
    private float immunityTimeCounter = 0;
    private bool applyKnockback = false;
    private Vector3 knockDirection;
    
    [HideInInspector] public bool reloadScene = false;
    
    
    // Singleton
    private static PlayerController instance;
    // Use the static modifier to declare a static member, which belongs to the type itself rather than to a specific object.
    // While an instance of a class contains a separate copy of all instance fields of the class, there's only one copy of each static field.
    
    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
    // Called first upon initialisation of an object
    private void Awake() {
        if(instance){
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider>();
        playerSprite = GetComponent<SpriteRenderer>();
        
        isMovable = true;
        // cursorLocked = false;
        // cursorLocked = true;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.lockState = CursorLockMode.None;
        if (Cursor.lockState == CursorLockMode.None){
            cursorLocked = false;
        }
                
        
        isFooted = false;
        jumpSeqnc = false;
        jumpStart = false;
        jumpHoldn = false;
        jumpGlide = false;
        
        facingAngle = playerTransform.rotation.eulerAngles.y;
        dashDirection = Quaternion.AngleAxis(facingAngle, Vector3.up) * Vector3.forward;
    }
    
    // Start is called before the first frame update
    private void Start() {
        
        uiController = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        debugTextObject = uiController.debugTextObject;

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    
    // Update is called once per frame
    private void Update() {
        
        updateIsFooted();
        
        if (isMovable && !reloadScene) {
            
            movementInputLogic();
            jumpInputLogic();
            dashInputLogic();
            mouseInputLogic(); // better after dash logic, due to same mouse input
        
        } else {
            movementInput = Vector3.zero;
        }
        
        animatorParametersUpdate();
        
        if (immune) {
            takeDamageLogic();
        }
        
        // DEBUG STUFF
        
        if (writeToDebugText) {
            writeToDebugFunc();
        }
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
        
        // Variable used to store values for movement, jump and dash
        prbVelocity = playerRigidbody.velocity;
        
        if ( !applyKnockback ){
            movementCalc();
            jumpCalc();
            dashCalc();
            cameraCalc();
        } else {
            takeDamageKnock();
        }
        
        playerRigidbody.velocity = prbVelocity;
    }
    
    // On Damage
    private void OnCollisionEnter(Collision collision) {
        
        foreach (ContactPoint contact in collision.contacts) {
            
            if ( contact.otherCollider.CompareTag("Enemy") && !immune && !dashSeqnc && !reloadScene ){
                
                // Set controll variables
                immune = true;
                immunityTimeCounter = 0;
                applyKnockback = true;
                
                health -= 1;
                
                if (health < 1){
                    reloadScene = true;
                }
                
                // Flash animation
                StartCoroutine(damageImmuneFlash());
                
                // Update UI
                uiController.UpdateHealth(health);
                
                // Set knockback direction
                knockDirection = contact.normal;
                // knockDirection *= -1;                       // oppisite direction
                knockDirection.y = 0;                       // on the XZ plane
                knockDirection = knockDirection.normalized;
                knockDirection.y = 1;                       // 45 deg. with XZ plane (because normalized)
                knockDirection = knockDirection.normalized;
                
                // Debug.DrawRay(contact.point, knockDirection * 5, Color.white, 5);
                // Debug.DrawRay(contact.point, knockDirection * 5, Color.black, 5);
                
                break;
            }
        }
    }
    
    private void OnDrawGizmos() {
        if (activateGizmos) {
            if (isFooted){
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            
            if (playerTransform){
                Gizmos.DrawWireCube(
                    center:footBoxCenter(),
                    size:footBoxSize
                );
            } else {
                playerTransform = GetComponent<Transform>();
            }
        }
    }
    
    
    
    // AUXILIARY FUNCTIONS
    
    
    private Vector3 footBoxCenter() {
        // 0.4f under the base of the playerBodyCollider so it still overlaps a bit with it
        return playerTransform.position - new Vector3(0, footBoxSize.y * 0.4f ,0);
    }
    
    private void updateIsFooted() {
        
        footColliders = Physics.OverlapBox(
            center:footBoxCenter(),
            halfExtents:footBoxSize*0.5f,
            orientation:Quaternion.identity,
            layerMask:everythingLayerMask,
            queryTriggerInteraction:QueryTriggerInteraction.Ignore
        );

        isFooted = false;
        
        foreach (var fc in footColliders){
            if ( fc != playerBodyCollider ){
                isFooted = true;
                break;
            }
        }
    }
    
    private void updateCoyoteJump() {
        
        if (!coyoteJumpAllow && isFooted){
            coyoteJumpAllow = true;
            coyoteTimeCounter = 0f;
        }
        
        if (coyoteJumpAllow && !isFooted){
            coyoteTimeCounter += Time.deltaTime;
        }
        
        if (coyoteTimeCounter >= coyoteTime){
            coyoteJumpAllow = false;
        }
    }
    
    // sets the prbVelocity, ignoring (not modifying) its y component
    private void setPRBxzVelocity(Vector3 vel){
        vel.y = prbVelocity.y;
        prbVelocity = vel;
    }
    
    private IEnumerator damageImmuneFlash() {
        
        Color originalColor;    // original color
        Color transparColor;    // trasparent color
        
        originalColor = transparColor = playerSprite.color;
        transparColor.a = 0;
        
        while (immune) {
            playerSprite.color = transparColor;
            yield return new WaitForSeconds(0.1f);
            
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    
    
    // LOGIC & CALC FUNCTIONS (update & fixed update)
    
    private void movementInputLogic() {
        
        // MOVEMENT
        
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.z = Input.GetAxisRaw("Vertical");
        
        facingAngle = playerTransform.rotation.eulerAngles.y;
        
    }
    
    private void jumpInputLogic() {
        
        // JUMP
        
        updateCoyoteJump();
        
        // conditions to initiate jump sequence
        if (Input.GetButtonDown("Jump") && coyoteJumpAllow && !jumpSeqnc){
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
                
                if ( !Input.GetButton("Jump") || glideTime > jumpGlideMaxTime){
                    jumpGlide = false;
                }
            }
        }
    }
    
    private void dashInputLogic() {
        
        // DASH
        
        if (!dashSeqnc && inputDirection != Vector3.zero){
            dashDirection = inputDirection;
        }
        
        if ( Input.GetKeyDown(KeyCode.Mouse0) && dashAllow && cursorLocked){
            dashSeqnc = true;
            dashAllow = false;
            dashTimer = 0f;
        }
        
        if (!dashAllow && hasDash && !dashSeqnc && isFooted){
            dashAllow = true;
        }
    }
        
    private void mouseInputLogic() {
        
        // MOUSE INPUT (for camera rotation)
        
        // toggle cursor lock when the "L" key is pressed
        if ( Input.GetKeyDown(KeyCode.L) || (Input.GetKeyDown(KeyCode.Mouse0) && !cursorLocked) ){
            
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
    }
    
    private void animatorParametersUpdate() {
        
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
        
    private void movementCalc() {
        
        // MOVEMENT (if not dashing)
        
        inputDirection = Quaternion.AngleAxis(facingAngle, Vector3.up) * movementInput;
        inputDirection = inputDirection.normalized;
        
        if (!dashSeqnc){
            // prbVelocity = inputDirection.normalized * movementSpeed;
            setPRBxzVelocity(inputDirection * movementSpeed);
        }
        
        // prbVelocity.x = horizontalVelocity.x;
        // prbVelocity.z = horizontalVelocity.z;
    }
    
    private void jumpCalc() {
        
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
            playerRigidbody.AddForce(Physics.gravity * (gravityScale - 1), ForceMode.Acceleration);
        }
    }
    
    private void dashCalc() {
        
        // DASH
        
        if (dashSeqnc){
            
            dashSpeed = Mathf.Lerp(movementSpeed * dashSpeedMultplier, movementSpeed, dashTimer/dashTotalTime);
            
            // prbVelocity =  dashDirection.normalized * dashSpeed;
            setPRBxzVelocity(dashDirection * dashSpeed);
            
            dashTimer += Time.fixedDeltaTime;
            
            if (dashTimer >= dashTotalTime){
                dashSeqnc = false;
            }
        }
        
        // prbVelocity.x = horizontalVelocity.x;
        // prbVelocity.z = horizontalVelocity.z;
    }
        
    private void cameraCalc() {
        
        // CAMERA ROTATION
        
        playerTransform.Rotate(
            axis: Vector3.up,
            angle: mouseInput.x * cameraRotationSpeed
        );
    }
    
    private void takeDamageLogic() {
        immunityTimeCounter += Time.deltaTime;
        if (immunityTimeCounter > immunityTime) {
            immune = false;
        }
        
        if (immunityTimeCounter > 1.5 && reloadScene){
        // if (other == playerBodyCollider && !gameController.IsChangingLevel){
            
            reloadScene = false;
            gameController.ReloadLevel();
        }
    }
    
    private void takeDamageKnock() {
        
        // prbVelocity = knockDirection * knockMultiplier;
        // prbVelocity += knockDirection * (knockMultiplier * (1-immunityTimeCounter));
        prbVelocity = knockDirection * (20 * (1-immunityTimeCounter) );

        if (immunityTimeCounter > 0.5f ) {
            applyKnockback = false;
            prbVelocity = Vector3.zero;
        }
        
        // playerRigidbody.AddForce(
        //     force:  prbVelocity = knockDirection * knockMultiplier,
        //     mode:   ForceMode.VelocityChange
        // );
        
        // applyKnockback = false;
    }
    
    private void writeToDebugFunc() {
        debugTextObject.text = string.Format(
            "immune: {0}\n" +
            "immunityTimeCounter: {1}\n" +
            "applyKnockback: {2}\n" +
            "knockDirection: {3}\n" +
            "",
            new object[] {
                immune,
                immunityTimeCounter,
                applyKnockback,
                knockDirection,
            }
        );
    }
}
