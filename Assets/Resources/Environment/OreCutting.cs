using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OreCutting : NetworkBehaviour, IOreDamageable
{
    [SerializeField] private Camera PlayerCamera;
    public float actionRange = 200f;
    public LayerMask actionMask;
    //public Inventory playerInventory;

    [SerializeField] private GameObject fxOreHit;
    [SerializeField] private GameObject fxOreHitBlocks;


    public int tempOreMaxDamage = 30;
    public int tempOreMinDamage = 10;

    public float criticalHitChance = 30;
    public float criticalHitMultiplier = 1.20f;
    public enum OreStatus
    {
        WhoreOre,
        ThreeQuarterOre,
        HalfOre,
        QuarterOre,
        EmptyOre
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    //https://www.youtube.com/watch?v=ql4prUAasEg&t=516s
    private void AnimationEvent_OnHit()
    {
        Vector3 colliderSize = Vector3.one * .3f;
        Collider[] colliderArray = Physics.OverlapBox(PlayerCamera.transform.position + PlayerCamera.transform.forward * 1.5f, colliderSize);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<IOreDamageable>(out IOreDamageable oreDamageable))
            {
                // Make damageAmount equal to the damage of the axe
                int damageAmount = UnityEngine.Random.Range(10, 30);
                //DamagePopup.Create(hitArea.transform.position, damageAmount, damageAmount > 14);

                // Damage Ore
                oreDamageable.Damage(damageAmount);

                // Shake Camera
                //OreShake.GenerateImpulse();

                // Spawn FX
                //Instantiate(fxOreHit, hitArea.transform.position, Quaternion.identity);
                //Instantiate(fxOreHitBlocks, hitArea.transform.position, Quaternion.identity);
            }
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
                if (HitInfo.collider.gameObject.GetComponent<OreHealth>())
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
                        // Cut the Ore here
                    }


                    Debug.Log("Orange");
                    Debug.Log("HitInfo.transform.gameObject: " + HitInfo.transform.gameObject);
                    GameObject objectToAction = HitInfo.transform.gameObject;

                    if (objectToAction.TryGetComponent(out ResourceNode resourceNode))
                    {
                        if (objectToAction.TryGetComponent(out OreHealth oreHealth))
                        {
                            bool isCriticalHit = Random.Range(0, 100) < criticalHitChance;

                            int damage;
                            if (isCriticalHit == true)
                            {
                                damage = Random.Range(tempOreMaxDamage + 1, Mathf.RoundToInt((float)(tempOreMaxDamage * criticalHitMultiplier)));
                            }
                            else
                            {
                                damage = Random.Range(tempOreMinDamage, tempOreMaxDamage);
                            }
                            //oreHealth.AffectOre(resourceNode, objectToAction, this.LocalConnection, playerInventory, damage, isCriticalHit, HitInfo.point);
                            oreHealth.AffectOre(resourceNode, objectToAction, this.LocalConnection, damage, isCriticalHit, HitInfo.point);

                            // Check if the variables in the previous line are initialized

                        }
                    }


                    /* Delete me?
                    else if (objectToAction.transform.parent.TryGetComponent(out ResourceNode resourceNodeParent))
                    {
                        AffectOre(resourceNodeParent, objectToAction.transform.parent.gameObject);
                        Debug.Log("Apple 2");
                    }
                    else if (objectToAction.transform.parent.parent.TryGetComponent(out ResourceNode resourceNodeParentParent))
                    {
                        AffectOre(resourceNodeParentParent, objectToAction.transform.parent.parent.gameObject);
                        Debug.Log("Apple 3");
                    }
                    */
                }
            }
        }
    }

    public void Damage(int amount)
    {
        // *** TODO ***
        //Remove health from player
    }
}
