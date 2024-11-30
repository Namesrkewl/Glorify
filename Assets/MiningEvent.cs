using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using static TreeCuttingEvent;

public class MiningEvent : MonoBehaviour
{
    public static MiningEvent instance;
    private bool isMining = false;
    public int circles = 1;
    public int maxScore;
    public int passingScore;
    public int currentScore = 0;

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

    public void StartMiningEvent(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection, GameObject player)
    {
        Debug.Log("In StartMiningEvent");
        circles = 1;
        // Instantiate actionCriclesPrefab GameObject
        GameObject actionCircles = Instantiate(actionCirclesPrefab);
        actionCircles.GetComponent<PopupCircles>().enabled = true;

        isMining = true;
        StartCoroutine(Mining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection, player));
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

    public IEnumerator Mining(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection, GameObject player)
    {
        while (isMining)
        {
            Debug.Log("Inside Mining() function. Circles count = " + circles);
            if (circles <= 0)
            {
                Debug.LogError("Circles is less than or equal to 0.");
                isMining = false;
                CompleteMining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection, player);
                //yield return null;
            }
            yield return null;
        }
        Debug.Log("Inside Mining() AFTERRRRRRRRRRRRRR function. Circles count = " + circles);
        //Debug.Log("Mining Coroutine ending.");
        //this.gameObject.SetActive(false);
        //CompleteMining(oreHealth, resourceNode, objectToAction, HitInfo, localConnection);
        yield return null;
    }

    private void CompleteMining(OreHealth oreHealth, ResourceNode resourceNode, GameObject objectToAction, RaycastHit HitInfo, FishNet.Connection.NetworkConnection localConnection, GameObject player)
    {
        Debug.LogError("Current score = " + currentScore);
        Debug.LogError("Circle count= " + circles);
        if (currentScore >= passingScore + 2)
        {
            Debug.LogError("Mining complete. Score of Excellent.");
            miningScore = MiningScore.Excellent;
            currentScore = 0;
            OreCutting.DamageOre(oreHealth, resourceNode, objectToAction, HitInfo, miningScore, localConnection, player);
            return;
        }
        else if (currentScore >= passingScore)
        {
            Debug.LogError("Mining complete. Score of Good.");
            miningScore = MiningScore.Excellent;
            currentScore = 0;
            OreCutting.DamageOre(oreHealth, resourceNode, objectToAction, HitInfo, miningScore, localConnection, player);
            return;
        }
        else
        {
            Debug.LogError("Mining failed.");
            currentScore = 0;
            miningScore = MiningScore.Failed;
            OreCutting.DamageOre(oreHealth, resourceNode, objectToAction, HitInfo, miningScore, localConnection, player);
        }
    }

    void Start()
    {
        //StartCoroutine(Mining());
    }
}
