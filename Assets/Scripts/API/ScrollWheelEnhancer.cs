using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollWheelEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] public ScrollRect targetScrollRect;
    private bool isHovering = false;

    void Update() {
        if (isHovering) {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                Debug.Log($"Scrolling {targetScrollRect.name}");
                targetScrollRect.verticalNormalizedPosition += scroll * Time.deltaTime * 100; // Adjust the multiplier as needed
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        Debug.Log("Hovering");
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
        Debug.Log("Exiting");
    }
}