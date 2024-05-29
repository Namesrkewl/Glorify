using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OreCutting;



public class OreHealth : NetworkBehaviour, IOreDamageable
{
    //public int oretooreCollisionMinDamage = 3;
    //public int oretooreCollisionMaxDamage = 15;
    //int oretooreCollisionDamageBase;

    public ResourceNode resourceNode;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IOreDamageable>(out IOreDamageable oreDamageable))
        {
            {
                if (collision.relativeVelocity.magnitude > 1f)
                {
                    Debug.Log("Ore collision magnitude: " + collision.relativeVelocity.magnitude);
                    int damageAmount = Mathf.RoundToInt(collision.relativeVelocity.magnitude);

                    DamagePopup.Create(collision.GetContact(0).point, damageAmount, damageAmount > 20);
                    oreDamageable.Damage(damageAmount);

                    Damage(damageAmount);
                }
            }
        }
    }

    public void SplitOre(ResourceNode resourceNode, GameObject oreObject)
    {
        Debug.Log("Ore part name: " + oreObject.name);

        oreObject.SetActive(false);
    }

    /*
    // Cut the damn thing
    public void SplitOre(ResourceNode resourceNode, GameObject orePart, OreStatus oreStatus, bool hasStump)
    {
        Debug.Log("Ore part name: " + orePart.name);

        switch (oreStatus)
        {

            case OreStatus.WholeOre:
                orePart.SetActive(false);
                orePart.transform.parent.Find("Fir Ore Divided").gameObject.SetActive(true);
                break;
            case OreStatus.TopAndBottomOre:
                Destroy(orePart.transform.GetComponent<Rigidbody>());
                orePart.transform.GetComponent<MeshCollider>().enabled = false;
                orePart.transform.GetComponent<MeshRenderer>().enabled = false;

                GameObject lowerHalfOre = orePart.transform.Find("Lower Half").gameObject;
                lowerHalfOre.SetActive(true);

                GameObject upperHalfOre = orePart.transform.Find("Top Half").gameObject;
                upperHalfOre.SetActive(true);

                break;
            case OreStatus.BottomOre:
                // Find game object with tag of ParentOre
                // Split log in half
                Debug.Log("Ore name: " + orePart.name);
                Debug.Log("BottomOre Status? How did you get here?");
                break;
            case OreStatus.TopOre:
                // Find game object with tag of ParentOre
                // Split log in half
                Debug.Log("Ore name: " + orePart.name);
                Debug.Log("TopOre Status? How did you get here?");
                break;
            case OreStatus.Stump:
                // Find game object with tag of ParentOre
                // Split log in half
                Debug.Log("Ore name: " + orePart.name);
                Debug.Log("Stump status? How did you get here?");
                break;
            default:
                Debug.Log("No ore status selected. How did you get here?");
                break;
        }
    }
    */

    public bool IsOreFullyHarvested(GameObject objectToAction)
    {
        Transform rootObjectToAction = objectToAction.transform.root;

        // #################### REMOVE REDUNDENCY IN CHECKS ####################

        /*
        // print status of everything in the following if statement
        if (rootObjectToAction.Find("Fir Ore Divided/Stump").gameObject.activeSelf == true)
        {
            return false;
        }
        else if (rootObjectToAction.Find("Fir Ore Divided/Top and Bottom Half/Top Half").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Ore Divided/Top and Bottom Half/Lower Half").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Ore Whole").gameObject.activeSelf == true)
        {
            return false;
        }

        else if (rootObjectToAction.Find("Fir Ore Divided/Top and Bottom Half").gameObject.activeSelf == true)
        {
            return false;
        }
        else
        {
            Debug.Log("Ore is fully harvested.");
            return true;
        }
        */

        return true;
    }

    public void AffectOre(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, int damage, bool criticalHit = false, Vector3? hitPoint = null)
    {
        //fullyHarvest = IsOreFullyHarvested(objectToAction);
        //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);


        Debug.Log("Ore damage: " + damage);
        Damage(damage);
        if (resourceNode.health.Value <= 0)
        {
            resourceNode.Harvest(localConnection);
        }
    }

    /*
    //public void AffectOre(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, Inventory playerInventory, int damage, bool criticalHit = false, Vector3? hitPoint = null)
    public void AffectOre(ResourceNode resourceNode, GameObject objectToAction, FishNet.Connection.NetworkConnection localConnection, int damage, bool criticalHit = false, Vector3? hitPoint = null)
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
                case "WholeOre":
                    // Look if ore has a stump
                    //if (objectToAction.transform.Find("Stump") != null)
                    //{
                    SplitOre(resourceNode, objectToAction, OreStatus.WholeOre, true);
                    //}
                    break;
                case "Stump":
                    objectToAction.SetActive(false);
                    fullyHarvest = IsOreFullyHarvested(objectToAction);
                    //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);
                    resourceNode.Harvest(localConnection, fullyHarvest);
                    break;
                case "TopAndBottomOre":
                    SplitOre(resourceNode, objectToAction, OreStatus.TopAndBottomOre, true);
                    break;
                case "TopOre":
                    objectToAction.SetActive(false);
                    // Get bottom ore
                    GameObject parentOre = objectToAction.transform.parent.gameObject;
                    GameObject bottomOre = parentOre.transform.Find("Lower Half").gameObject;

                    // check if bottomOre is active. If it is deactivated, the deactivate the parent game object as well
                    if (bottomOre.activeSelf == false)
                    {
                        parentOre.SetActive(false);
                    }
                    fullyHarvest = IsOreFullyHarvested(objectToAction);
                    //resourceNode.Harvest(localConnection, playerInventory, fullyHarvest);
                    resourceNode.Harvest(localConnection, fullyHarvest);
                    break;
                case "BottomOre":
                    objectToAction.SetActive(false);
                    // Get bottom ore
                    GameObject parentOre2 = objectToAction.transform.parent.gameObject;
                    GameObject topOre = parentOre2.transform.Find("Top Half").gameObject;
                    // check if bottomOre is active. If it is deactivated, the deactivate the parent game object as well
                    if (topOre.activeSelf == false)
                    {
                        parentOre2.SetActive(false);
                    }
                    fullyHarvest = IsOreFullyHarvested(objectToAction);
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