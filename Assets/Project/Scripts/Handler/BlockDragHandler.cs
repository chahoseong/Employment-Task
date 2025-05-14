using System.Collections.Generic;
using UnityEngine;

public class BlockDragHandler : MonoBehaviour
{
    public int uniqueIndex;
    public List<ObjectPropertiesEnum.BlockGimmickType> gimmickType;
    
    private IDraggable draggable;
    
    private Camera mainCamera;
    private float zDistanceToCamera;
    private bool isDragging = false;
    private Vector3 offset;
    
    private void Start()
    {
        mainCamera = Camera.main;
        draggable = GetComponent<IDraggable>();
    }

    private void OnMouseDown()
    {
        isDragging = true;
        
        // 카메라와의 z축 거리 계산
        zDistanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        // 마우스와 오브젝트 간의 오프셋 저장
        offset = transform.position - GetMouseWorldPosition();
        
        draggable?.BeginDrag();
    }

    void OnMouseUp()
    {
        draggable?.EndDrag();
        isDragging = false;
    }
    
    private void FixedUpdate()
    {
        if (!isDragging)
        {
            return;
        }
        
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPosition = mouseWorldPos + offset;
        Vector3 delta = targetPosition - transform.position;
        draggable?.OnDrag(delta);
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = zDistanceToCamera;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
    
    public void ReleaseInput()
    {
        if (isDragging)
        {
            draggable?.EndDrag();
        }
        isDragging = false;
    }
}
