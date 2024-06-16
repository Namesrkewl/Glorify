using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Object;
using FishNet;


public class TerrainManager : NetworkBehaviour
{
    [SerializeField] private Terrain terrain;
    private void Awake()
    {
        SpawnTrees();
    }

    private void SpawnTrees()
    {
        TerrainData terrainData = terrain.terrainData;

        //for (int index = 0; index < terrainData.treeInstances.Length; index++)
        foreach (TreeInstance treeInstance in terrainData.treeInstances)
        {
            // Spawn GameObject version of the tree
            //GameObject GOTree = Instantiate(terrainData.treePrototypes[treeInstance.prototypeIndex].prefab, new Vector3(treeInstance.position.x * terrainData.size.x, treeInstance.position.y * terrainData.size.y, treeInstance.position.z * terrainData.size.z), Quaternion.identity);

            /*
            GOTree.GetComponent<Collider>().enabled = true;
            GOTree.GetComponent<MeshRenderer>().enabled = true;
            InstanceFinder.ServerManager.Spawn(GOTree, null);
            */


            //Destroy (treeInstance) that's part of the terrain;

            //List<TreeInstance> tempTreeInstances = new List<TreeInstance>(terrainData.treeInstances);
            //tempTreeInstances.RemoveAt(index); 
            //terrainData.treeInstances = tempTreeInstances.ToArray


            // Get the terrain's position in the world space
            Vector3 terrainPosition = terrain.transform.position;

            // Convert normalized terrain coordinates to world coordinates
            Vector3 treeWorldPosition = new Vector3(
                treeInstance.position.x * terrainData.size.x + terrainPosition.x,
                treeInstance.position.y * terrainData.size.y + terrainPosition.y,
                treeInstance.position.z * terrainData.size.z + terrainPosition.z
            );

            // Instantiate the tree at the correct world position
            GameObject GOTree = Instantiate(
                terrainData.treePrototypes[treeInstance.prototypeIndex].prefab,
                treeWorldPosition,
                Quaternion.identity
            );

            GOTree.GetComponent<Collider>().enabled = true;
            //GOTree.GetComponent<MeshRenderer>().enabled = true;
            InstanceFinder.ServerManager.Spawn(GOTree, null);




        }

        // Then deletes all Terrain Trees
        //        List<TreeInstance> newTrees = new List<TreeInstance>(0);
        //terrainData.treeInstances = newTrees.ToArray();

    }
}