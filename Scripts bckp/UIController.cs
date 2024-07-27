using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIController : MonoBehaviour {
    
    
    // PUBLIC VARIABLES
    
    [Header("# Loading Screen")]
    public GameObject loadingScreenPanel;
    public Image progressBarImage;
    
    [Space(10)]
    [Header("# Debug")]
    public GameObject debugPanel;
    
    
    
    // PRIVATE VARIABLES
    
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
        loadingScreenPanel.SetActive(false);
    }
    
    public void ShowLoadingScreen(bool show){
        if (show){
            loadingScreenPanel.SetActive(true);
        } else {
            loadingScreenPanel.SetActive(false);
            // Reset Load Screen
            UpdateProgressBar(0f);
        }
    }
    
    public void UpdateProgressBar(float value){
        progressBarImage.fillAmount = value;
    }
}
