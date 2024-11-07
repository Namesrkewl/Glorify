using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TreeCutting : NetworkBehaviour, ITreeDamageable
{
    [SerializeField] private Camera PlayerCamera;
    public float actionRange = 200f;
    public LayerMask actionMask;
    //public Inventory playerInventory;

    [SerializeField] private GameObject fxTreeHit;
    [SerializeField] private GameObject fxTreeHitBlocks;


    public int tempTreeMaxDamage = 30;
    public int tempTreeMinDamage = 10;

    public float criticalHitChance = 30;
    public float criticalHitMultiplier = 1.20f;

    public GameObject player;

    /*
    public enum TreeStatus
    {
        WholeTree,
        TopTree,
        BottomTree,
        TopAndBottomTree,
        Stump
    }
    */

    // Start is called before the first frame update
    void Start()
    {
    }

    //https://www.youtube.com/watch?v=ql4prUAasEg&t=516s
    /*
    private void AnimationEvent_OnHit()
    {
        Vector3 colliderSize = Vector3.one * .3f;
        Collider[] colliderArray = Physics.OverlapBox(PlayerCamera.transform.position + PlayerCamera.transform.forward * 1.5f, colliderSize);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable))
            {
                // Make damageAmount equal to the damage of the axe
                //int damageAmount = UnityEngine.Random.Range(10, 30);
                //DamagePopup.Create(hitArea.transform.position, damageAmount, damageAmount > 14);

                // Damage Tree
                //treeDamageable.Damage(damageAmount);
                StartCuttingEvent(treeHealth, resourceNode, objectToAction, HitInfo);

                // Shake Camera
                //treeShake.GenerateImpulse();

                // Spawn FX
                //Instantiate(fxTreeHit, hitArea.transform.position, Quaternion.identity);
                //Instantiate(fxTreeHitBlocks, hitArea.transform.position, Quaternion.identity);
            }
        }
    }
    */

    public void StartCuttingEvent(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, GameObject player)
    {
        //int damageAmount = 100;

        GameObject cuttingEvent = GameObject.Find("TreeCuttingEvent");
        cuttingEvent.GetComponent<TreeCuttingEvent>().enabled = true;
        //miningEvent.GetComponent<MiningEvent>().circles = 1;

        // Return CompleteMining() value of MiningEvent
        Debug.Log("Start Event Tree Cutting");
        cuttingEvent.GetComponent<TreeCuttingEvent>().StartCuttingEvent(treeHealth, resourceNode, objectToAction, HitInfo, this.LocalConnection, player);

        // Start cutting tree
    }

    public static void DamageTree(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, TreeCuttingEvent.TreeCuttingScore treeCuttingScore, FishNet.Connection.NetworkConnection localConnection, GameObject player)
    {
        if (treeCuttingScore == TreeCuttingEvent.TreeCuttingScore.Good || treeCuttingScore == TreeCuttingEvent.TreeCuttingScore.Excellent)
        {
            Debug.LogError("TreeCuttingScore is Good or Excellent");
            bool isCriticalHit = false;

            int damage = 100;

            /*** FOR ASSIGNING CRITICAL HIT - WORKS BUT REMOVED
            if (isCriticalHit == true)
            {
                damage = Random.Range(tempOreMaxDamage + 1, Mathf.RoundToInt((float)(tempOreMaxDamage * criticalHitMultiplier)));
            }
            else
            {
                damage = Random.Range(tempOreMinDamage, tempOreMaxDamage);
            }
            */

            //Debug.Log("Hit was good, doing damage of " + damage + " and critical bool = " + isCriticalHit);
            //oreHealth.AffectOre(resourceNode, objectToAction, this.LocalConnection, playerInventory, damage, isCriticalHit, HitInfo.point);
            treeHealth.AffectTree(resourceNode, objectToAction, localConnection, damage, player, isCriticalHit, HitInfo.point);
        }
        else
        {
            int damage = 0;
            treeHealth.AffectTree(resourceNode, objectToAction, localConnection, damage, player, false, HitInfo.point);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            //PlayerCamera = Camera.main;

            //Debug.Log("Owner");
        }
        else
        {
            //Debug.Log("Not Owner");
        }
    }

    /*
       private void Update()
       {
           if (Input.GetMouseButtonDown(0))
           {
               //if (Animator != null) Animator.SetTrigger("Attack");
               //FunctionTimer.Create(AnimationEvent_OnHit, 0.5f);

               //Ray CameraRay = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

               Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
               RaycastHit hit;
               Debug.Log("apple");
               Debug.DrawRay(PlayerCamera.transform.position, PlayerCamera.ScreenToWorldPoint(Input.mousePosition), Color.green);
               Debug.Log("banana");

               if (Physics.Raycast(ray, out RaycastHit HitInfo, 200f))
               {
                   Debug.Log("Kiwi");
                   Debug.Log("HitInfo.collider.gameObject: " + HitInfo.collider.gameObject);
                   if (HitInfo.collider.gameObject.GetComponent<TreeHealth>())
                   {



                       Debug.Log("Peach");
                       GameObject localPlayer = PlayerBehaviour.instance.gameObject;

                       Vector3 directionToTarget = localPlayer.transform.position - HitInfo.transform.position;
                       float angle = Vector3.Angle(localPlayer.transform.forward, directionToTarget);

                       // Print angle
                       Debug.Log("angle: " + angle);

                       if (angle > 90)
                       {
                           Debug.Log("angle is greater than 90");
                           // Cut the tree here
                       }


                       Debug.Log("Orange");
                       Debug.Log("HitInfo.transform.gameObject: " + HitInfo.transform.gameObject);
                       GameObject objectToAction = HitInfo.transform.gameObject;

                       if (objectToAction.TryGetComponent(out ResourceNode resourceNode))
                       {
                           if (objectToAction.TryGetComponent(out TreeHealth treeHealth))
                           {
                               /***
                               bool isCriticalHit = Random.Range(0, 100) < criticalHitChance;

                               int damage;
                               if (isCriticalHit == true)
                               {
                                   damage = Random.Range(tempTreeMaxDamage + 1, Mathf.RoundToInt((float)(tempTreeMaxDamage * criticalHitMultiplier)));
                               }
                               else
                               {
                                   damage = Random.Range(tempTreeMinDamage, tempTreeMaxDamage);
                               }
                               */
    //treeHealth.AffectTree(resourceNode, objectToAction, this.LocalConnection, playerInventory, damage, isCriticalHit, HitInfo.point);
    //treeHealth.AffectTree(resourceNode, objectToAction, this.LocalConnection, damage, isCriticalHit, HitInfo.point);

    // How can I ensure treeHealth, resourceNode, objectToAction, and HitInfo objects are null before using it in StartCuttingEvent?
    /*

                                if (treeHealth == null)
                                {
                                    Debug.LogError("treeHealth is null");
                                }
                                if (resourceNode == null)
                                {
                                    Debug.LogError("resourceNode is null");
                                }
                                if (objectToAction == null)
                                {
                                    Debug.LogError("objectToAction is null");
                                }
                                // 
                                if (HitInfo.collider == null)
                                {
                                    Debug.LogError("HitInfo is null");
                                }






                                StartCuttingEvent(treeHealth, resourceNode, objectToAction, HitInfo);

                                // Check if the variables in the previous line are initialized

                            }
                        }


                        /* Delete me?
                        else if (objectToAction.transform.parent.TryGetComponent(out ResourceNode resourceNodeParent))
                        {
                            AffectTree(resourceNodeParent, objectToAction.transform.parent.gameObject);
                            Debug.Log("Apple 2");
                        }
                        else if (objectToAction.transform.parent.parent.TryGetComponent(out ResourceNode resourceNodeParentParent))
                        {
                            AffectTree(resourceNodeParentParent, objectToAction.transform.parent.parent.gameObject);
                            Debug.Log("Apple 3");
                        }
                        */
    /*
                    }
                }
            }
        }
    */


    public void Damage(int amount)
    {
        // *** TODO ***
        //Remove health from player
    }
}
