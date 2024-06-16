using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class CustomSlider : MonoBehaviour
{
    //Root Visual Element
    private VisualElement m_Root;

    //Slider
    private Slider m_Slider;

    //Dragger
    private VisualElement m_Dragger;

    //Bar
    private VisualElement m_Bar;

    //Tracker
    private VisualElement m_Tracker;

    //New Dragger
    private VisualElement m_NewDragger;

    //Color Elements
    private VisualElement m_Left;
    private VisualElement m_Right;
    private VisualElement m_Middle;

    private bool sliderDirection = true;
    float changeAmount = .03f;

    private float m_Dragger_Size;
    private float m_Dragger_Size_Relative;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to necessary visual elements
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_Slider = m_Root.Q<Slider>("MySlider");
        m_Dragger = m_Slider.Q<VisualElement>("unity-dragger");
        m_Tracker = m_Slider.Q<VisualElement>("unity-tracker");

        AddElements();

        m_Slider.RegisterCallback<ChangeEvent<float>>(SliderValueChanged);

        m_Slider.RegisterCallback<GeometryChangedEvent>(SliderInit);

        m_Slider.value = 66.6666666666f;

    }

    void AddElements()
    {
        m_Dragger.pickingMode = PickingMode.Ignore;

        m_Left = new VisualElement();
        m_Right = new VisualElement();
        m_Middle = new VisualElement();

        m_Tracker.Add(m_Left);
        m_Tracker.Add(m_Right);
        m_Tracker.Add(m_Middle);

        m_Left.name = "Left";
        m_Right.name = "Right";
        m_Middle.name = "Middle";

        m_Left.AddToClassList("left");
        m_Right.AddToClassList("right");
        m_Middle.AddToClassList("middle");




    }

    void SliderValueChanged(ChangeEvent<float> evt)
    {
    }

    void SliderInit(GeometryChangedEvent evt)
    {
        m_Dragger_Size = m_Dragger.layout.width;

        // Dragger size relative to the max value of the slider. This is to include the offset of the dragger size in case it overlaps the threshold lines.
        // Not sure if the dragger is in between the different colours on the tracker, if we want to use the middle value, or the outside value.
        // I'm using the middle value for now.
        m_Dragger_Size_Relative = (m_Dragger_Size / m_Tracker.layout.width * m_Slider.highValue);
        Debug.Log(m_Dragger_Size);
    }

    void ChangeSliderValue(float changeAmount)
    {
        //m_Bar.style.width = m_Slider.value * m_Slider.contentRect.width;
        //m_NewDragger.style.left = m_Slider.value * m_Slider.contentRect.width;

        /*
        Vector2 dist = new Vector2((m_NewDragger.layout.width - m_Dragger.layout.width) / 2 - 5f, (m_NewDragger.layout.height - m_Dragger.layout.height) / 2 - 5f);
        Vector2 pos = m_Dragger.parent.LocalToWorld(m_Dragger.transform.position);
        m_NewDragger.transform.position = m_NewDragger.parent.WorldToLocal(pos - dist);
        */

        //Vector2 dist = new Vector2((m_NewDragger.layout.width - m_Dragger.layout.width) / 2 - 5f, (m_NewDragger.layout.height - m_Dragger.layout.height) / 2 - 5f);
        //Vector2 pos = m_Dragger.parent.LocalToWorld(m_Dragger.transform.position);



        //m_NewDragger.transform.position = m_NewDragger.transform.position + new Vector3(0.5f, 0, 0);
        //m_Dragger.transform.position = m_NewDragger.transform.position;

        // Set value of slider
        m_Slider.value += changeAmount;


    }

    // Update is called once per frame
    void Update()
    {

        if (m_Slider.value >= m_Slider.highValue && sliderDirection == true)
        {
            changeAmount *= -1f;
            sliderDirection = false;
        }
        else if (m_Slider.value <= m_Slider.lowValue && sliderDirection == false)
        {
            changeAmount *= -1f;
            sliderDirection = true;
        }

        ChangeSliderValue(changeAmount);



        //Vector2 pos = m_Yellow.parent.LocalToWorld(m_Yellow.layout.position);
        //m_Black.transform.position = m_Black.parent.WorldToLocal(pos);


        if (Input.GetKeyDown("space"))
        {
            float m_Dragger_Size_Relative_Half = m_Dragger_Size_Relative / 2;
            if (m_Slider.value + m_Dragger_Size_Relative_Half >= 33.3333333333f && m_Slider.value - m_Dragger_Size_Relative_Half <= 66.6666666666f)
            {
                Debug.Log("space key was pressed in middle");
            }
            else if (m_Slider.value - m_Dragger_Size_Relative_Half < 33.3333333333f)
            {
                Debug.Log("space key was pressed on left side");
            }
            else if (m_Slider.value + m_Dragger_Size_Relative_Half > 66.6666666666f)
            {
                Debug.Log("space key was pressed on right side");
            }
        }
    }
}
