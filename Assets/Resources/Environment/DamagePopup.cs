using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class DamagePopup : MonoBehaviour
{
    // Create a Damage Popup
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        Debug.Log("Create Position equals " + position);
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        Spawner.instance.SpawnObject(damagePopupTransform.gameObject);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);
        // damagePopup.transform.LookAt(-Camera.main.transform.position);

        return damagePopup;
    }

    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    //private Vector3 moveVector;
    public float moveYSpeed = 1f;
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit)
        {
            // Normal hit
            textMesh.fontSize = 12;
            ColorUtility.TryParseHtmlString("#FF5D00", out textColor);
        }
        else
        {
            // Critical hit
            textMesh.fontSize = 20;
            ColorUtility.TryParseHtmlString("#FF0A00", out textColor);
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        //moveVector = new Vector3(1, 1) * 1f;

    }

    private void Update()
    {
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        //moveYSpeed -= moveYSpeed * 1f * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        /*
        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5F)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += new Vector3(1, 1, 1) * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= new Vector3(1, 1, 1) * decreaseScaleAmount * Time.deltaTime;
        }
        */
        if (disappearTimer < 0)
        //Start disappearing
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }

        UpdateDamagePopup(PlayerManager.instance.localPlayer);
    }

    public void UpdateDamagePopup(NetworkPlayerController localPlayer)
    {
        if (localPlayer != null)
        {
            //transform.LookAt(localPlayer.localCamera.transform);
            transform.rotation = Quaternion.LookRotation(transform.position - localPlayer.localCamera.transform.position);
        }
    }
}
