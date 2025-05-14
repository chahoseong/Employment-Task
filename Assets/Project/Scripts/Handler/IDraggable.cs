using UnityEngine;

public interface IDraggable
{
    void BeginDrag();
    void OnDrag(Vector3 worldDelta);
    void EndDrag();
}
