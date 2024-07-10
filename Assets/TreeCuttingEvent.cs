using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class TreeCuttingEvent : MonoBehaviour
{
    public static TreeCuttingEvent instance;
    private bool isCutting = false;
    public int circles = 1;
    public int maxScore;
    public int passingScore;
    public int currentScore;

    [SerializeField] private GameObject actionCirclesPrefab;
    public enum TreeCuttingScore
    {
        Perfect,
        Excellent,
        Good,
        Poor,
        Bad,
        Failed
    }
    public TreeCuttingScore cuttingScore;

    private void Update()
    {
    }

    public void StartCuttingEvent(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        Debug.Log("In StartCuttingEvent");
        // Instantiate actionCirclesPrefab GameObject
        GameObject actionCircles = Instantiate(actionCirclesPrefab);
        actionCircles.GetComponent<PopupCircles>().enabled = true;

        isCutting = true;
        StartCoroutine(Cutting(treeHealth, resourceNode, objectToAction, HitInfo, localConnection));
        Debug.Log("Cutting Coroutine stopped.");
        //this.gameObject.SetActive(false);
    }

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

    public IEnumerator Cutting(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        while (isCutting)
        {
            Debug.Log("Inside Mining() function. Circles count = " + circles);
            if (circles <= 0)
            {
                Debug.Log("Circles is less than or equal to 0.");
                isCutting = false;
                CompleteCutting(treeHealth, resourceNode, objectToAction, HitInfo, localConnection);
                //yield return null;
            }
            yield return null;
        }
        Debug.Log("Mining Coroutine ending.");
        this.gameObject.SetActive(false);
        CompleteCutting(treeHealth, resourceNode, objectToAction, HitInfo, localConnection);
        yield return null;
    }

    private void CompleteCutting(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        if (currentScore >= passingScore)
        {
            Debug.Log("Mining complete. Score of Excellent=1.");
            cuttingScore = TreeCuttingScore.Excellent;
            TreeCutting.DamageTree(treeHealth, resourceNode, objectToAction, HitInfo, cuttingScore, localConnection);
            return;
        }

        Debug.Log("Mining failed.");
        cuttingScore = TreeCuttingScore.Failed;
    }

    void Start()
    {
        //StartCoroutine(Mining());
    }
}
