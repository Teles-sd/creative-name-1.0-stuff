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
    
    // public Transform playerTransform;
    
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
            
        if ( !playerTransform ){
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
            
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
    
    private Transform playerTransform;
    private PlayerController playerController;
    
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
        
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        playerTransform = playerObject.transform;
        playerController = playerObject.GetComponent<PlayerController>();
        
        uiControllerScript = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        currentSceneName = SceneManager.GetActiveScene().name;
        isChangingLevel = false;
    }
    
    public void ChangeLevel(string sceneName) {
        
        // Update isChangingLevel
        isChangingLevel = true;
        
        // Make player not movable
        playerController.isMovable = false;
        
        // Load Sequence
        StartCoroutine( LoadingSequence(sceneName) );
    }
    
    private IEnumerator LoadingSequence(string sceneName) {
        
        // UI Loading Screen On
        uiControllerScript.ActivateLoadScreen(true);
        uiControllerScript.FadeLoadScreen(true);
        yield return new WaitForSeconds(0.5f);
        
        // Load Scene
        previousSceneName = currentSceneName;
        AsyncOperation loadingAsyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        float smoothProgress = 0f;
        do{
            // uiControllerScript.UpdateProgressBar(loadingAsyncOp.progress);
            smoothProgress = Mathf.MoveTowards(smoothProgress, loadingAsyncOp.progress, 0.1f);
            uiControllerScript.UpdateProgressBar(smoothProgress);
            yield return null;
        // } while( !loadingAsyncOp.isDone );
        } while( smoothProgress < 1 );
        
        currentSceneName = SceneManager.GetActiveScene().name;
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        // Set Player location
        playerTransform.position = solDataScript.SpawnPositionFromScene(previousSceneName);
        
        // UI Loading Screen Off
        uiControllerScript.FadeLoadScreen(false);
        yield return new WaitForSeconds(0.5f);
        uiControllerScript.ActivateLoadScreen(false);
        
        // Make player movable
        playerController.isMovable = true;
        
        // Update isChangingLevel
        isChangingLevel = false;
    }
    
    public void ExitGame() {
        Application.Quit();
    }
}
