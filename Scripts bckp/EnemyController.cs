using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using ThirdPartyFunctions;
// using static ThirdPartyFunctions.TPFs;

public class EnemyController : MonoBehaviour {

    
    // SERIALIZED
    
    [Header("# Idle Movement")]

    [SerializeField]
    [Range(1, 10)] private int movementSpeed = 4;
    
    [Space(10)]
    [SerializeField]
    [Range(1, 10)] private int idleWaitMinRandTime = 1;
    [SerializeField]
    [Range(1, 10)] private int idleWaitMaxRandTime = 7;
    
    [SerializeField]
    [Range(1, 10)] private int idlePathSearchQuantity = 5;
    [SerializeField]
    [Range(2, 10)] private int idleWalkMaxTime = 10;
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    [SerializeField]
    [ContextMenuItem("Run Start()", "runStart")]
    [ContextMenuItem("Test idlePathFind", "testidlePathFind")]
    [ContextMenuItem("Test prints", "testprints")]
    private bool doTestStuff = false;
    private void runStart(){ Start(); }
    private bool startCheck = false;
    private void testidlePathFind(){
        wpIndex = -1;
        walkPathsFound = idlePathFind(idlePathSearchQuantity);
    }
    private void testprints(){
        // print(enemyBodyCollider.center);
        // print(Vector3.down);
        // print(enemyBodyCollider.height);
    }
    
    [Space(10)]
    [SerializeField]
    private bool activateGizmos = false;

    [SerializeField]
    private bool writeToDebugText = false;
    
    // [Space(10)]
    // [SerializeField]
    // private WalkPath[] walkPathsFound;
    
    
    
    // NOT SERIALIZED
    
    private CapsuleCollider enemyBodyCollider;
    private Rigidbody enemyRigidbody;
    // private Transform enemyTransform;

    // private Transform playerTransform;
    // private PlayerController playerController;
    // private CapsuleCollider playerBodyCollider;
    
    private TextMeshProUGUI debugTextObject;
    
    private LayerMask everythingLayerMask = -1; // everything
    private Vector3 checkCapStr;
    private Vector3 checkCapEnd;
    
    private enemyStates state = enemyStates.idle;
    private enemyStates idleState = enemyStates.idleNone;
    private enemyStates detcState = enemyStates.detcNone;
    private enemyStates agroState = enemyStates.agroNone;
    
    private float idleWaitTime;
    private float idleWaitTimeCounter = 0;
    private WalkPath[] walkPathsFound;
    private int wpIndex = -1;               // default invalid value
    private float idleWalkTimeCounter = 0;
    

    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
    // Start is called before the first frame update
    void Start() {
        
        enemyBodyCollider = gameObject.GetComponent<CapsuleCollider>();
        enemyRigidbody = gameObject.GetComponent<Rigidbody>();
        // enemyTransform = gameObject.GetComponent<Transform>();
        
//         GameObject playerObject = GameObject.FindWithTag("Player");
//         
//         playerTransform = playerObject.transform;
//         playerController = playerObject.GetComponent<PlayerController>();
//         playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        
        debugTextObject = GameObject.FindWithTag("UIController").GetComponent<UIController>().debugTextObject;
        
        // Stuff for idlePathFind calculation
        checkCapStr = Vector3.up * (enemyBodyCollider.height * 0.5f - enemyBodyCollider.radius);
        checkCapEnd = Vector3.down * (enemyBodyCollider.height * 0.5f - enemyBodyCollider.radius);
        
        // just to instanciate this dude so he stops complaining
        walkPathsFound = idlePathFind(idlePathSearchQuantity);
        startCheck = true;
    }

