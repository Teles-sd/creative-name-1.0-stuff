using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {
    
    
    // SERIALIZED
    
    [Header("# Main Menu")]
    
    // [SerializeField]
    public GameObject mainMenuPanel;
    
    
    [Space(10)]
    [Header("# Pause Menu")]
    
    [SerializeField]
    private GameObject pauseMenuPanel;
    [SerializeField]
    private Slider mouseSensitivitySlider;
    
    
    [Space(10)]
    [Header("# Loading Screen")]
    
    [SerializeField]
    private GameObject loadScreenPanel;
    [SerializeField]
    private Image progressBarImage;
    
    
    [Space(10)]
    [Header("# Player UI")]
    
    [SerializeField]
    private GameObject healthPip1;
    [SerializeField]
    private GameObject healthPip2;
    
    [Space(10)]
    [SerializeField]
    private GameObject keyItemsPanel;
    [SerializeField]
    private GameObject kipKey;
    [SerializeField]
    private GameObject kipDash;
    
    
    [Space(10)]
    [Header("# Debug")]
    
    [SerializeField]
    private GameObject debugPanel;
    public TextMeshProUGUI debugTextObject;
    
    
    
    // NOT SERIALIZED
    
    // [SerializeField]
    // [ContextMenuItem("Run Start()", "Start")]
    // [ContextMenuItem("Run onClickStartGame()", "onClickStartGame")]
    private GameController gameController;
    
    private PlayerController playerController;
    
    private Animator loadScreenAnimator;
    
    private Image healthPip1Image;
    private Color originalColor1;
    private Color transparColor1;
    private Image healthPip2Image;
    private Color originalColor2;
    private Color transparColor2;
    private Coroutine flashCoroutineCheck;
    
    private bool gameHaveStarted = false;
    
    
    // Singleton
    private static UIController instance;
    // Use the static modifier to declare a static member, which belongs to the type itself rather than to a specific object.
    // While an instance of a class contains a separate copy of all instance fields of the class, there's only one copy of each static field.
    
    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
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
        
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        
        loadScreenAnimator = loadScreenPanel.GetComponent<Animator>();
        
        loadScreenPanel.SetActive(false);
        loadScreenPanel.GetComponent<Image>().fillAmount = 0f;
        
        healthPip1Image = healthPip1.GetComponent<Image>();
        healthPip2Image = healthPip2.GetComponent<Image>();
        
        originalColor1 = transparColor1 = healthPip1Image.color;
        transparColor1.a = 0;
        originalColor2 = transparColor2 = healthPip2Image.color;
        transparColor2.a = 0;
    }
    
    // Update is called once per frame
    private void Update() {
        
        // if ( Input.GetKeyDown(KeyCode.P) && !gameController.IsChangingLevel && gameHaveStarted){
        if ( Input.GetKeyDown(KeyCode.Escape) && !gameController.IsChangingLevel && gameHaveStarted){

            if (!pauseMenuPanel.activeSelf){
                Pause();
            } else {
                Unpause();
            }
        }
    }
    
    
    
    // FUNCTIONS
    
    
    public void ActivateLoadScreen(bool active){
        loadScreenPanel.SetActive(active);
        if (!active){
            // Reset Load Screen
            UpdateProgressBar(0f);
        }
    }
    
    public void FadeLoadScreen(bool fade){
        loadScreenAnimator.SetBool("loading", fade);
    }
    
    public void UpdateProgressBar(float value){
        progressBarImage.fillAmount = value;
    }
    
    public void UpdateHealth(int health){
        
        if (flashCoroutineCheck != null){
            StopCoroutine(flashCoroutineCheck);
        }

        flashCoroutineCheck = StartCoroutine(healthPipFlash(health));
    }
    
    private IEnumerator healthPipFlash(int health) {
        
        if (health == 2){
            healthPip1Image.color = originalColor1;
            healthPip2Image.color = originalColor2;
            
            healthPip1.SetActive(true);
            healthPip2.SetActive(true);
        } else {
            while (playerController.immune) {
                if (health == 1){
                    healthPip2Image.color = transparColor2;
                    yield return new WaitForSeconds(0.1f);
                    
                    healthPip2Image.color = originalColor2;
                    yield return new WaitForSeconds(0.1f);
                } else {
                    healthPip1Image.color = transparColor1;
                    yield return new WaitForSeconds(0.1f);
                    
                    healthPip1Image.color = originalColor1;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            
            if (health == 1){
                healthPip1.SetActive(true);
                healthPip2.SetActive(false);
            } else {
                healthPip1.SetActive(false);
                healthPip2.SetActive(false);
            }
        }
        
        // Set flashCoroutineCheck to null, signaling that it's finished
        flashCoroutineCheck = null;
    }
    
    public void UpdateKeyItemsPanel(bool hasDash, bool hasKey){
        
        keyItemsPanel.SetActive(hasDash || hasKey);
        
        kipKey.SetActive(hasKey);
        kipDash.SetActive(hasDash);
    }
    
    
    
    // MENU INTERACTION FUNCTIONS
    
    public void onClickStartGame() {
        if (!gameController){
            gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }
        
        gameHaveStarted = true;
        gameController.ChangeLevel("Hub");
    }
    
    public void onClickExit() {
        if (!gameController){
            gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }
        
        gameController.ExitGame();
    }
    
    private void Pause() {
        playerController.isMovable = false;
        Time.timeScale = 0;
        pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void Unpause() {
        // Cursor.lockState = CursorLockMode.Locked;
        pauseMenuPanel.SetActive(false);
        playerController.isMovable = true;
        Time.timeScale = 1;
    }
    
    public void onClickContinue() {
        Unpause();
    }
    
    public void onSliderValueChange() {
        if (!playerController){
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        
        playerController.cameraRotationSpeed = mouseSensitivitySlider.value;
    }
}
