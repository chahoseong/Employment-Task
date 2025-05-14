using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody mainRigidbody;
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float maxSpeed = 20f;

    private Vector3 velocity;
    
    public float MoveSpeed => moveSpeed;
    public float FollowSpeed => followSpeed;

    private void Start()
    {
        mainRigidbody.useGravity = false;
        mainRigidbody.isKinematic = true;
        mainRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        mainRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 감지 모드 향상
    }
    
    private void FixedUpdate()
    {
        if (!mainRigidbody.isKinematic)
        {
            mainRigidbody.linearVelocity = Vector3.Lerp(
                mainRigidbody.linearVelocity,
                velocity,
                Time.fixedDeltaTime * 10f
            );
        }
    }
    
    public void Move(Vector3 desiredVelocity)
    {
        if (mainRigidbody.isKinematic)
        {
            mainRigidbody.isKinematic = false;
        }
        
        // 속도 제한
        if (desiredVelocity.magnitude > maxSpeed)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        velocity = desiredVelocity;
    }

    public void Stop()
    {
        if (!mainRigidbody.isKinematic)
        {
            mainRigidbody.linearVelocity = Vector3.zero;
            mainRigidbody.isKinematic = true;
        }
    }
}
