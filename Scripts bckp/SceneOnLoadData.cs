using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    
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

[System.Serializable]
public class SpawnPoint {
    [HideInInspector] 
    public string name;
    public string fromSceneName;
    public Vector3 position;
}



public class SceneOnLoadData : MonoBehaviour {
    
    // PUBLIC VARIABLES
    
    [Header("# Scene Entrances")]
    public List<SpawnPoint> spawnPoints;
    
    
//     [Space(10)]
//     [Header("# Possible Commands")]
//     
//     public string sendPlayerToPosition;
//     public Vector3 positionsVectors;
    
    
    
    // PRIVATE VARIABLES
    
    private string defaultFromSceneName;
    
    // Singleton
    private static SceneOnLoadData instance;
    
    
    
    // FUNCTIONS
    
    // Called first upon initialisation of an object
    private void Awake() {
        if(instance){
            Destroy(gameObject);
        } else {
            instance = this;
        }
        
        defaultFromSceneName = spawnPoints[0].fromSceneName;
    }
    
    private void OnValidate(){
        foreach (var sp in spawnPoints){
            if ( !string.IsNullOrEmpty(sp.fromSceneName) ){
                sp.name = "from \"" + sp.fromSceneName + "\"";
            }
        }
    }
    
    public Vector3 SpawnPositionFromScene(string previousSceneName){
        if (previousSceneName == null){
            return FindSpawnByFromScene(defaultFromSceneName).position;
        } else {
            return FindSpawnByFromScene(previousSceneName).position;
        }
    }
    
    private SpawnPoint FindSpawnByName(string sceneName){
        foreach (var sp in spawnPoints){
            if (sp.name == sceneName){
                return sp;
            }
        }
        return null;
    }
    
    private SpawnPoint FindSpawnByFromScene(string sceneName){
        foreach (var sp in spawnPoints){
            if (sp.fromSceneName == sceneName){
                return sp;
            }
        }
        return null;
    }
    
    
}
