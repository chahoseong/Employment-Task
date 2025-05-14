using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlockGroup : MonoBehaviour, IDraggable
{
    [Header("References")]
    [SerializeField] private BlockMovement blockMovement;
    [SerializeField] private BlockCollision blockCollision;
    [SerializeField] private Outline outline;

    private BlockBounds offsetBounds;
    private List<BlockPiece> blockPieces = new();
    private List<Vector2> blockOffsets = new();
    
    private Vector2 centerPosition;
    private bool isDragging = false;

    public int Id { get; set; }
    public List<ObjectPropertiesEnum.BlockGimmickType> GimmickType { get; set; } = new();
    public BlockBounds Bounds => new BlockBounds { min = centerPosition + offsetBounds.min, max = centerPosition + offsetBounds.max };
    public IEnumerable<BlockPiece> BlockPieces => blockPieces;
    
    #region Unity Events
    private void Start()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 2f;
        outline.enabled = false;
    }
    
    private void OnDisable()
    {
        blockMovement.enabled = false;
        blockCollision.enabled = false;
        transform.DOKill(true);
    }

    private void OnDestroy()
    {
        transform.DOKill(true);
    }
    #endregion
    
    #region IDraggable Implementation
    public void BeginDrag()
    {
        isDragging = true;
        outline.enabled = true;
        blockCollision.ResetCollisionState();
    }
    
    public void OnDrag(Vector3 delta)
    {
        SetBlockPosition(false);
        
        if (blockCollision.IsColliding && delta.magnitude > 0.5f)
        {
            if (Vector3.Dot(delta.normalized, blockCollision.LastCollisionNormal) > 0.1f)
            {
                blockCollision.ResetCollisionState();
            }
        }
        
        // 속도 계산 개선
        Vector3 velocity = CalculateVelocity(delta);
        blockMovement.Move(velocity);
    }
    
    private Vector3 CalculateVelocity(Vector3 moveVector)
    {
        if (!blockCollision.IsColliding)
        {
            return moveVector * blockMovement.FollowSpeed;
        }
        // 충돌면에 대해 속도 투영 (실제 이동)
        Vector3 projectedMove = Vector3.ProjectOnPlane(moveVector, blockCollision.LastCollisionNormal);
        return projectedMove * blockMovement.MoveSpeed;
    }
    
    public void EndDrag()
    {
        if (!isDragging)
        {
            return;
        }
        
        isDragging = false;
        outline.enabled = false;
        
        blockMovement.Stop();
        blockCollision.ResetCollisionState();
        
        SetBlockPosition();
    }
    #endregion
    
    private void SetBlockPosition(bool applyToTransform = true)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = new Vector3(
                hit.transform.position.x,
                transform.position.y,
                hit.transform.position.z
            );
            
            if (applyToTransform)
            {
                transform.position = targetPosition;
            }
            
            centerPosition.x = Mathf.Round(transform.position.x / 0.79f);
            centerPosition.y = Mathf.Round(transform.position.z / 0.79f);
            
            if (hit.collider.TryGetComponent(out BoardBlockObject boardBlock))
            {
                foreach (var blockPiece in blockPieces)
                {
                    blockPiece.SetBlockPosition(centerPosition);
                }
                foreach (var blockPiece in blockPieces)
                {
                    boardBlock.CheckAdjacentBlock(blockPiece, targetPosition);
                    blockPiece.CheckBelowBoardBlock(targetPosition);
                }
            }
        }
        else
        {
            Debug.LogWarning("Nothing Detected");
        }
    }
    
    public Vector3 GetCenterX()
    {
        if (blockPieces == null || blockPieces.Count == 0)
        {
            return Vector3.zero; // Return default value if list is empty
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (var blockPiece in blockPieces)
        {
            float blockX = blockPiece.transform.position.x;
        
            if (blockX < minX)
            {
                minX = blockX;
            }
        
            if (blockX > maxX)
            {
                maxX = blockX;
            }
        }
    
        // Calculate the middle value between min and max
        return new Vector3((minX + maxX) / 2f, transform.position.y, 0);
    }

    public Vector3 GetCenterZ()
    {
        if (blockPieces == null || blockPieces.Count == 0)
        {
            return Vector3.zero; // Return default value if list is empty
        }

        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (var blockPiece in blockPieces)
        {
            float blockZ = blockPiece.transform.position.z;
        
            if (blockZ < minZ)
            {
                minZ = blockZ;
            }
        
            if (blockZ > maxZ)
            {
                maxZ = blockZ;
            }
        }
    
        return new Vector3(transform.position.x, transform.position.y, (minZ + maxZ) / 2f);
    }
    
    public void AddPiece(BlockPiece blockPiece)
    {
        blockPieces.Add(blockPiece);
        blockPiece.Group = this;
        
        blockOffsets.Add(blockPiece.Offset);
        
        offsetBounds.min = new Vector2(
            Mathf.Min(blockPiece.Offset.x, offsetBounds.min.x),
            Mathf.Min(blockPiece.Offset.y, offsetBounds.min.y)
        );
        offsetBounds.max = new Vector2(
            Mathf.Max(blockPiece.Offset.x, offsetBounds.max.x),
            Mathf.Max(blockPiece.Offset.y, offsetBounds.max.y)
        );
    }
    
    public void DestroyMove(Vector3 pos, GameObject particle)
    {
        ClearPreboardBlockObjects();
        
        transform.DOMove(pos, 1f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(particle);
                Destroy(gameObject);
            });
    }
    
    private void ClearPreboardBlockObjects()
    {
        foreach (var blockPiece in blockPieces)
        {
            if (blockPiece.LastBoardBlock != null)
            {
                blockPiece.LastBoardBlock.playingBlock = null;
            }
        }
    }

    public void SetCollisionEnabled(bool isEnabled)
    {
        foreach (var blockPiece in blockPieces)
        {
            blockPiece.SetCollisionEnabled(isEnabled);
        }
    }
}
