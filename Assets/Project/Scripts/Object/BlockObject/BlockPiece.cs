using UnityEngine;

public class BlockPiece : MonoBehaviour
{
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Collider touchCollider;

    public ColorType Color { get; set; }
    public BlockGroup Group { get; set; }
    public Vector2 BlockPosition { get; set; }
    public Vector2 Offset { get; set; }
    public BoardBlockObject LastBoardBlock { get; set; }

    public void SetBlockPosition(Vector2 centerPosition)
    {
        BlockPosition = new Vector2(centerPosition.x + Offset.x, centerPosition.y + Offset.y);
    }
    
    public void CheckBelowBoardBlock(Vector3 dropPosition)
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out BoardBlockObject boardBlock))
            {
                // 이전 BoardBlockObject의 playingBlock 초기화
                if (LastBoardBlock != null && LastBoardBlock != boardBlock)
                {
                    LastBoardBlock.playingBlock = null;
                }

                if(boardBlock.CheckAdjacentBlock(this, dropPosition)) boardBlock.playingBlock = this;

                // 이전 BoardBlockObject 갱신
                LastBoardBlock = boardBlock;
            }
        }
        else
        {
            Debug.LogWarning("Nothing Detected");

            // 이전 BoardBlockObject가 있으면 초기화
            if (LastBoardBlock != null)
            {
                LastBoardBlock.playingBlock = null;
                LastBoardBlock = null;
            }
        }
    }

    public void SetCollisionEnabled(bool isEnabled)
    {
        mainCollider.enabled = isEnabled;
        touchCollider.enabled = isEnabled;
    }
}
