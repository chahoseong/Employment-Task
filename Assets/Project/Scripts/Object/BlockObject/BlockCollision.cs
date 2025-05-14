using UnityEngine;
using UnityEngine.Serialization;

public class BlockCollision : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float collisionResetTime = 0.1f; // 충돌 상태 자동 해제 시간
    
    private bool isColliding = false;
    private Vector3 lastCollisionNormal;
    private float lastCollisionTime;

    public bool IsColliding => isColliding;
    public Vector3 LastCollisionNormal => lastCollisionNormal;
    
    private void Update()
    {
        // 충돌 상태 자동 해제 검사
        if (isColliding && Time.time - lastCollisionTime > collisionResetTime)
        {
            // 일정 시간 동안 충돌 갱신이 없으면 충돌 상태 해제
            ResetCollisionState();
        }
    }
    
    // 충돌 감지
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }
    
    private void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }
    
    private void HandleCollision(Collision collision)
    {
        if (collision.contactCount > 0 && collision.gameObject.layer != LayerMask.NameToLayer("Board"))
        {
            Vector3 normal = collision.contacts[0].normal;
            
            // 수직 충돌(바닥과의 충돌)은 무시
            if (Vector3.Dot(normal, Vector3.up) < 0.8f)
            {
                isColliding = true;
                lastCollisionNormal = normal;
                lastCollisionTime = Time.time; // 충돌 시간 갱신
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        // 현재 충돌 중인 오브젝트가 떨어질 때만 충돌 상태 해제
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            
            // 현재 저장된 충돌 normal과 유사한 경우에만 해제
            if (Vector3.Dot(normal, lastCollisionNormal) > 0.8f)
            {
                ResetCollisionState();
            }
        }
    }
    
    // 충돌 상태 초기화
    public void ResetCollisionState()
    {
        isColliding = false;
        lastCollisionNormal = Vector3.zero;
    }
}
