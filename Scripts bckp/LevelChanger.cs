using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Events;

public class LevelChanger : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    // public Scene goToLevel;
    public string sceneName;
    
    // public UnityEvent changeLevel;
    
    
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
            
            print("Level Changer trigger");
            // changeLevel.Invoke();
            gameControllerScript.ChangeLevel(sceneName);
        }
    }
}
