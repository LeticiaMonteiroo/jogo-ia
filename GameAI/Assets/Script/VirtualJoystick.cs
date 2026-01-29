using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform background;
    private RectTransform handle;
    private Vector2 inputVector;

    // Public properties to access input direction
    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;

    void Start()
    {
        background = GetComponent<RectTransform>();
        // Assumes the Handle is the first child of the Background
        handle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPosition;
        
        // Converts screen point to local point within the background rectangle
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            // Normalizes position between 0 and 1
            touchPosition.x = (touchPosition.x / background.sizeDelta.x);
            touchPosition.y = (touchPosition.y / background.sizeDelta.y);

            // Calculates input vector (mapping 0..1 to -1..1)
            inputVector = new Vector2(touchPosition.x * 2 - 1, touchPosition.y * 2 - 1);
            
            // Keeps the vector circular (avoids faster diagonal movement)
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Moves the handle visual
            handle.anchoredPosition = new Vector2(inputVector.x * (background.sizeDelta.x / 2), inputVector.y * (background.sizeDelta.y / 2));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // Starts dragging immediately on touch
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero; // Resets input
        handle.anchoredPosition = Vector2.zero; // Resets handle visual to center
    }
}