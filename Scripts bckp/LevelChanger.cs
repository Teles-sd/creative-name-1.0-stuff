using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public SceneReference goToLevel;
    
    
    // PRIVATE VARIABLES
    
    private GameController gameControllerScript;
    private CapsuleCollider playerBodyCollider;
    
    
    
    // Start is called before the first frame update
    private void Start() {
        
        playerBodyCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
        gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        
        // changeLevel.AddListener(gameControllerScript.ChangeLevel);
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other == playerBodyCollider){
            
            // print("Level Changer trigger");
            gameControllerScript.ChangeLevel(goToLevel);
        }
    }
}
