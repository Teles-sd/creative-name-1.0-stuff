using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThirdPartyFunctions;
// using static ThirdPartyFunctions.TPFs;

public class EnemyController : MonoBehaviour {

    
    // SERIALIZED
    
    [Header("# Idle Movement")]

    [SerializeField]
    [Range(1, 10)] private int movementSpeed = 4;
    
    [SerializeField]
    [Range(1, 10)] private float idleWalkMaxTime;
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    [SerializeField]
    [ContextMenuItem("Run Start()", "runStart")]
    [ContextMenuItem("Test idlePathFind", "testidlePathFind")]
    [ContextMenuItem("Test prints", "testprints")]
    private bool doTestStuff = false;
    private void runStart(){ Start(); }
    private void testidlePathFind(){
        walkPathsFound = idlePathFind(5);
    }
    private void testprints(){
        // print(enemyBodyCollider.center);
        // print(Vector3.down);
        // print(enemyBodyCollider.height);
        print(walkPathsFound[0].rayTargetCenter + checkCapStr);
        print(walkPathsFound[0].rayTargetCenter + checkCapEnd);
    }
    
    [SerializeField]
    private bool activateGizmos = false;
    
    [Space(10)]
    [SerializeField]
    private WalkPath[] walkPathsFound;
    
    
    
    // NOT SERIALIZED
    
    private CapsuleCollider enemyBodyCollider;
    // private Transform enemyTransform;
    
    private LayerMask everythingLayerMask = -1; // everything
    private Vector3 checkCapStr;
    private Vector3 checkCapEnd;

    private Transform playerTransform;
    private PlayerController playerController;
    private CapsuleCollider playerBodyCollider;
    
    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
    // Start is called before the first frame update
    void Start() {
        
        enemyBodyCollider = gameObject.GetComponent<CapsuleCollider>();
        // enemyTransform = gameObject.GetComponent<Transform>();
        
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        playerTransform = playerObject.transform;
        playerController = playerObject.GetComponent<PlayerController>();
        playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        
        // Stuff for idlePathFind calculation
        checkCapStr = Vector3.up * (enemyBodyCollider.height * 0.5f - enemyBodyCollider.radius);
        checkCapEnd = Vector3.down * (enemyBodyCollider.height * 0.5f - enemyBodyCollider.radius);
        
        // print("doe");
        // print( enemyBodyCollider.center );
        // print( enemyBodyCollider.bounds.center );
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    void FixedUpdate() {
        
    }
    
    private void OnDrawGizmos() {
        if (activateGizmos) {
            
            Color rayColor;
            
            foreach (var wpf in walkPathsFound){
                
                // if (wpf.rayHit){
                //     rayColor = Color.red;
                // } else {
                //     rayColor = Color.yellow;
                //     // rayColor = Color.cyan;
                //     // rayColor = Color.green;
                //     // rayColor = Color.white;
                //     // rayColor = new Color(1.0f, 0.64f, 0.0f); // orange
                // }
                
                // Debug.DrawRay(
                //     start:  wpf.rayOrigin,
                //     dir:    wpf.rayDirection * wpf.randomDist,
                //     color:  rayColor,
                //     duration:   0.0f, // in seconds, if 0 then is rendered 1 frame.
                //     depthTest:  true
                // );
                
                if (wpf.rayHit){
                    Gizmos.color = Color.red;
                } else {
                    Gizmos.color = Color.yellow;
                }
                
                Gizmos.DrawRay(
                    from:       wpf.rayOrigin,
                    direction:  wpf.rayDirection * wpf.randomDist
                );
                
                if (wpf.checkCollide){
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
    
    
    
    // FUNCTIONS & CLASSES
    
    [System.Serializable]
    private class WalkPath{
        public Vector3 rayOrigin;
        public Vector3 rayDirection;
        public float randomDist;
        public bool rayHit;
        public Vector3 rayTargetCenter;
        public bool checkCollide;
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
        
        return walkPathsFound;
    }
    
    private void idleMovement() {}
    private void agroMovement() {}
    
}




