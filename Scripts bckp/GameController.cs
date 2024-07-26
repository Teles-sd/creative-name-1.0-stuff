using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;


    
public enum PlayerPositionsNames{
    Spawn,
    Hub,
    Well,
};

[System.Serializable]
public class PlayerPositionsVectors {
    
    public Vector3 Spawn;
    public Vector3 Hub;
    public Vector3 Well;
    
    public Vector3 this[PlayerPositionsNames _name]{
        get{
            return (Vector3)typeof(PlayerPositionsVectors).GetField( _name.ToString() ).GetValue(this);
        }
    }
}



public class GameController : MonoBehaviour {
    
    
    // PUBLIC VARIABLES
    
    [Header("# Context Menu Commands")]
    
    public Transform playerTransform;
    
    [ContextMenuItem("Set Current Position", "setCurrentPosition")]
    public PlayerPositionsNames sendPlayerToPosition;
    
    [ContextMenuItem("Reset playerPositions", "playerPositionsResetter")]
    public PlayerPositionsVectors positionsVectors;
    
    
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
    
    // private Transform playerTransform;
    private UIController uiControllerScript;
    
    private Scene thisScene;
    
    
    
    // FUNCTIONS
    
    // Use the static modifier to declare a static member, which belongs to the type itself rather than to a specific object.
    // While an instance of a class contains a separate copy of all instance fields of the class, there's only one copy of each static field.
    private static GameController instance;
    
    // Called first upon initialisation of an object
    private void Awake() {
        if(instance){
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called before the first frame update
    private void Start() {
        if ( !playerTransform ){
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
        
        uiControllerScript = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        
        amAlive();
    }
    
    private void amAlive(){
        if (doTestStuff){
            thisScene = SceneManager.GetActiveScene();
            print("Imma live now on '" + thisScene.name + "' Scene.");
        }
    }
    
    public void ChangeLevel(string sceneName) {
        
        print("GameController changing level");
        // Make player not movable
        // UI fade in
        // UI Loading Screen On
        uiControllerScript.ShowLoadingScreen(true);
        // Load Scene
        StartCoroutine( LoadingScene(sceneName) );
    }
    
    private IEnumerator LoadingScene(string sceneName) {
        
        AsyncOperation loadingAsyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        while( !loadingAsyncOp.isDone ){
            uiControllerScript.UpdateProgressBar(loadingAsyncOp.progress);
            yield return null;
        }
        
        // wait time
        // yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(0.5f);
        
        // UI fade out
        // UI Loading Screen Off
        uiControllerScript.ShowLoadingScreen(false);
        // Am Alive
        amAlive();
        // Make player movable
        // Print loaded Scenes
    }
    
    public void ExitGame() {
        Application.Quit();
    }
}
