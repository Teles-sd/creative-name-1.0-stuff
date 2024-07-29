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
    public string name;
    public Vector3 position;
    public string fromSceneName;
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
    }
    
    public Vector3 SpawnPositionFromScene(string previousSceneName){
        return FindSpawnByFromScene(previousSceneName).position;
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
