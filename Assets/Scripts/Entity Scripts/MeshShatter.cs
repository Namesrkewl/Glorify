using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;

public class MeshShatter : NetworkBehaviour {
    public float cubeSize = 0.2f;
    public int totalCubes = 50; // Total cubes to distribute among target meshes
    private List<GameObject> shatteredCubes = new List<GameObject>();
    public GameObject shatterCubes;

    public static MeshShatter instance;

    public override void OnStartServer() {
        base.OnStartServer();
        instance = this;
    }

    public void ShatterMesh(List<GameObject> targetMeshes, List<Material> targetMaterials) {
        if (targetMeshes == null || targetMeshes.Count == 0) return;

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
            CreateCubesAtPoints(points, targetMaterials[i]);
        }
    }

    public void ShatterMesh(GameObject targetMesh, Material material) {
        if (!targetMesh) return;

        List<Vector3> points = SamplePointsInBounds(targetMesh, totalCubes);
        CreateCubesAtPoints(points, material);
    }

    void CreateCubesAtPoints(List<Vector3> points, Material targetMaterial) {
        foreach (Vector3 point in points) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(shatterCubes.transform);
            cube.transform.position = point;
            cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
            cube.GetComponent<MeshRenderer>().material = targetMaterial;

            var rb = cube.AddComponent<Rigidbody>();
            rb.mass = cubeSize;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation; // Allow only downward movement

            // Assign the cube to the ShatterCubes layer
            cube.layer = LayerMask.NameToLayer("ShatterCubes");
            rb.GetComponent<Collider>().material = Resources.Load<PhysicMaterial>("Materials/Rigid_Material");

            shatteredCubes.Add(cube);

            StartCoroutine(ShrinkAndDestroy(cube));
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
        float shrinkDuration = 2f; // Duration in seconds before cube disappears
        float elapsedTime = 0;

        Vector3 originalScale = cube.transform.localScale;

        while (elapsedTime < shrinkDuration) {
            cube.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, (elapsedTime / shrinkDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shatteredCubes.Remove(cube);
        Destroy(cube);
    }

    public void ResetMeshAppearance() {
        foreach (var cube in shatteredCubes) {
            Destroy(cube);
        }
        shatteredCubes.Clear();
    }
}