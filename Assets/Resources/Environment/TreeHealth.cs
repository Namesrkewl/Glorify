using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeCutting;



public class TreeHealth : NetworkBehaviour, ITreeDamageable
{
    //public int treetotreeCollisionMinDamage = 3;
    //public int treetotreeCollisionMaxDamage = 15;
    //int treetotreeCollisionDamageBase;

    public ResourceNode resourceNode;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable))
        {
            {
                if (collision.relativeVelocity.magnitude > 1f)
                {
                    Debug.Log("Tree collision magnitude: " + collision.relativeVelocity.magnitude);
                    int damageAmount = Mathf.RoundToInt(collision.relativeVelocity.magnitude);

                    DamagePopup.Create(collision.GetContact(0).point, damageAmount, damageAmount > 20);
                    treeDamageable.Damage(damageAmount);

                    Damage(damageAmount);
                }
            }
        }
    }

    public void SplitTree(ResourceNode resourceNode, GameObject treeObject)
    {
        Debug.Log("Tree part name: " + treeObject.name);

        treeObject.SetActive(false);
    }

    /*
    // Cut the damn thing
    public void SplitTree(ResourceNode resourceNode, GameObject treePart, TreeStatus treeStatus, bool hasStump)
    {
        Debug.Log("Tree part name: " + treePart.name);

        switch (treeStatus)
        {

            case TreeStatus.WholeTree:
                treePart.SetActive(false);
                treePart.transform.parent.Find("Fir Tree Divided").gameObject.SetActive(true);
                break;
            case TreeStatus.TopAndBottomTree:
                Destroy(treePart.transform.GetComponent<Rigidbody>());
                treePart.transform.GetComponent<MeshCollider>().enabled = false;
                treePart.transform.GetComponent<MeshRenderer>().enabled = false;

                GameObject lowerHalfTree = treePart.transform.Find("Lower Half").gameObject;
                lowerHalfTree.SetActive(true);

                GameObject upperHalfTree = treePart.transform.Find("Top Half").gameObject;
                upperHalfTree.SetActive(true);

                break;
            case TreeStatus.BottomTree:
                // Find game object with tag of ParentTree
                // Split log in half
                Debug.Log("Tree name: " + treePart.name);
                Debug.Log("BottomTree Status? How did you get here?");
                break;
            case TreeStatus.TopTree:
                // Find game object with tag of ParentTree
                // Split log in half
                Debug.Log("Tree name: " + treePart.name);
                Debug.Log("TopTree Status? How did you get here?");
                break;
            case TreeStatus.Stump:
                // Find game object with tag of ParentTree
                // Split log in half
                Debug.Log("Tree name: " + treePart.name);
                Debug.Log("Stump status? How did you get here?");
                break;
            default:
                Debug.Log("No tree status selected. How did you get here?");
                break;
        }
    }
    */

    public bool IsTreeFullyHarvested(GameObject objectToAction)
    {
        Transform rootObjectToAction = objectToAction.transform.root;

        // #################### REMOVE REDUNDENCY IN CHECKS ####################

        /*
        // print status of everything in the following if statement
        if (rootObjectToAction.Find("Fir Tree Divided/Stump").gameObject.activeSelf == true)
        {
            return false;
        }
        else if (rootObjectToAction.Find("Fir Tree Divided/Top and Bottom Half/Top Half").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Tree Divided/Top and Bottom Half/Lower Half").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Tree Whole").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Tree Divided/Top and Bottom Half").gameObject.activeSelf == true)
        {
            return false;
        }
        else
        {
            Debug.Log("Tree is fully harvested.");
            return true;
        }
        */

        return true;
    }

    public void AffectTree(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, int damage, bool criticalHit = false, Vector3? hitPoint = null)
    {
        //fullyHarvest = IsTreeFullyHarvested(objectToAction);
        //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);


        Debug.Log("Tree damage: " + damage);
        Damage(damage);
        if (resourceNode.health.Value <= 0)
        {
            resourceNode.Harvest(localConnection);
        }
    }

    /*
    //public void AffectTree(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, Inventory playerInventory, int damage, bool criticalHit = false, Vector3? hitPoint = null)
    public void AffectTree(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, int damage, bool criticalHit = false, Vector3? hitPoint = null)
    {
        Damage(damage);
        if (hitPoint != null)
        {
            Debug.Log("Hit location not null");
            Debug.Log("hintPoint val = " + hitPoint.Value);
            DamagePopup.Create(hitPoint.Value, damage, criticalHit);
        }
        else
        {
            Debug.Log("Hit location is null. Using objects position.");
            DamagePopup.Create(objectToAction.transform.position, damage, criticalHit);
        }


        if (resourceNode.health.Value <= 0)
        {
            bool fullyHarvest = false;
            switch (objectToAction.tag)
            {
                case "WholeTree":
                    // Look if tree has a stump
                    //if (objectToAction.transform.Find("Stump") != null)
                    //{
                    SplitTree(resourceNode, objectToAction, TreeStatus.WholeTree, true);
                    //}
                    break;
                case "Stump":
                    objectToAction.SetActive(false);
                    fullyHarvest = IsTreeFullyHarvested(objectToAction);
                    //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);
                    resourceNode.Harvest(localConnection, fullyHarvest);
                    break;
                case "TopAndBottomTree":
                    SplitTree(resourceNode, objectToAction, TreeStatus.TopAndBottomTree, true);
                    break;
                case "TopTree":
                    objectToAction.SetActive(false);
                    // Get bottom tree
                    GameObject parentTree = objectToAction.transform.parent.gameObject;
                    GameObject bottomTree = parentTree.transform.Find("Lower Half").gameObject;

                    // check if bottomTree is active. If it is deactivated, the deactivate the parent game object as well
                    if (bottomTree.activeSelf == false)
                    {
                        parentTree.SetActive(false);
                    }
                    fullyHarvest = IsTreeFullyHarvested(objectToAction);
                    //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);
                    resourceNode.Harvest(localConnection, fullyHarvest);
                    break;
                case "BottomTree":
                    objectToAction.SetActive(false);
                    // Get bottom tree
                    GameObject parentTree2 = objectToAction.transform.parent.gameObject;
                    GameObject topTree = parentTree2.transform.Find("Top Half").gameObject;
                    // check if bottomTree is active. If it is deactivated, the deactivate the parent game object as well
                    if (topTree.activeSelf == false)
                    {
                        parentTree2.SetActive(false);
                    }
                    fullyHarvest = IsTreeFullyHarvested(objectToAction);
                    //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);
                    resourceNode.Harvest(localConnection, fullyHarvest);
                    break;
                default:
                    Debug.Log("How did you get here?");
                    break;
            }
        }
        else if (resourceNode.health.Value > 0)
        {

        }
        //}
    }
    */
    public void Damage(int amount)
    {
        resourceNode.health.Value -= amount;
    }
}