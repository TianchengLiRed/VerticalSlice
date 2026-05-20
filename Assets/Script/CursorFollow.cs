using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    [SerializeField] private RectTransform imageRect;
    [SerializeField] private Vector2 offset = new Vector2(30f, -30f);

    private void Awake()
    {
        if (imageRect == null)
            imageRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        imageRect.position = Input.mousePosition + (Vector3)offset;
    }
}
