using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    

    // PUBLIC VARIABLES
    
    public Transform playerTransform;
    
    
    
    [Space(10)]
    [Header("# Context Menu Commands")]
    
    [ContextMenuItem("Set Current Position", "setCurrentPosition")]
    public playerPositionsNames sendPlayerToPosition;
    
    [ContextMenuItem("Reset playerPositions", "playerPositionsResetter")]
    public playerPositionsVectors positionsVectors;
    
    public enum playerPositionsNames{
        Spawn,
        Hub,
        Well,
    };
    
    [System.Serializable]
    public class playerPositionsVectors {
        
        public Vector3 Spawn;
        public Vector3 Hub;
        public Vector3 Well;
        
        public Vector3 this[playerPositionsNames _name]{
            get{
                return (Vector3)typeof(playerPositionsVectors).GetField( _name.ToString() ).GetValue(this);
            }
        }
    }
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    public bool doTestStuff = false;

    // [Space(10)]
    // public bool writeToDebugText = false;
    // public TextMeshProUGUI debugTextObject;

    
    
    // CONTEXT MENU COMMANDS
    
    private void setCurrentPosition(){
        print("\nTp to " + positionsVectors[sendPlayerToPosition].ToString() );
        playerTransform.position = positionsVectors[sendPlayerToPosition];
    }
    
    private void playerPositionsResetter(){
        positionsVectors.Spawn = new Vector3(7,3,7);
        positionsVectors.Hub = new Vector3(44,15,71);
        positionsVectors.Well = new Vector3(62.5f,11,17);
    }
    
    
    
    // PRIVATE VARIABLES
    
    private Scene thisScene;
    
    
    
    // Start is called before the first frame update
    void Start() {
        if ( !playerTransform ){
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
        
        if (doTestStuff){
            thisScene = SceneManager.GetActiveScene();
            print("Imma live on '" + thisScene.name + "' Scene.");
        }
    }

    // Update is called once per frame
//     void Update() {
//         
//     }
}
