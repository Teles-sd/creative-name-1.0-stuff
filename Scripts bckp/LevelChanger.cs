using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    // public SceneReference goToSceneName;
    public string goToSceneName;
    
    
    // PRIVATE VARIABLES
    
    private GameController gameControllerScript;
    private CapsuleCollider playerBodyCollider;
    
    
    
    // FUNCTIONS
    
    // Start is called before the first frame update
    private void Start() {
        
        playerBodyCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
        gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other == playerBodyCollider && !gameControllerScript.IsChangingLevel){
            gameControllerScript.ChangeLevel(goToSceneName);
        }
    }
}
