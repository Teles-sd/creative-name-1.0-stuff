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
    [Range(1, 7)] private int movementSpeed = 3;
    
    
    [Space(10)]
    [SerializeField]
    private int detcDistance;
    [SerializeField]
    private int agroDistance;
    
    
    [Space(10)]
    [SerializeField]
    [Range(1, 10)] private int idleWaitMinRandTime = 1;
    [SerializeField]
    [Range(1, 10)] private int idleWaitMaxRandTime = 7;
    [SerializeField]
    [Range(1, 10)] private int idlePathSearchQuantity = 5;
    [SerializeField]
    [Range(2, 7)] private int idleWalkTime = 7;
    
    
    [Space(10)]
    [SerializeField]
    [Range(1, 5)] private int detcWalkTime = 2;
    
    
    [Space(10)]
    [SerializeField]
    [Range(1, 5)] private int agroDashTime = 3;
    [SerializeField]
    [Range(2, 6)] private int agroDashSpeedMultiplier = 4;
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    [SerializeField]
    [ContextMenuItem("Run Start()", "Start")]
    [ContextMenuItem("Run resetAllSubStates()", "resetAllSubStates")]
    [ContextMenuItem("Test idlePathFind", "testidlePathFind")]
    [ContextMenuItem("Test prints", "testprints")]
    private bool doTestStuff = false;
    // private void runStart(){ Start(); }
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
    private CapsuleCollider playerBodyCollider;
    
    private TextMeshProUGUI debugTextObject;
    
    private LayerMask everythingLayerMask = -1; // everything
    private Vector3 checkCapStr;
    private Vector3 checkCapEnd;
    
    [Space(10)] [SerializeField] private enemyStates state = enemyStates.idle;
    // private enemyStates state = enemyStates.idle;
    private enemyStates idleState = enemyStates.idleReset;
    private enemyStates detcState = enemyStates.detcReset;
    private enemyStates agroState = enemyStates.agroReset;
    private bool detcTired;
    private float timeCounter = 0;
    
    
    private float idleWaitTime;
    private WalkPath[] walkPathsFound;
    private int wpIndex = -1;               // default invalid value
    
    private Vector3 currentTarget;
    
    private int detcWaitTime = 1;
    
    private int agroAimTime = 1;
    private Vector3 agroDashVelocity;
    

    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
    // Start is called before the first frame update
    void Start() {
        
        enemyBodyCollider = gameObject.GetComponent<CapsuleCollider>();
        enemyRigidbody = gameObject.GetComponent<Rigidbody>();
        // enemyTransform = gameObject.GetComponent<Transform>();
        
        GameObject playerObject = GameObject.FindWithTag("Player");
//         
//         playerTransform = playerObject.transform;
//         playerController = playerObject.GetComponent<PlayerController>();
        playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        
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
        
        if (state == enemyStates.detc){
            detcLogic();
        }
        
        if (state == enemyStates.agro){
            agroLogic();
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
        
        if (state == enemyStates.detc){
            detcMovement();
        }
        
        if (state == enemyStates.agro){
            agroMovement();
        }
        
    }
    
    private void OnDrawGizmos() {
        if (activateGizmos && startCheck) {
            
            if (state == enemyStates.idle){
            // if (startCheck){
                foreach(var wpf in walkPathsFound){
                    
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
            // } else if (state == enemyStates.detc){
            } else {
                
                if (state == enemyStates.detc){
                    Gizmos.color = Color.yellow;
                } else {
                    Gizmos.color = Color.red;
                }
                
                Gizmos.DrawRay(
                    from:       enemyBodyCollider.bounds.center,
                    direction:  currentTarget - enemyBodyCollider.bounds.center
                );
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
    
    // calculates enemy velocity proportional to distance from target
    private (Vector3 vel,float dist) calcVelDist(Vector3 current, Vector3 target){
        
        Vector3 velocityVector = target - current;
        float distance = velocityVector.magnitude;
        // speed = a + a*log_10 (x+1)
        // a - movementSpeed
        // x - distance, always positive
        float speed = movementSpeed + movementSpeed*Mathf.Log(distance+1,10);
        // speed = Mathf.Clamp(speed/2, movementSpeed*0.5f, movementSpeed*1.5f);
        
        // remove Y component before normalize
        velocityVector.y = 0;
        
        velocityVector = velocityVector.normalized * speed;
        // velocityVector = velocityVector.normalized * speed * 10;
        
        return (velocityVector,distance);
    }
    
    private bool RandomBool(){
        return Random.Range(0, 2) == 1;
    }
        
    // sets the Rigidbody velocity, ignoring (not modifying) its y component
    private void setRBxzVelocity(Vector3 vel){
        vel.y = enemyRigidbody.velocity.y;
        enemyRigidbody.velocity = vel;
    }
    
    private void resetAllStates(){
        state = enemyStates.idle;
        resetAllSubStates();
    }
    
    private void resetAllSubStates(){
        idleState = enemyStates.idleReset;
        detcState = enemyStates.detcReset;
        agroState = enemyStates.agroReset;
        detcTired = false;
        timeCounter = 0;
    }
    
    private enum enemyStates{
        idle,
        idleReset,
        idleWaiting,
        idlePathing,
        idleWalking,
        detc,
        detcReset,
        detcWaiting,
        detcWalking,
        agro,
        agroReset,
        agroAiming,
        agroDashing,
    };
    
    private void idleLogic() {
        // state == enemyStates.idle;
        
        // reset
        if (idleState == enemyStates.idleReset){
            idleWaitTime = Random.Range( (float)idleWaitMinRandTime, (float)idleWaitMaxRandTime );
            idleState = enemyStates.idleWaiting;
            timeCounter = 0;
        }
        
        // wait random time
        if (idleState == enemyStates.idleWaiting){
            timeCounter += Time.deltaTime;
            if (timeCounter > idleWaitTime){
                idleState = enemyStates.idlePathing;
            }
        }
        
        // pathing
        if (idleState == enemyStates.idlePathing){
            
            wpIndex = -1;
            walkPathsFound = idlePathFind(idlePathSearchQuantity);
            
            if (wpIndex == -1){
                idleState = enemyStates.idleReset;
            } else {
                idleState = enemyStates.idleWalking;
                timeCounter = 0;
            }
        }
        
        // walk randomly
        if (idleState == enemyStates.idleWalking){
            timeCounter += Time.deltaTime;
            if (timeCounter > idleWalkTime){
                idleState = enemyStates.idleReset;
            }
        }
    }
    
    private void idleMovement() {
        // state == enemyStates.idle;
        
        if (idleState == enemyStates.idleWalking){
            
            Vector3 current = enemyBodyCollider.bounds.center;
            Vector3 target = walkPathsFound[wpIndex].rayTargetCenter;
            
//             Vector3 velocityVector = target - current;
//             float distance = velocityVector.magnitude;
//             // speed = a + a*log_10 (x+1)
//             // a - movementSpeed
//             // x - distance, always positive
//             float speed = movementSpeed + movementSpeed*Mathf.Log(distance+1,10);
//             // speed = Mathf.Clamp(speed/2, movementSpeed*0.5f, movementSpeed*1.5f);
//             
//             velocityVector = velocityVector.normalized * speed;
//             // velocityVector = velocityVector.normalized * speed * 10;
            
            // var (velocityVector,distance) = calcVelDist(current,target);
            (Vector3 velocityVector,float distance) = calcVelDist(current,target);
            
            // velocityVector.y = enemyRigidbody.velocity.y;
            // enemyRigidbody.velocity = velocityVector;
            setRBxzVelocity(velocityVector);
            
            // enemyRigidbody.AddForce(
            //     force:  velocityVector, 
            //     // mode:   ForceMode.Acceleration
            //     mode:   ForceMode.VelocityChange
            // );
            
            if ( distance < 0.1f ){
                
                idleState = enemyStates.idleReset;
                // enemyRigidbody.velocity = Vector3.Scale(enemyRigidbody.velocity, new Vector3(0,1,0));
                setRBxzVelocity(Vector3.zero);
                
                // enemyRigidbody.AddForce(
                //     force:  Vector3.zero, 
                //     mode:   ForceMode.VelocityChange
                // );
            }
            // timeCounter += Time.deltaTime;
            // if (timeCounter > idleWalkTime){
            //     idleState = enemyStates.idleReset;
            // }
        }
    }
    
    private void detcLogic() {
        // state == enemyStates.detc;
        
        // reset
        if (detcState == enemyStates.detcReset){
            detcState = enemyStates.detcWalking;
            timeCounter = 0;
            
            // when detc state is reset, decide at random if its gonna stop between intervals
            detcTired = RandomBool();
        }
        
        // walk towards player
        if (detcState == enemyStates.detcWalking && detcTired){
            timeCounter += Time.deltaTime;
            if (timeCounter > detcWalkTime){
                detcState = enemyStates.detcWaiting;
                timeCounter = 0;
            }
        }
            
        // wait small time
        if (detcState == enemyStates.detcWaiting){
            timeCounter += Time.deltaTime;
            if (timeCounter > detcWaitTime){
                detcState = enemyStates.detcWalking;
                timeCounter = 0;
            }
        }
    }
    
    private void detcMovement() {
        // state == enemyStates.detc;
        
        if (detcState == enemyStates.detcWalking){
            
//             Vector3 current = enemyBodyCollider.bounds.center;
//             // Vector3 target = playerBodyCollider.bounds.center;
//             currentTarget = playerBodyCollider.bounds.center;
//             
//             (Vector3 velocityVector,_) = calcVelDist(current,currentTarget);
            
            currentTarget = playerBodyCollider.bounds.center;

            (Vector3 velocityVector,_) = calcVelDist(enemyBodyCollider.bounds.center,currentTarget);
            
            setRBxzVelocity(velocityVector);
        }
    }
    
    private void agroLogic() {
        // state == enemyStates.agro;
        
        // reset
        if (agroState == enemyStates.agroReset){
            agroState = enemyStates.agroAiming;
            timeCounter = 0;
        }
            
        // wait to aim/track
        if (agroState == enemyStates.agroAiming){
            
            timeCounter += Time.deltaTime;
            currentTarget = playerBodyCollider.bounds.center;
            
            if (timeCounter > agroAimTime){
                
                // randomly decide if attacks or chases some more
                if (RandomBool()){
                    agroState = enemyStates.agroDashing;
                    timeCounter = 0;
                    
                    agroDashVelocity = currentTarget - enemyBodyCollider.bounds.center;
                    agroDashVelocity.y = 0;
                    agroDashVelocity = agroDashVelocity.normalized * movementSpeed * agroDashSpeedMultiplier;
                    
                }else{
                    agroState = enemyStates.agroReset;
                }
            }
        }
        
        // dash chosen direction
        if (agroState == enemyStates.agroDashing){
            timeCounter += Time.deltaTime;
            if (timeCounter > agroDashTime){
                agroState = enemyStates.agroReset;
            }
        }
    }
    
    private void agroMovement() {
        // state == enemyStates.agro;
        
        if (agroState == enemyStates.agroDashing){
            setRBxzVelocity(agroDashVelocity);
        }
    }
    
    private void writeToDebugFunc() {
        debugTextObject.text = string.Format(
            "       state : {0}\n" +
            "\n" +
            "   idleState : {1}\n" +
            "   detcState : {2}\n" +
            "   agroState : {3}\n" +
            "\n" +
            "   detcTired : {4}\n" +
            "timeCounter : {5}\n" +
            "",
            new object[] {
                state,
                idleState,
                detcState,
                agroState,
                detcTired,
                timeCounter,
            }
        );
        // debugTextObject.text = string.Format(
        //     "       state : {0}\n" +
        //     "   idleState : {1}\n" +
        //     "   detcState : {2}\n" +
        //     "   agroState : {3}\n" +
        //     "timeCounter : {4}\n" +
        //     "timeCounter : {5}\n" +
        //     "target : {6}\n" +
        //     "",
        //     new object[] {
        //         state,
        //         idleState,
        //         detcState,
        //         agroState,
        //         timeCounter,
        //         timeCounter,
        //         wpIndex == -1 ? "none" : walkPathsFound[wpIndex].rayTargetCenter,
        //     }
        // );
    }
    
}




