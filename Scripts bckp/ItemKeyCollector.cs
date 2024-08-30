using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeyCollector : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public bool deactivateAfterCollect = true;
    public bool respawnAfterCollect = false;
    
    // public string itemName;
    // public bool keyItem;

    
    
    // PRIVATE VARIABLES
    
    private CapsuleCollider playerBodyCollider;
    private PlayerController playerController;
    
    private UIController uiController;
    
    
    
    // FUNCTIONS
    
    // Start is called before the first frame update
    private void Start() {
        
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        playerBodyCollider = playerObject.GetComponent<CapsuleCollider>();
        playerController = playerObject.GetComponent<PlayerController>();
        
        uiController = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        
        if (playerController.hasKey && !respawnAfterCollect){
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other == playerBodyCollider) {
            
            playerController.hasKey = true;
            
            uiController.UpdateKeyItemsPanel(playerController.hasDash, playerController.hasKey);
            
            if (deactivateAfterCollect) {
                gameObject.SetActive(false);
            }
        }
    }
}
