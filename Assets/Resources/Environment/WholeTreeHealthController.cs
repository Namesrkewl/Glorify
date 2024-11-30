using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeTreeHealthController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void WholeTree(GameObject treePart, ResourceNode resourceNode)
    {
        // Find game object with tag of Stump
        GameObject stump = treePart.transform.Find("Stump").gameObject;

        resourceNode._myCollection.Add(stump);

        stump.transform.parent = null;
        Rigidbody stumpRB = stump.AddComponent<Rigidbody>();
        stumpRB.isKinematic = true;

        // Get rigidbody of tree
        Rigidbody treeRB = treePart.GetComponent<Rigidbody>();
        treeRB.isKinematic = false;

        float randomPositionXAxis = Random.Range(-0.2f, 0.2f);
        float randomPositionYAxis = Random.Range(-0.2f, 0.2f);
        treePart.transform.position += new Vector3(randomPositionXAxis, randomPositionYAxis, 0);

        // Generate random small tree rotation
        float randomRotationXAxis = Random.Range(-1.2f, 1.2f);
        float randomRotationYAxis = Random.Range(-1.2f, 1.2f);
        float randomRotationZAxis = Random.Range(-2f, 2f);

        treePart.transform.Rotate(randomRotationXAxis, randomRotationYAxis, randomRotationZAxis);
        treePart.transform.Find("Leaves").gameObject.SetActive(false);
    }
}
