using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using FishNet.Managing.Logging;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class MeshShatter : NetworkBehaviour {
    #region Renderer Logic
    public List<SkinnedMeshRenderer> skinnedMeshRenderers; // Assign Skinned Mesh Renderers in the Inspector
    public GameObject originalCharacterSkin;
    private GameObject shatteredCharacterObject;
    private List<Mesh> characterMeshes = new List<Mesh>();
    private List<Material> materials = new List<Material>();
    private List<GameObject> objectsToShatter = new List<GameObject>();
    public float cubeSize = 0.2f;
    public int totalCubes = 50; // Total cubes to distribute among target meshes
    private List<GameObject> shatteredCubes = new List<GameObject>();
    private bool isShattered;

    private void Awake() {
        isShattered = false;
        shatteredCubes.Clear();
        objectsToShatter.Clear();
        materials.Clear();
    }

    public void CreateStaticMesh() {

        characterMeshes.Clear();
        Debug.Log("Creating Static Mesh!");

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers) {
            Debug.Log(skinnedMeshRenderer.name);
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);
            bakedMesh.name = skinnedMeshRenderer.sharedMesh.name;
            characterMeshes.Add(bakedMesh);
        }
    }

    [ObserversRpc]
    public void ShatterCharacter() {
        if (!originalCharacterSkin || skinnedMeshRenderers.Count < 1) {
            ShatterUnskinned();
        } else {
            ShatterSkinned();
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerShatterCharacter() {
        ShatterCharacter();
    }

    private void ShatterSkinned() {
        // Check if a shattered version already exists
        if (isShattered) return;
        isShattered = true;
        shatteredCharacterObject = new GameObject($"Shattered Remains of {transform.name}");
        shatteredCharacterObject.transform.SetParent(transform);
        shatteredCharacterObject.transform.position = transform.position + originalCharacterSkin.transform.localPosition;
        CreateStaticMesh();
        objectsToShatter.Clear();
        materials.Clear();
        Debug.Log("Shattering Skinned mesh!~");
        Debug.Log(characterMeshes.Count);
        for (int i = 0; i < characterMeshes.Count; i++) {
            Debug.Log(i);
            // Create a new child GameObject for each mesh
            GameObject child = new GameObject(skinnedMeshRenderers[i].name);
            child.transform.SetParent(shatteredCharacterObject.transform);
            child.transform.localPosition = Vector3.zero; // Adjust as necessary
            child.transform.localRotation = transform.localRotation; // Adjust as necessary


            // Add Mesh Filter and Renderer, copy mesh and materials
            MeshFilter meshFilter = child.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
            meshFilter.mesh = characterMeshes[i];
            meshRenderer.sharedMaterial = skinnedMeshRenderers[i].sharedMaterial;
            objectsToShatter.Add(child);
            materials.Add(skinnedMeshRenderers[i].sharedMaterial);
        }
        ShatterMesh(objectsToShatter, true);
        originalCharacterSkin.SetActive(false);
        shatteredCharacterObject.SetActive(false);
    }

    private void ShatterUnskinned() {
        if (isShattered) return;
        isShattered = true;
        materials.Clear();
        materials.Add(GetComponent<MeshRenderer>().material);
        List<GameObject> objectsToShatter = new List<GameObject> { gameObject };
        ShatterMesh(objectsToShatter, false);
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerSpawnCharacter() {
        SpawnCharacter();
    }

    [ObserversRpc]
    void SpawnCharacter() {
        CreateStaticMesh();
        GameObject statue = new GameObject($"Statue of {transform.name}");
        if (originalCharacterSkin != null) {
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
                meshRenderer.sharedMaterial = skinnedMeshRenderers[i].sharedMaterial;
            }
        } else {
            statue.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            statue.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            statue.transform.position = transform.position;
            statue.transform.localRotation = transform.localRotation; // Adjust as necessary
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerResetCharacter() {
        ResetCharacter();
    }

    [ObserversRpc]
    public void ResetCharacter() {
        // Destroy the shattered character object
        if (shatteredCharacterObject != null)
            DestroyImmediate(shatteredCharacterObject);
        foreach (var cube in shatteredCubes) {
            Destroy(cube);
        }
        shatteredCubes.Clear();
        if (!originalCharacterSkin || skinnedMeshRenderers.Count < 1) {
            GetComponent<MeshRenderer>().enabled = true;
        } else {
            originalCharacterSkin.SetActive(true);
        }
        isShattered = false;
    }
    #endregion

    #region Shattering Logic

    public void ShatterMesh(List<GameObject> targetMeshes, bool isSkinned) {
        if (targetMeshes == null || targetMeshes.Count == 0) return;


        if (!isSkinned) {
            if (!targetMeshes[0]) return;
            List<Vector3> points = SamplePointsInBounds(targetMeshes[0], totalCubes);
            CreateCubesAtPoints(points, 0);
        } else {
            
            float totalVolume = 0;
            List<float> volumes = new List<float>();

            foreach (GameObject meshObject in targetMeshes) {
                float volume = CalculateVolumeOfMesh(meshObject);
                volumes.Add(volume);
                totalVolume += volume;
            }

            for (int i = 0; i < targetMeshes.Count; i++) {
                float proportion = volumes[i] / totalVolume;
                int cubesForThisMesh = Mathf.RoundToInt(totalCubes * proportion);
                List<Vector3> points = SamplePointsInBounds(targetMeshes[i], cubesForThisMesh);
                CreateCubesAtPoints(points, i);
            }
        }
    }

    void CreateCubesAtPoints(List<Vector3> points, int index) {
        foreach (Vector3 point in points) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(transform);
            cube.transform.position = point;
            cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
            cube.GetComponent<MeshRenderer>().material = materials[index];

            var rb = cube.AddComponent<Rigidbody>();
            rb.mass = cubeSize;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation; // Allow only downward movement

            // Assign the cube to the ShatterCubes layer
            cube.layer = LayerMask.NameToLayer("ShatterCubes");
            rb.GetComponent<Collider>().material = Resources.Load<PhysicMaterial>("Materials/Rigid_Material");

            shatteredCubes.Add(cube);

            StartCoroutine(ShrinkAndDestroy(cube));
        }
        if (!originalCharacterSkin || skinnedMeshRenderers.Count < 1) {
            GetComponent<MeshRenderer>().enabled = false;
        } else {
            originalCharacterSkin.SetActive(false);
        }
    }

    float CalculateVolumeOfMesh(GameObject meshObject) {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        float volume = bounds.size.x * bounds.size.y * bounds.size.z;
        return volume;
    }

    List<Vector3> SamplePointsInBounds(GameObject meshObject, int sampleCount) {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < sampleCount; i++) {
            Vector3 point = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
            points.Add(meshObject.transform.TransformPoint(point)); // Transform point to world space
        }

        return points;
    }

    IEnumerator ShrinkAndDestroy(GameObject cube) {
        Debug.Log("Destroying Cubes");
        float shrinkDuration = 2f; // Duration in seconds before cube disappears
        float elapsedTime = 0;

        Vector3 originalScale = cube.transform.localScale;
        Debug.Log(originalScale);

        while (elapsedTime < shrinkDuration) {
            if (cube.IsDestroyed()) {
                yield break;
            }
            cube.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, (elapsedTime / shrinkDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shatteredCubes.Remove(cube);
        Destroy(cube);
    }

    #endregion
#if UNITY_EDITOR
    [CustomEditor(typeof(MeshShatter))]
    public class MeshShatterEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            MeshShatter script = (MeshShatter)target;
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