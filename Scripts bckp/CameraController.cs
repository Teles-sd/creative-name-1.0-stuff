using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CameraController : MonoBehaviour {   
    

    // PUBLIC VARIABLES
    
    [Header("# Camera Rotation")]
    
    // For The Camera at relative position Vector3(0,1,-6) and rotation Vector3(15,0,0),
    // the pivot shoud stay at 1-6*(2-sqrt(3)) units below the local origin,
    // i.e. Vector3(0,-0.607f,0)
    [Tooltip("Custom pivot for rotation (local position, in relation to player).")]
    [ContextMenuItem("Reset Property", "ResetRelativePivotPosition")]
    public Vector3 relativePivotPosition = new Vector3(0,-0.607f,0);
    
    
    [Header("# Tests & Debug stuff")]
    
    public bool doTestStuff = false;
    public bool activateGizmos = false;

    [Space(10)]
    public bool writeToDebugText = false;
    public TextMeshProUGUI debugTextObject;

    
    
    // RESETTERS
    
    private void ResetRelativePivotPosition(){ relativePivotPosition = new Vector3(0,-0.607f,0); }

    
    
    // PRIVATE VARIABLES
    
    private Transform playerTransform;
    private PlayerController pcScript;
    
    private Vector3 pivotPosition;
    private float cameraRotationXtracker;
    
    
    
    // Start is called before the first frame update
    void Start() {
        
        playerTransform = gameObject.transform.parent;
        pcScript = playerTransform.GetComponent<PlayerController>();
        
        cameraRotationXtracker = transform.rotation.eulerAngles.x;
        
    }

    // Update is called once per frame
    void Update() {       
        
        
        // DEBUG STUFF
        
        float angleDistance = Mathf.Abs(transform.rotation.eulerAngles.x - Quaternion.Euler(cameraRotationXtracker,0,0).eulerAngles.x);
        
        if (writeToDebugText) {
            debugTextObject.text = string.Format(
                "playerTransform.position {0}\n" +
                "activateGizmos {1}\n" +
                "transform.rotation.eulerAngles.x {2}\n" +
                "cameraRotationXtracker {3}\n" +
                "distance {4}\n" +
                "",
                new object[] {
                    playerTransform.position,
                    activateGizmos,
                    transform.rotation.eulerAngles.x,
                    cameraRotationXtracker,
                    angleDistance,
                }
            );
        }
        
        if (angleDistance > 1){
            Debug.Log(string.Format(
                "Error: Camera Rotation Tracker NOT WORKING!\n" +
                "\"transform.rotation.eulerAngles.x\" = {0}\n" +
                "          \"cameraRotationXtracker\" = {1}\n" +
                "                   \"angleDistance\" = {2}\n" +
                "",
                new object[] {
                    transform.rotation.eulerAngles.x,
                    cameraRotationXtracker,
                    angleDistance,
                }
            ));
        }
    }
    
    void OnDrawGizmos() {
        if (activateGizmos && playerTransform) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(center:pivotPosition, radius:0.1f);
        }
    }
    
    // FixedUpdate is called a fixed amount of times per second.
    // Frame-rate independent for physics calculations.
    private void FixedUpdate() {
                
        pivotPosition = playerTransform.position + relativePivotPosition;
        
        float angleStep = - (pcScript.getMouseInput().y * pcScript.cameraRotationSpeed);
        float newAngle = cameraRotationXtracker + angleStep;
        
        if ( -10 <= newAngle && newAngle <= 45 ){
            
            cameraRotationXtracker = newAngle;
            
            // Makes the camera rotate around the "point".
            // To rotate up-and-over around the pivot:
            // the "axis" should be the `Vector3.right` rotated to the position the player is facing,
            // and the "angle" is positive to "up-and-over" and negative for "down-and-under".
            transform.RotateAround(
                point: pivotPosition,
                axis:  Quaternion.AngleAxis(playerTransform.rotation.eulerAngles.y, Vector3.up) *  Vector3.right,
                angle: angleStep
            );
        }
    }
}










