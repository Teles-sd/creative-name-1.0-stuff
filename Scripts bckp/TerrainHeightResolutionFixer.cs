using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeightResolutionFixer : MonoBehaviour
{
    
    

    // PUBLIC VARIABLES
    
    [Header("Hub Terrain")]
    [ContextMenuItem("= setHubTerrainHeightmap", "setHubTerrainHeightmap")]
    public Terrain hubTerrain;
    
    
    
    [Header("hubTerrainHeightmap")]
    [ContextMenuItem("= getHubTerrainHeightmap", "getHubTerrainHeightmap")]
    [SerializeField] private int res = 0;
    
    public float[,] hubTerrainHeightmap;
    
    [ContextMenuItem("= Print stuff", "printStuff")]
    public Vector2Int asdad;
    
    
    
    
    [Header("OTHERhubTerrainHeightmap")]
    
    [ContextMenuItem("= setOTHERhubTerrainHeightmap", "setOTHERhubTerrainHeightmap")]
    public int scaleaaa = 1;
    
    
    public float[,] OTHERhubTerrainHeightmap;
    
    
    [ContextMenuItem("= OTHER Print stuff", "otherprintStuff")]
    public Vector2Int aaaaaa;
    
    
    
    
    
    [Header("This Terrain")]
    [ContextMenuItem("= Get this Terrain", "getThisTerrain")]
    [ContextMenuItem("= setThisTerrainHeightmap", "setThisTerrainHeightmap")]
    public Terrain thisTerrain;
    
    
    
    
    // GETTERS
    
    
    private void getHubTerrainHeightmap(){
        
        res = hubTerrain.terrainData.heightmapResolution;
        
        // print(hubTerrain.terrainData.heightmapResolution);
        hubTerrainHeightmap = hubTerrain.terrainData.GetHeights(
            xBase : 0,
            yBase : 0,
            width : res,
            height: res
        );
    }
    
    private void printStuff(){
        print(hubTerrainHeightmap[asdad.x,asdad.y]);
    }
    
    private void setOTHERhubTerrainHeightmap(){
        
        OTHERhubTerrainHeightmap = hubTerrainHeightmap;
        
        for (int i = 0; i < res; i++){
            for (int j = 0; j < res; j++){
                
                OTHERhubTerrainHeightmap[i,j] = hubTerrainHeightmap[i,j] * scaleaaa;
            }
        }
    }
    
    private void otherprintStuff(){
        print(OTHERhubTerrainHeightmap[aaaaaa.x,aaaaaa.y]);
    }
        
    private void getThisTerrain(){
        thisTerrain = GetComponent<Terrain>();
    }
    
    private void setThisTerrainHeightmap(){
        thisTerrain.terrainData.SetHeights(
            xBase : 0,
            yBase : 0,
            OTHERhubTerrainHeightmap
        );
    }
    
    private void setHubTerrainHeightmap(){
        hubTerrain.terrainData.SetHeights(
            xBase : 0,
            yBase : 0,
            OTHERhubTerrainHeightmap
        );
    }
    
}
