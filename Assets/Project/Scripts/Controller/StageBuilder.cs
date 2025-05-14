using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Builder/Stage", fileName = "New Stage Builder")]
public class StageBuilder : ScriptableObject
{
    [SerializeField] private StageData[] stageDatas;
    [SerializeField] private WallBuilder wallBuilder;
    [SerializeField] private BoardBlockBuilder boardBlockBuilder;
    [SerializeField] private BlockBuilder blockBuilder;
    
    public readonly float BlockDistance = 0.79f;
    
    public async Task<StageContext> Build(StageInitParams initParams)
    {
        if (stageDatas == null)
        {
            Debug.LogError("StageData가 할당되지 않았습니다!");
            return null;
        }
        
        StageContext stageContext = new StageContext
        {
            WallsRoot = initParams.wallsRootTransform,
            BoardBlocksRoot = initParams.boardBlocksRootTransform,
            BlocksRoot = initParams.blocksRootTransform,
            BlockDistance = BlockDistance,
        };

        await wallBuilder.Build(stageDatas[initParams.stageNumber].Walls, stageContext);
        await boardBlockBuilder.Build(stageDatas[initParams.stageNumber].boardBlocks, stageContext);
        await blockBuilder.Build(stageDatas[initParams.stageNumber].playingBlocks, stageContext);
        
        return stageContext;
    }

    public bool IsFirstStage(int stageNumber)
    {
        return stageNumber == 0;
    }

    public bool IsLastStage(int stageNumber)
    {
        return stageNumber == stageDatas.Length - 1;
    }
}