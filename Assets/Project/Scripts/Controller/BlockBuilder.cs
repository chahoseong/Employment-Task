using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Builder/Block", fileName = "New Block Builder")]
public class BlockBuilder : ScriptableObject
{
    [SerializeField] private GameObject blockGroupPrefab; 
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private MaterialService materialService;
    
    public async Task Build(IEnumerable<PlayingBlockData> source, StageContext stageContext)
    {
        foreach (var blockData in source)
        {
            GameObject blockGroupObject = Instantiate(blockGroupPrefab, stageContext.BlocksRoot);
            blockGroupObject.transform.position = new Vector3(
                blockData.center.x * stageContext.BlockDistance, 
                0.33f, 
                blockData.center.y * stageContext.BlockDistance
            );
            
            BlockGroup blockGroup = blockGroupObject.GetComponent<BlockGroup>();
            blockGroup.Id = blockData.uniqueIndex;
            foreach (var gimmick in blockData.gimmicks)
            {
                if (System.Enum.TryParse(gimmick.gimmickType, out ObjectPropertiesEnum.BlockGimmickType gimmickType))
                {
                    blockGroup.GimmickType.Add(gimmickType);
                }
            }
            
            int maxX = 0;
            int minX = stageContext.BoardWidth;
            int maxY = 0;
            int minY = stageContext.BoardHeight;

            foreach (var shape in blockData.shapes)
            {
                GameObject singleBlock = Instantiate(blockPrefab, blockGroupObject.transform);
                singleBlock.transform.localPosition = new Vector3(
                    shape.offset.x * stageContext.BlockDistance,
                    0f,
                    shape.offset.y * stageContext.BlockDistance
                );
                
                var blockRenderer = singleBlock.GetComponentInChildren<SkinnedMeshRenderer>();
                if (blockRenderer != null && blockData.colorType >= 0)
                {
                    blockRenderer.material = materialService.GetTestBlockMaterial(blockData.colorType);
                }
                
                if (singleBlock.TryGetComponent(out BlockPiece blockPiece))
                {
                    blockPiece.Color = blockData.colorType;
                    blockPiece.BlockPosition = blockData.center + shape.offset;
                    blockPiece.Offset = new Vector2(shape.offset.x, shape.offset.y);
                     
                    blockGroup.AddPiece(blockPiece);
                    
                    BoardBlockObject boardBlock = stageContext.GetBoardBlock((int)blockPiece.BlockPosition.x, (int)blockPiece.BlockPosition.y);
                    boardBlock.playingBlock = blockPiece;
                    blockPiece.LastBoardBlock = boardBlock;
                }
            }
        }
        
        await Task.Yield();
    }
}
