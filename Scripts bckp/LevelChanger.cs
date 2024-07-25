using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public Scene goToLevel;
    
    
    // PRIVATE VARIABLES
    
    private CapsuleCollider playerBodyCollider;
    
    
    
    // Start is called before the first frame update
    private void Start() {
        playerBodyCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other = playerBodyCollider){
            print("Change Level");
        }
    }
}
