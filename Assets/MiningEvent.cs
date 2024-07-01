using System.Collections;
using UnityEngine;

public class MiningEvent : MonoBehaviour
{
    public static MiningEvent instance;
    private bool isMining = false;
    private int circles = 1;
    public int maxScore;
    public int passingScore;
    public int currentScore;

    private void Update()
    {
        Debug.Log(currentScore);
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

    public IEnumerator Mining()
    {
        while (isMining)
        {
            if (circles <= 0)
            {
                isMining = false;
                CompleteMining();

            }
        }
        return null;
    }

    private void CompleteMining()
    {
        if (currentScore >= passingScore)
        {
            // Send to original resource


        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Mining());
    }

}
