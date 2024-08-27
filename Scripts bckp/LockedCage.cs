using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedCage : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public float openSpeed = 3f;
    
    public GameObject cageDoor;
    public GameObject cageNPC;
    
    
    
    
    // PRIVATE VARIABLES
    
    private CapsuleCollider playerBodyCollider;
    private PlayerController playerController;
    
    private bool isLocked = true;
    
//     private Vector3 doorClosedRot = new Vector3(270,359.433441,0);
//     private Vector3 doorOpenedRot = new Vector3(270,224.433456,0);
//     
//     Quaternion(-0.707098246,-0.00349600101,-0.00349600124,0.707098126)
//     Quaternion(-0.17963095,-0.683909893,-0.683909893,0.17963095)

    private Vector3 doorOpenedRot = new Vector3(270,225,0);
    
    // Vector3(270,44.9999962,0)
    
    // FUNCTIONS
    
    // Start is called before the first frame update
    private void Start() {
        
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        playerController = playerObject.GetComponent<PlayerController>();
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other == playerBodyCollider && playerController.hasKey && isLocked) {
            StartCoroutine( OpenCageDoor() );
            isLocked = false;
        }
    }

    private IEnumerator OpenCageDoor() {
        while(cageDoor.transform.localEulerAngles != doorOpenedRot){
            cageDoor.transform.localEulerAngles = Vector3.MoveTowards(
                current:cageDoor.transform.localEulerAngles,
                target:doorOpenedRot,
                maxDistanceDelta:openSpeed
            );
            yield return 0; 
        }
        print("ye");
    }
}
