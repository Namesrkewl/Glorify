using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatController : MonoBehaviour {
    private ScrollRect scrollRect;
    private RectTransform contentRectTransform;
    private float savedScrollPosition;

    void Awake() {
        scrollRect = GetComponent<ScrollRect>();
        contentRectTransform = scrollRect.content;
        // Ensure to subscribe to the delegate/event in ChatManager that notifies about message additions.
        ChatManager.instance.onMessageAdded += MessageAdded;
    }

    void OnDestroy() {
        if (ChatManager.instance != null) {
            // Unsubscribe when the object is destroyed to prevent memory leaks.
            ChatManager.instance.onMessageAdded -= MessageAdded;
        }
    }

    private void MessageAdded() {
        StartCoroutine(AdjustScrollPositionAfterFrame());
    }

    private IEnumerator AdjustScrollPositionAfterFrame() {
        // Save the current scroll position before the frame updates
        float contentHeightBefore = contentRectTransform.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        Debug.Log("Viewport Height: " + viewportHeight);
        savedScrollPosition = (contentHeightBefore - viewportHeight) * scrollRect.verticalNormalizedPosition;
        Debug.Log("Saved scroll position: " + savedScrollPosition);

        Debug.Log(contentHeightBefore);

        // Wait for the end of the frame to ensure all UI elements are updated
        yield return new WaitForEndOfFrame();

        // Recalculate the scroll position based on the new content height
        float contentHeightAfter = contentRectTransform.rect.height;
        float difference = contentHeightAfter - contentHeightBefore;
        Debug.Log(difference);

        Debug.Log(contentHeightAfter);

        if (contentHeightAfter > viewportHeight && scrollRect.verticalNormalizedPosition != 0) {
            float newScrollPosition = (savedScrollPosition + difference) / (contentHeightAfter - viewportHeight);
            Debug.Log("New scroll position: " + newScrollPosition);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newScrollPosition);
        }
    }
}