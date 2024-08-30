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
    
    
    // Singleton
    private static UIController instance;
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
        loadScreenPanel.SetActive(false);
        loadScreenPanel.GetComponent<Image>().fillAmount = 0f;
        
        loadScreenAnimator = loadScreenPanel.GetComponent<Animator>();
    }
    
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
    
    public void UpdateHealth(int value){
        if (value == 2){
            healthPip1.SetActive(true);
            healthPip2.SetActive(true);
        } else if (value == 1){
            healthPip1.SetActive(true);
            healthPip2.SetActive(false);
        } else {
            healthPip1.SetActive(false);
            healthPip2.SetActive(false);
        }
    }
    
    public void UpdateKeyItemsPanel(bool hasDash, bool hasKey){
        
        keyItemsPanel.SetActive(hasDash || hasKey);
        
        kipKey.SetActive(hasKey);
        kipDash.SetActive(hasDash);
    }
}
