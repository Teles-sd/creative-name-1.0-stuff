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
    
    private UIController uiController;
    private SceneOnLoadData solDataScript;
    
    private string previousSceneName;
    private string currentSceneName;
    
    private bool isChangingLevel = false;
    // private float smoothProgress = 0f;
    private bool isReloadingScene = false;
    
    
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
        
        uiController = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        uiController.UpdateKeyItemsPanel(playerController.hasDash, playerController.hasKey);
        
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        currentSceneName = SceneManager.GetActiveScene().name;
        isChangingLevel = false;
        isReloadingScene = false;
    }
    
    public void ReloadLevel() {
        
        // update isReloadingScene, for differences on LoadingSequence
        isReloadingScene = true;
        
        // honestly? do almost the same as the other way
        ChangeLevel(SceneManager.GetActiveScene().name);
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
        uiController.ActivateLoadScreen(true);
        uiController.FadeLoadScreen(true);
        yield return new WaitForSeconds(0.5f);
        
//         // If isReloadingScene, Unload Scene first.
//         if (isReloadingScene) {
//             AsyncOperation unloadingAsyncOp = SceneManager.UnloadSceneAsync(sceneName);
//         
//             smoothProgress = 0f;
//             do{
//                 smoothProgress = Mathf.MoveTowards(smoothProgress, unloadingAsyncOp.progress, 0.3f);
//                 uiController.UpdateProgressBar(smoothProgress);
//                 yield return null;
//             } while( smoothProgress < 1 );
//             
//             uiController.UpdateProgressBar(0);
//         }
        
        // Load Scene
        if (!isReloadingScene) {
            previousSceneName = currentSceneName;
        }
        AsyncOperation loadingAsyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        float smoothProgress = 0f;
        do{
            // uiController.UpdateProgressBar(loadingAsyncOp.progress);
            smoothProgress = Mathf.MoveTowards(smoothProgress, loadingAsyncOp.progress, 0.1f);
            uiController.UpdateProgressBar(smoothProgress);
            yield return null;
        // } while( !loadingAsyncOp.isDone );
        } while( smoothProgress < 1 );
        
        currentSceneName = SceneManager.GetActiveScene().name;
        solDataScript = FindAnyObjectByType<SceneOnLoadData>();
        
        // Set Player location
        playerTransform.position = solDataScript.SpawnPositionFromScene(previousSceneName);
        
        // If isReloadingScene, restore player health (and key items ?)
        if (isReloadingScene) {
            playerController.health = 2;
            uiController.UpdateHealth(2);
            // uiController.UpdateKeyItemsPanel(playerController.hasDash, playerController.hasKey);
        }
        
        // UI Loading Screen Off
        uiController.FadeLoadScreen(false);
        yield return new WaitForSeconds(0.5f);
        uiController.ActivateLoadScreen(false);
        
        // Make player movable
        playerController.isMovable = true;
        
        // Update isChangingLevel
        isChangingLevel = false;
        
        // And isReloadingScene
        if (isReloadingScene) {
            isReloadingScene = false;
            playerController.reloadScene = false;
        }
    }
    
    public void ExitGame() {
        Application.Quit();
    }
}
