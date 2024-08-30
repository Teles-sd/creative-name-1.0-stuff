using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFaceCamera : MonoBehaviour {
    
    
    //  SERIALIZED
    
    
    
    // NOT SERIALIZED
    
    private Transform npcTransform;
    private Transform mainCameraTransform;
    
    Vector3 targetDirection;
    
    
    // BUILT-IN EVENT FUNCTIONS
    
    
    // Start is called before the first frame update
    void Start() {
        
        npcTransform = GetComponent<Transform>();
        
        mainCameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        
        targetDirection = mainCameraTransform.position - npcTransform.position;
        targetDirection.y = 0;

        float rotationYAngleToFaceCamera = Vector3.SignedAngle(
            from:Vector3.back,
            to:targetDirection,
            axis:Vector3.up
        );

        npcTransform.eulerAngles = new Vector3(0, rotationYAngleToFaceCamera, 0);
        
        
        // BEYBLADE!!! BEYBLADE!!! LET IT RIP!!!
        
        // float rotationYAngleToFaceCamera = Vector3.Angle(targetDirection, Vector3.forward);
        //
        // npcTransform.Rotate(
        //     axis: Vector3.up,
        //     angle: rotationYAngleToFaceCamera
        // );
    }
}
