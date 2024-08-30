using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {
    
    
    // SERIALIZED
    
    [Header("# Loading Screen")]
    
    [SerializeField]
    private GameObject loadScreenPanel;
    [SerializeField]
    private Image progressBarImage;
    
    
    [Space(10)]
    [Header("# Debug")]
    
    [SerializeField]
    private GameObject debugPanel;
    public TextMeshProUGUI debugTextObject;
    
    
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
    
    
    
    // NOT SERIALIZED
    
    private Animator loadScreenAnimator;
    
    private PlayerController playerController;
    
    private Image healthPip1Image;
    private Color originalColor1;
    private Color transparColor1;
    private Image healthPip2Image;
    private Color originalColor2;
    private Color transparColor2;
    private Coroutine flashCoroutineCheck;
    
    
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
        loadScreenPanel.SetActive(false);
        loadScreenPanel.GetComponent<Image>().fillAmount = 0f;
        
        loadScreenAnimator = loadScreenPanel.GetComponent<Animator>();
        
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        
        healthPip1Image = healthPip1.GetComponent<Image>();
        healthPip2Image = healthPip2.GetComponent<Image>();
        
        originalColor1 = transparColor1 = healthPip1Image.color;
        transparColor1.a = 0;
        originalColor2 = transparColor2 = healthPip2Image.color;
        transparColor2.a = 0;
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
}
