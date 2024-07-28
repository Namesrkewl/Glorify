using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class TreeCuttingEvent : MonoBehaviour
{
    public static TreeCuttingEvent instance;
    private bool isCutting = false;
    public int cuttingCounter = 1;
    public int maxScore;
    public int passingScore;
    public int currentScore = 0;

    [SerializeField] private GameObject actionSliderPrefab;
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
        cuttingCounter = 1;
        // Instantiate actionCirclesPrefab GameObject
        GameObject actionSlider = Instantiate(actionSliderPrefab);
        //actionSlider.GetComponent<CustomSlider>().enabled = true;

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
            //Debug.Log("Inside Cutting() function. Circles count = " + circles);
            if (cuttingCounter <= 0)
            {
                Debug.Log("Cutting counter is less than or equal to 0.");
                isCutting = false;
                CompleteCutting(treeHealth, resourceNode, objectToAction, HitInfo, localConnection);
                //yield return null;
            }
            yield return null;
        }
        //Debug.Log("Cutting Coroutine ending.");
        //this.gameObject.SetActive(false);
        CompleteCutting(treeHealth, resourceNode, objectToAction, HitInfo, localConnection);
        yield return null;
    }

    private void CompleteCutting(TreeHealth treeHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        if (currentScore >= passingScore + 2)
        {
            Debug.Log("Cutting complete. Score of Excellent.");
            cuttingScore = TreeCuttingScore.Excellent;
            currentScore = 0;
            TreeCutting.DamageTree(treeHealth, resourceNode, objectToAction, HitInfo, cuttingScore, localConnection);
            return;
        }
        else if (currentScore >= passingScore)
        {
            Debug.Log("Cutting complete. Score of good.");
            cuttingScore = TreeCuttingScore.Good;
            currentScore = 0;
            TreeCutting.DamageTree(treeHealth, resourceNode, objectToAction, HitInfo, cuttingScore, localConnection);
            return;
        }
        else
        {
            Debug.Log("Cutting failed.");
            currentScore = 0;
            cuttingScore = TreeCuttingScore.Failed;
            TreeCutting.DamageTree(treeHealth, resourceNode, objectToAction, HitInfo, cuttingScore, localConnection);
        }
    }

    void Start()
    {
        //StartCoroutine(Cutting());
    }
}
