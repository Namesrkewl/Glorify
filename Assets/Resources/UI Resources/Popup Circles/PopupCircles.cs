using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.PlayerLoop.PreUpdate;
using System.Collections;

public class PopupCircles : MonoBehaviour
{
    //Root Visual Element
    private VisualElement m_Root;

    // Circle that usually don't change
    private VisualElement m_BaseCircle;

    // Circle that shrinks or grows
    private VisualElement m_ShrinkGrowCircle;

    bool GrowBoolean = false;

    private bool canUpdate = false;

    IEnumerator EnableUpdateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canUpdate = true;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EnableUpdateAfterDelay(2f));

        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_Root.style.flexGrow = 0f;
        m_Root.style.flexShrink = 0f;
        m_Root.pickingMode = PickingMode.Ignore;

        m_Root.style.flexWrap = Wrap.NoWrap;
        /* Removed from Slider
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        */
        Debug.Log("Start Popup Circles");
        Debug.Log(m_Root);
        AddElements();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canUpdate) return;

        // Check if m_ShrinkGrowCircle.style.width is greater than or equal to m_ShrinkGrowCircle.style.maxWidth. If it is, change GrowBoolean to false and call ShrinkGrowCircle
        if (m_ShrinkGrowCircle.resolvedStyle.width >= m_ShrinkGrowCircle.resolvedStyle.maxWidth.value)
        {
            //Debug.Log("Max Width Reached");
            GrowBoolean = false;
            ShrinkGrowCircle(m_ShrinkGrowCircle, GrowBoolean);
        }
        // Check if m_ShrinkGrowCircle.style.width is less than or equal to m_ShrinkGrowCircle.style.minWidth. If it is, change GrowBoolean to true and call ShrinkGrowCircle
        else if (m_ShrinkGrowCircle.resolvedStyle.width <= m_ShrinkGrowCircle.resolvedStyle.minWidth.value)
        {
            //Debug.Log("Min Width Reached");
            GrowBoolean = true;
            ShrinkGrowCircle(m_ShrinkGrowCircle, GrowBoolean);
        }
        // If neither of the above conditions are met, call ShrinkGrowCircle
        else
        {
            //Debug.Log("Neither Max or Min Width Reached");
            ShrinkGrowCircle(m_ShrinkGrowCircle, GrowBoolean);
        }
    }

    void AddElements()
    {
        /* Removed from Slider
        m_Dragger.pickingMode = PickingMode.Ignore;
        */


        m_BaseCircle = new VisualElement();
        m_ShrinkGrowCircle = new VisualElement();

        m_BaseCircle.style.position = Position.Absolute;
        m_ShrinkGrowCircle.style.position = Position.Absolute;

        m_BaseCircle.pickingMode = PickingMode.Position;
        m_ShrinkGrowCircle.pickingMode = PickingMode.Ignore;

        /* Removed from Slider
        m_Tracker.Add(m_Left);
        m_Tracker.Add(m_Right);
        m_Tracker.Add(m_Middle);
        */

        m_BaseCircle.name = "Base Circle 1";
        m_ShrinkGrowCircle.name = "Shrink Grow Circle 1";

        m_BaseCircle.AddToClassList("BaseCircle1");
        m_ShrinkGrowCircle.AddToClassList("ShrinkGrowCircle1");

        m_BaseCircle.style.flexGrow = 0f;
        m_ShrinkGrowCircle.style.flexGrow = 0f;
        m_BaseCircle.style.flexShrink = 0f;
        m_ShrinkGrowCircle.style.flexShrink = 0f;
        m_BaseCircle.style.flexWrap = Wrap.NoWrap;
        m_ShrinkGrowCircle.style.flexWrap = Wrap.NoWrap;

        m_Root.Add(m_ShrinkGrowCircle);
        m_Root.Add(m_BaseCircle);


        //m_BaseCircle.RegisterCallback<PointerDownEvent>()
        // Add PointerDownEvent to m_BaseCircle
        m_BaseCircle.RegisterCallback<ClickEvent>(OnBaseCircleClick);

        Debug.Log("Done adding circles");
    }

    void ShrinkGrowCircle(VisualElement m_ShrinkGrowCircle, bool growBooleanInput)
    {

        if (growBooleanInput == true)
        {
            // Grow the circle
            m_ShrinkGrowCircle.style.width = new StyleLength(m_ShrinkGrowCircle.resolvedStyle.width + 1f);
            m_ShrinkGrowCircle.style.height = new StyleLength(m_ShrinkGrowCircle.resolvedStyle.height + 1f);
            m_ShrinkGrowCircle.style.translate = new Translate(m_ShrinkGrowCircle.resolvedStyle.translate.x - .5f, m_ShrinkGrowCircle.resolvedStyle.translate.y - .5f);


        }

        else
        {
            // Shrink the circle
            m_ShrinkGrowCircle.style.width = new StyleLength(m_ShrinkGrowCircle.resolvedStyle.width - 1f);
            m_ShrinkGrowCircle.style.height = new StyleLength(m_ShrinkGrowCircle.resolvedStyle.height - 1f);
            m_ShrinkGrowCircle.style.translate = new Translate(m_ShrinkGrowCircle.resolvedStyle.translate.x + .5f, m_ShrinkGrowCircle.resolvedStyle.translate.y + .5f);

        }
    }

    void OnBaseCircleClick(ClickEvent evt)
    {
        Debug.Log("Base Circle Clicked start");
        // Only perform this action at the target, not in a parent
        if (evt.target != m_BaseCircle)
        {
            Debug.Log("Base Circle clicked NOT");
            return;
        }

        if (m_ShrinkGrowCircle.resolvedStyle.width <= m_ShrinkGrowCircle.resolvedStyle.minWidth.value + 50f)
        {
            Debug.Log("Good timing");
            MiningEvent.instance.currentScore += 2;
        }

        // +1 for good timing
    }

}
