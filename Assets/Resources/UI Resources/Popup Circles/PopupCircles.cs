using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupCircles : MonoBehaviour
{
    //Root Visual Element
    private VisualElement m_Root;

    // Circle that usually don't change
    private VisualElement m_BaseCircle;

    // Circle that shrinks or grows
    private VisualElement m_ShrinkGrowCircle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Root = GetComponent<UIDocument>().rootVisualElement;
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
    }

    void AddElements()
    {
        /* Removed from Slider
        m_Dragger.pickingMode = PickingMode.Ignore;
        */


        m_BaseCircle = new VisualElement();
        m_ShrinkGrowCircle = new VisualElement();

        /* Removed from Slider
        m_Tracker.Add(m_Left);
        m_Tracker.Add(m_Right);
        m_Tracker.Add(m_Middle);
        */

        m_BaseCircle.name = "Base Circle 1";
        m_ShrinkGrowCircle.name = "Shrink Grow Circle 1";

        m_BaseCircle.AddToClassList("BaseCircle1");
        m_ShrinkGrowCircle.AddToClassList("ShrinkGrowCircle1");

        m_Root.Add(m_BaseCircle);
        m_Root.Add(m_ShrinkGrowCircle);
        Debug.Log("Done adding circles");
    }

    void ShrinkGrowCircle(VisualElement m_ShrinkGrowCircle)
    {
        // Shrink or grow the circle
        //m_ShrinkGrowCircle.style.width = 50;
    }

}
