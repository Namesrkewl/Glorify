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
        Debug.Log(terrainData.treeInstances[0].position);

        for (int index = 0; index < terrainData.treeInstances.Length; index++)
        {
            // Spawn GameObject version of the tree
            TreeInstance treeInstance = terrainData.treeInstances[index];
            GameObject GOTree = Instantiate(terrainData.treePrototypes[treeInstance.prototypeIndex].prefab, new Vector3(treeInstance.position.x * terrainData.size.x, treeInstance.position.y * terrainData.size.y, treeInstance.position.z * terrainData.size.z), Quaternion.identity);
            GOTree.GetComponent<Collider>().enabled = true;
            //GOTree.GetComponent<MeshRenderer>().enabled = true;
            InstanceFinder.ServerManager.Spawn(GOTree, null);

            //Destroy (treeInstance) that's part of the terrain;
            /*
            List<TreeInstance> tempTreeInstances = new List<TreeInstance>(terrainData.treeInstances);
            tempTreeInstances.RemoveAt(index);
            terrainData.treeInstances = tempTreeInstances.ToArray();
            */

        }

    }
}