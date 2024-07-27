using FishNet.Example.ColliderRollbacks;
using UnityEngine;

public class ActionEventSystem : MonoBehaviour
{
    //public static ActionEventSystem instance;
    [SerializeField] private Camera PlayerCamera;
    public float actionRange = 200f;
    public LayerMask actionMask;

    // Attach network scripts from player to this script
    //[TextArea]
    [Header("Player harvesting scripts")]
    public TreeCutting treeCutting;
    public OreCutting oreCutting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionMask = LayerMask.GetMask("HarvestObject");
    }
    /*
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
    */
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if (Animator != null) Animator.SetTrigger("Attack");
            //FunctionTimer.Create(AnimationEvent_OnHit, 0.5f);

            //Ray CameraRay = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
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
                        // Check if the object is a tree
                        if (objectToAction.TryGetComponent(out TreeHealth treeHealth))
                        {
                            // Not sure if the following "if" statements work.
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
                            if (treeCutting == null)
                            {
                                Debug.LogError("treeCutting is null");
                            }

                            // Print all values
                            Debug.Log("treeHealth: " + treeHealth);
                            Debug.Log("resourceNode: " + resourceNode);
                            Debug.Log("objectToAction: " + objectToAction);
                            Debug.Log("HitInfo: " + HitInfo);


                            treeCutting.StartCuttingEvent(treeHealth, resourceNode, objectToAction, HitInfo);

                        }

                        // Check if object is an ore
                        else if (objectToAction.TryGetComponent(out OreHealth oreHealth))
                        {
                            // Not sure if the following "if" statements work.
                            if (oreHealth == null)
                            {
                                Debug.LogError("oreHealth is null");
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

                            oreCutting.StartMiningEvent(oreHealth, resourceNode, objectToAction, HitInfo);
                        }
                    }
                }
            }
        }
    }
}
