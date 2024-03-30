using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomSlider : MonoBehaviour
{
    //Root Visual Element
    private VisualElement m_Root;

    //Slider
    private VisualElement m_Slider;

    //Dragger
    private VisualElement m_Dragger;

    //Bar
    private VisualElement m_Bar;

    //New Dragger
    private VisualElement m_NewDragger;


    // Start is called before the first frame update
    void Start()
    {
        // Get references to necessary visual elements
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_Slider = m_Root.Q<VisualElement>("MySlider");
        m_Dragger = m_Slider.Q<VisualElement>("unity-dragger");

        AddElements();

        m_Slider.RegisterCallback<ChangeEvent<float>>(SliderValueChanged);
    }

    void AddElements()
    {
        m_Bar = new VisualElement();
        m_Dragger.Add(m_Bar);
        m_Bar.name = "Bar";
        m_Bar.AddToClassList("bar");

        m_NewDragger = new VisualElement();
        m_Dragger.Add(m_NewDragger);
        m_NewDragger.name = "NewDragger";
        m_NewDragger.AddToClassList("newdragger");
    }

    void SliderValueChanged(ChangeEvent<float> evt)
    {
        //m_Bar.style.width = m_Slider.value * m_Slider.contentRect.width;
        //m_NewDragger.style.left = m_Slider.value * m_Slider.contentRect.width;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
