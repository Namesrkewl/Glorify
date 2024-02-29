using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DeathShatter : MonoBehaviour {
    public List<SkinnedMeshRenderer> skinnedMeshRenderers; // Assign Skinned Mesh Renderers in the Inspector
    public GameObject originalCharacterSkin;
    private GameObject shatteredCharacterObject;
    private List<Mesh> characterMeshes = new List<Mesh>();
    private List<Material> materials = new List<Material>();
    private List<GameObject> objectsToShatter = new List<GameObject>();

    public void CreateStaticMesh() {
        
        characterMeshes.Clear();

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers) {
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);
            bakedMesh.name = skinnedMeshRenderer.sharedMesh.name;
            characterMeshes.Add(bakedMesh);
        }
    }

    public void ShatterCharacter() {
        if (!originalCharacterSkin || skinnedMeshRenderers.Count < 1) {
            ShatterUnskinned();
        } else {
            ShatterSkinned();
        }
    }

    private void ShatterSkinned() {
        // Check if a shattered version already exists
        if (shatteredCharacterObject != null) return;
        shatteredCharacterObject = new GameObject($"Shattered Remains of {transform.name}");
        shatteredCharacterObject.transform.position = transform.position + originalCharacterSkin.transform.localPosition;
        CreateStaticMesh();
        objectsToShatter.Clear();
        materials.Clear();

        for (int i = 0; i < characterMeshes.Count; i++) {
            // Create a new child GameObject for each mesh
            GameObject child = new GameObject(skinnedMeshRenderers[i].name);
            child.transform.SetParent(shatteredCharacterObject.transform);
            child.transform.localPosition = Vector3.zero; // Adjust as necessary
            child.transform.localRotation = transform.localRotation; // Adjust as necessary


            // Add Mesh Filter and Renderer, copy mesh and materials
            MeshFilter meshFilter = child.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
            meshFilter.mesh = characterMeshes[i];
            meshRenderer.material = skinnedMeshRenderers[i].material;
            objectsToShatter.Add(child);
            materials.Add(skinnedMeshRenderers[i].material);
        }
        //MeshShatter.ShatterMesh(objectsToShatter, materials);
        originalCharacterSkin.SetActive(false);
        shatteredCharacterObject.SetActive(false);
    }

    private void ShatterUnskinned() {
        //MeshShatter.instance.ShatterMesh(gameObject, GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().enabled = false;
    }

    void SpawnCharacter() {
        CreateStaticMesh();
        GameObject statue = new GameObject($"Statue of {transform.name}");
        statue.transform.position = transform.position + originalCharacterSkin.transform.localPosition;
        for (int i = 0; i < characterMeshes.Count; i++) {
            // Create a new child GameObject for each mesh
            GameObject child = new GameObject(skinnedMeshRenderers[i].name);
            child.transform.SetParent(statue.transform);
            child.transform.localPosition = Vector3.zero; // Adjust as necessary
            child.transform.localRotation = transform.localRotation; // Adjust as necessary


            // Add Mesh Filter and Renderer, copy mesh and materials
            MeshFilter meshFilter = child.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
            meshFilter.mesh = characterMeshes[i];
            meshRenderer.material = skinnedMeshRenderers[i].material;
        }
    }

    public void ResetCharacter() {
        // Destroy the shattered character object
        if (shatteredCharacterObject != null)
            DestroyImmediate(shatteredCharacterObject);

        // Re-enable the skinned mesh
        if (originalCharacterSkin) {
            originalCharacterSkin.SetActive(true);
        } else {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DeathShatter))]
    public class DeathShatterEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            DeathShatter script = (DeathShatter)target;
            if (GUILayout.Button("Shatter Character")) {
                script.ShatterCharacter();
            }

            if (GUILayout.Button("Reset Character")) {
                script.ResetCharacter();
            }

            if (GUILayout.Button("Spawn Character")) {
                script.SpawnCharacter();
            }
        }
    }
#endif
}