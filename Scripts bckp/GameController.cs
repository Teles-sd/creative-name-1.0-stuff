using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;



public class GameController : MonoBehaviour {
    
    
    // PUBLIC VARIABLES
    
//     [Header("# All Scene References")]
//     
//     public SceneReference hubSceneRef;
//     public SceneReference phase1SceneRef;
//     public SceneReference testingGroundsSceneRef;
//     
//     
//     [Space(10)]
    [Header("# Context Menu Commands")]
    
    public Transform playerTransform;
    
    [ContextMenuItem("Set Current Position", "setCurrentPosition")]
    public PlayerPositionsNames sendPlayerToPosition;
    
    [ContextMenuItem("Reset playerPositions", "playerPositionsResetter")]
    public PlayerPositionsVectors positionsVectors;
    
    
    [Space(10)]
    [Header("# Tests & Debug stuff")]
    
    [ContextMenuItem("doTestStuff Function", "doTestStufffunc")]
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
    
    private void doTestStufffunc(){
        // print(hubSceneRef);
        // print("a:" + hubSceneRef);
        // print(hubSceneRef.ScenePath);
    }
    
    
    
    // GETTERS
    
    public bool IsChangingLevel{ get{return isChangingLevel;} }
    
    
    
    // PRIVATE VARIABLES
    
    // private Transform playerTransform;
    private UIController uiControllerScript;
    private SceneOnLoadData solDataScript;
    
    private string previousSceneName;
    private string currentSceneName;
    private bool isChangingLevel = false;    
    
    // Singleton
    private static GameController instance;
    // Use the static modifier to declare a static member, which belongs to the type itself rather than to a specific object.
    // While an instance of a class contains a separate copy of all instance fields of the class, there's only one copy of each static field.
    
    
    
    // FUNCTIONS
    
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
        
        playerTransform = GameObject.FindWithTag("Player").transform;
        
        uiControllerScript = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        currentSceneName = SceneManager.GetActiveScene().name;
        isChangingLevel = false;
    }
    
    public void ChangeLevel(string sceneName) {
        
        // Update isChangingLevel
        isChangingLevel = true;
        
        // Make player not movable
        // UI fade in
        // UI Loading Screen On
        uiControllerScript.ShowLoadingScreen(true);
        
        // Load Scene
        StartCoroutine( LoadingScene(sceneName) );
    }
    
    private IEnumerator LoadingScene(string sceneName) {
        
        previousSceneName = currentSceneName;
        AsyncOperation loadingAsyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        do{
            uiControllerScript.UpdateProgressBar(loadingAsyncOp.progress);
            yield return null;
        } while( !loadingAsyncOp.isDone );
        
        currentSceneName = SceneManager.GetActiveScene().name;
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        // wait time
        yield return new WaitForSeconds(0.5f);
        
        // Set Player location
        playerTransform.position = solDataScript.SpawnPositionFromScene(previousSceneName);
        
        // UI fade out
        // UI Loading Screen Off
        uiControllerScript.ShowLoadingScreen(false);
        
        // Make player movable
        // Print loaded Scenes
        // Update isChangingLevel
        isChangingLevel = false;
    }
    
    public void ExitGame() {
        Application.Quit();
    }
}