    // Update is called once per frame
    void Update() {
        
        if (state == enemyStates.idle){
            idleLogic();
        }
        
        
        // DEBUG STUFF
        
        if (writeToDebugText) {
            writeToDebugFunc();
        }
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    void FixedUpdate() {
        
        if (state == enemyStates.idle){
            idleMovement();
        }
        
    }
    
    private void OnDrawGizmos() {
        if (activateGizmos) {
            
            // if (idleState == enemyStates.idleWalking){
            if (startCheck){
                foreach
                    (
                        var
                        wpf
                        in
                        walkPathsFound
                    )
                    {
                    
                    if (wpf.selected){
                        Gizmos.color = Color.green;
                    } else if (wpf.rayHit){
                        Gizmos.color = Color.red;
                    } else {
                        Gizmos.color = Color.yellow;
                    }
                    
                    Gizmos.DrawRay(
                        from:       wpf.rayOrigin,
                        direction:  wpf.rayDirection * wpf.randomDist
                    );
                    
                    if (wpf.selected){
                        Gizmos.color = Color.green;
                    } else if (wpf.checkCollide){
                        Gizmos.color = Color.red;
                    } else {
                        Gizmos.color = Color.yellow;
                    }
                    
                    TPFs.DrawWireCapsule(
                        wpf.rayTargetCenter + checkCapStr,
                        wpf.rayTargetCenter + checkCapEnd,
                        enemyBodyCollider.radius
                    );
                }
            }
        }
    }
    
    
    
    // FUNCTIONS & CLASSES
    
    [System.Serializable]
    private class WalkPath{
        public Vector3 rayOrigin;
        public Vector3 rayDirection;
        public float randomDist;
        public bool rayHit;
        
        public Vector3 rayTargetCenter;
        public bool checkCollide;
        
        public bool selected = false;
    }
    
    // private void idlePathFind(int searchQuantity, int minDist=1, int maxDist=5) {
    private WalkPath[] idlePathFind(int searchQuantity, int minDist=2, int maxDist=7) {
        
        // instanciate the array
        WalkPath[] walkPathsFound = new WalkPath[searchQuantity];
        
        for (int i=0; i<searchQuantity; i++){
            
            // instanciate each element of the array. Kinda stchewpid, but sure.
            walkPathsFound[i] = new WalkPath();

            walkPathsFound[i].rayOrigin = enemyBodyCollider.bounds.center;
            int randomAngle = Random.Range(0, 360);
            Vector3 rayDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
            walkPathsFound[i].rayDirection = rayDirection.normalized;
            walkPathsFound[i].randomDist = Random.Range( (float)minDist, (float)maxDist );
            
            // Vector3 rayOrigin = enemyBodyCollider.bounds.center;
            // int randomAngle = Random.Range(0, 360);
            // Vector3 rayDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
            // rayDirection = rayDirection.normalized;
            // float randomDist = Random.Range( (float)minDist, (float)maxDist );
            
            walkPathsFound[i].rayHit = Physics.Raycast(
                origin:     walkPathsFound[i].rayOrigin,
                direction:  walkPathsFound[i].rayDirection,
                maxDistance:walkPathsFound[i].randomDist,
                layerMask:              everythingLayerMask,
                queryTriggerInteraction:QueryTriggerInteraction.Ignore
                // queryTriggerInteraction:QueryTriggerInteraction.UseGlobal
                // queryTriggerInteraction:QueryTriggerInteraction.Collide
            );
            
            // Vector3 checkCapStr = enemyBodyCollider.center + Vector3.up * enemyBodyCollider.height * 0.5f;
            // Vector3 checkCapEnd = enemyBodyCollider.center + Vector3.down * enemyBodyCollider.height * 0.5f;
            
            // Vector3 rayTargetCenter = rayOrigin + rayDirection * randomDist;
            
            walkPathsFound[i].rayTargetCenter =
                walkPathsFound[i].rayOrigin +
                walkPathsFound[i].rayDirection * walkPathsFound[i].randomDist;
            
            walkPathsFound[i].checkCollide = Physics.CheckCapsule(
                start:  walkPathsFound[i].rayTargetCenter + checkCapStr,
                end:    walkPathsFound[i].rayTargetCenter + checkCapEnd,
                radius: enemyBodyCollider.radius * 0.95f,
                layerMask:              everythingLayerMask,
                queryTriggerInteraction:QueryTriggerInteraction.Ignore
            );
        }
            
        for (int i=0; i<searchQuantity; i++){
            if ( !walkPathsFound[i].rayHit && !walkPathsFound[i].checkCollide){
                walkPathsFound[i].selected = true;
                wpIndex = i;
                break;
            }
        }
        
        return walkPathsFound;
    }
    
    private enum enemyStates{
        idle,
        idleNone,
        idleWaiting,
        idlePathing,
        idleWalking,
        detc,
        detcNone,
        agro,
        agroNone,
    };
    
    private void idleLogic() {
        // state == enemyStates.idle;
        
        // wait random time
        if (idleState == enemyStates.idleNone){
            idleWaitTime = Random.Range( (float)idleWaitMinRandTime, (float)idleWaitMaxRandTime );
            idleState = enemyStates.idleWaiting;
            idleWaitTimeCounter = 0;
        }
        if (idleState == enemyStates.idleWaiting){
            idleWaitTimeCounter += Time.deltaTime;
            if (idleWaitTimeCounter > idleWaitTime){
                idleState = enemyStates.idlePathing;
            }
        }
        
        // walk randomly
        if (idleState == enemyStates.idlePathing){
            
            wpIndex = -1;
            walkPathsFound = idlePathFind(idlePathSearchQuantity);
            
            if (wpIndex == -1){
                idleState = enemyStates.idleNone;
            } else {
                idleState = enemyStates.idleWalking;
                idleWalkTimeCounter = 0;
            }
        }
        if (idleState == enemyStates.idleWalking){
            idleWalkTimeCounter += Time.deltaTime;
            if (idleWalkTimeCounter > idleWalkMaxTime){
                idleState = enemyStates.idleNone;
            }
        }
        
    }
    
    private void idleMovement() {
        // state == enemyStates.idle;
        
        if (idleState == enemyStates.idleWalking){
            
            Vector3 target = walkPathsFound[wpIndex].rayTargetCenter;
            Vector3 current = enemyBodyCollider.bounds.center;
            
            Vector3 forceVector = target - current;
            float distance = forceVector.magnitude;
            // speed = a + a*log_10 (x+1)
            // a - movementSpeed
            // x - distance, always positive
            float speed = movementSpeed + movementSpeed*Mathf.Log(distance,10);
            
            forceVector = forceVector.normalized * speed;
            
            enemyRigidbody.AddForce(
                force:  forceVector, 
                // mode:   ForceMode.Acceleration
                mode:   ForceMode.VelocityChange
            );
            
            if ( distance < 0.05f ){
                idleState = enemyStates.idleNone;
            }
            // idleWalkTimeCounter += Time.deltaTime;
            // if (idleWalkTimeCounter > idleWalkMaxTime){
            //     idleState = enemyStates.idleNone;
            // }
        }
    }
    
    private void detcLogic() {
        // state == enemyStates.detc;
    }
    
    private void detcMovement() {
        // state == enemyStates.detc;
    }
    
    private void agroLogic() {
        // state == enemyStates.agro;
    }
    
    private void agroMovement() {
        // state == enemyStates.agro;
    }
    
    private void writeToDebugFunc() {
        debugTextObject.text = string.Format(
            "    state : {0}\n" +
            "idleState : {1}\n" +
            "detcState : {2}\n" +
            "agroState : {3}\n" +
            "idleWaitTimeCounter : {4}\n" +
            "idleWalkTimeCounter : {5}\n" +
            "target : {6}\n" +
            "",
            new object[] {
                state,
                idleState,
                detcState,
                agroState,
                idleWaitTimeCounter,
                idleWalkTimeCounter,
                walkPathsFound[wpIndex].rayTargetCenter,
            }
        );
    }
    
}




