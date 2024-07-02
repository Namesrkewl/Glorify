using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class MiningEvent : MonoBehaviour
{
    public static MiningEvent instance;
    private bool isMining = false;
    public int circles = 1;
    public int maxScore;
    public int passingScore;
    public int currentScore;

    [SerializeField] private GameObject actionCirclesPrefab;
    public enum MiningScore
    {
        Perfect,
        Excellent,
        Good,
        Poor,
        Bad,
        Failed
    }
    public MiningScore miningScore;

    private void Update()
    {
    }

    public void StartMiningEvent(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        Debug.Log("In StartMiningEvent");
        // Instantiate actionCriclesPrefab GameObject
        GameObject actionCircles = Instantiate(actionCirclesPrefab);
        actionCircles.GetComponent<PopupCircles>().enabled = true;

        isMining = true;
        StartCoroutine(Mining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection));
        Debug.Log("Mining Coroutine stopped.");
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

    public IEnumerator Mining(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        while (isMining)
        {
            Debug.Log("Inside Mining() function. Circles count = " + circles);
            if (circles <= 0)
            {
                Debug.Log("Circles is less than or equal to 0.");
                isMining = false;
                CompleteMining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection);
                //yield return null;
            }
            yield return null;
        }
        Debug.Log("Mining Coroutine ending.");
        this.gameObject.SetActive(false);
        CompleteMining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection);
        yield return null;
    }

    private void CompleteMining(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection)
    {
        if (currentScore >= passingScore)
        {
            Debug.Log("Mining complete. Score of Excellent=1.");
            miningScore = MiningScore.Excellent;
            OreCutting.DamageOre(oreHealth, resourceNode, objectToAction, HitInfo, miningScore, localConnection);
            return;
        }

        Debug.Log("Mining failed.");
        miningScore = MiningScore.Failed;
    }

    void Start()
    {
        //StartCoroutine(Mining());
    }
}
