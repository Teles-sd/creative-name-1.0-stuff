using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDashCollector : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public bool deactivateAfterCollect = true;
    public bool respawnAfterCollect = false;
    
    // public string itemName;
    // public bool keyItem;

    
    
    // PRIVATE VARIABLES
    
    private CapsuleCollider playerBodyCollider;
    private PlayerController playerController;
    
    // private bool isCollected;
    
    
    
    // FUNCTIONS
    
    // Start is called before the first frame update
    private void Start() {
        
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        playerController = playerObject.GetComponent<PlayerController>();
        
        if (playerController.hasDash && !respawnAfterCollect){
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other == playerBodyCollider) {
            
            playerController.hasDash = true;
            
            if (deactivateAfterCollect) {
                gameObject.SetActive(false);
            }
        }
    }
}
