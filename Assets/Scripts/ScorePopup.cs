using UnityEngine;

// Points that float up from where they were earned ("100", "200", "1UP"...),
// drawn on the HUD canvas but following the world position as the camera moves.
[RequireComponent(typeof(PixelText))]
public class ScorePopup : MonoBehaviour
{
    public float duration = 0.8f;
    public float rise = 2f;

    private Vector3 worldPosition;
    private RectTransform canvasRect;
    private RectTransform rectTransform;
    private Camera worldCamera;
    private float elapsed;

    public static ScorePopup Create(RectTransform canvas, string text, Vector3 position)
    {
        PixelText label = PixelText.Create(canvas, "Score Popup", 14, TextAnchor.MiddleCenter);
        label.Text = text;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(80f, 20f);

        ScorePopup popup = label.gameObject.AddComponent<ScorePopup>();
        popup.canvasRect = canvas;
        popup.worldPosition = position + Vector3.up * 0.5f;
        popup.worldCamera = Camera.main;
        popup.UpdatePosition();
        return popup;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        elapsed += Time.deltaTime;

        // the camera belongs to the scene, so a scene change removes the popup
        if (elapsed >= duration || worldCamera == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (worldCamera == null) return;

        float t = Mathf.Clamp01(elapsed / duration);
        Vector3 position = worldPosition + Vector3.up * rise * Mathf.Sin(t * Mathf.PI * 0.5f);
        Vector2 screen = worldCamera.WorldToScreenPoint(position);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, null, out Vector2 local)) {
            rectTransform.anchoredPosition = local;
        }
    }

}
