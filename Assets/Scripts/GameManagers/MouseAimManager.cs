using UnityEngine;

public class MouseAimManager
{
    public Vector3 MouseWorldPosition { get; private set; }
    public Vector2 MouseScreenPosition { get; private set; }
    public GameObject HitGameObject { get; private set; }
    public RaycastHit DefaltHit { get; private set; }

    public void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f);
        DefaltHit = hit;
        MouseWorldPosition = hit.point;
        MouseScreenPosition = Input.mousePosition;
        HitGameObject = hit.collider != null ? hit.collider.gameObject : null;
    }
    
}