using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField]
    public RectTransform cursorImage; // Assign your circle image RectTransform in the Inspector.
    private Vector2 lockedPosition;  // The position where the cursor is locked
    private bool isLocked = false;
    [SerializeField]
    private RectTransform lineImage;

    // Maximum distance for color interpolation
    private float maxDistance = 300f; // Adjust this based on your needs
    [SerializeField]
    private Color closeColor = Color.green;
    [SerializeField]
    private Color farColor = Color.red;

    private RawImage lineRawImage; // Reference to the RawImage component

    private void Start()
    {
        // Hide the default cursor
        Cursor.visible = false;
        lineImage.gameObject.SetActive(false);

        // Get the RawImage component on the line
        lineRawImage = lineImage.GetComponent<RawImage>();
        if (lineRawImage == null)
        {
            Debug.LogError("Line Image is missing a RawImage component!");
        }
    }

    private void Update()
    {
        // Move the custom cursor image to the mouse position
        Vector2 mousePosition = Input.mousePosition;
        cursorImage.position = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            // Lock the cursor position
            lockedPosition = mousePosition;
            isLocked = true;
            lineImage.gameObject.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Release the cursor lock
            isLocked = false;
            lineImage.gameObject.SetActive(false);
        }

        if (isLocked)
        {
            cursorImage.position = lockedPosition; // Keep it at the locked position
            Vector2 direction = mousePosition - lockedPosition;
            // Clamp the x and y components of the direction
            direction.x = Mathf.Clamp(direction.x, -50f, 50f);
            direction.y = Mathf.Clamp(direction.y, -50f, 50f);

            cursorImage.rotation = Quaternion.Euler(0f, -direction.x, 0f);
            DrawLine(lockedPosition, mousePosition);
        }
        else
        {
            cursorImage.position = mousePosition; // Follow the mouse
            cursorImage.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        // Set the position of the line to be between the start and end points
        lineImage.position = (start + end) / 2;

        // Calculate the distance and angle between the points
        float distance = Vector2.Distance(start, end);

        // Clamp the distance to a maximum of 300f
        if (distance > 300f)
        {
            // Calculate the direction from start to end
            Vector2 direction = (end - start).normalized;

            // Adjust the end point to be within the maximum distance
            end = start + direction * 300f;

            // Recalculate the distance with the new end point
            distance = 300f;
        }

        // Interpolate color based on distance
        float t = Mathf.Clamp01(distance / maxDistance); // Normalize distance to range [0, 1]
        Color currentColor = Color.Lerp(closeColor, farColor, t);

        // Apply the color to the RawImage's material
        if (lineRawImage != null)
        {
            lineRawImage.color = currentColor;
        }

        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        // Adjust the line's size and rotation
        lineImage.sizeDelta = new Vector2(distance, lineImage.sizeDelta.y);
        lineImage.rotation = Quaternion.Euler(0, 0, angle);
    }
}
