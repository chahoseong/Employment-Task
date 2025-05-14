using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Builder/BoardBlock", fileName = "New BoardBlock Builder")]
public class BoardBlockBuilder : ScriptableObject
{
    [SerializeField] private GameObject boardBlockPrefab;
    
    public async Task Build(IEnumerable<BoardBlockData> source, StageContext stageContext)
    {
        CreateBoardBlocks(source, stageContext);
        UpdateStandardBlocks(stageContext);
        UpdateCheckBlocks(stageContext);
        
        await Task.Yield();

        stageContext.BoardWidth = stageContext.BoardBlocks.Max(element => element.Key.x);
        stageContext.BoardHeight = stageContext.BoardBlocks.Max(element => element.Key.y);
    }

    private void CreateBoardBlocks(IEnumerable<BoardBlockData> source, StageContext stageContext)
    {
        int standardBlockIndex = -1;
        
        foreach (var boardBlockData in source)
        {
            GameObject blockObj = Instantiate(boardBlockPrefab, stageContext.BoardBlocksRoot);
            blockObj.transform.localPosition = new Vector3(
                boardBlockData.x * stageContext.BlockDistance,
                0,
                boardBlockData.y * stageContext.BlockDistance
            );

            if (blockObj.TryGetComponent(out BoardBlockObject boardBlock))
            {
                boardBlock.x = boardBlockData.x;
                boardBlock.y = boardBlockData.y;

                bool isCheckBlock = false;
                
                foreach (var wall in stageContext.GetWalls(boardBlock.x, boardBlock.y))
                {
                    boardBlock.colorType.Add(wall.Color);
                    boardBlock.len.Add(wall.Length);
                    
                    bool isHorizontal = wall.Direction is DestroyWallDirection.Up or DestroyWallDirection.Down;
                    boardBlock.isHorizon.Add(isHorizontal);
                    
                    stageContext.SetStandardBoardBlock(++standardBlockIndex, boardBlock, isHorizontal);
                    
                    isCheckBlock = true;
                }

                boardBlock.isCheckBlock = isCheckBlock;
                
                stageContext.SetBoardBlock(boardBlock, boardBlockData.x, boardBlockData.y);
            }
            else
            {
                Debug.LogWarning("boardBlockPrefab에 BoardBlockObject 컴포넌트가 필요합니다!");
            }
        }
    }

    private void UpdateStandardBlocks(StageContext stageContext)
    {
        foreach (var (uniqueIndex, (boardBlock, isHorizontal)) in stageContext.StandardBlocks)
        {
            for (int i = 0; i < boardBlock.colorType.Count; ++i)
            {
                if (isHorizontal)
                {
                    for (int x = boardBlock.x + 1; x < boardBlock.x + boardBlock.len[i]; ++x)
                    {
                        BoardBlockObject targetBoardBlock = stageContext.GetBoardBlock(x, boardBlock.y);
                        if (targetBoardBlock)
                        {
                            targetBoardBlock.colorType.Add(boardBlock.colorType[i]);
                            targetBoardBlock.len.Add(boardBlock.len[i]);
                            targetBoardBlock.isHorizon.Add(true);
                            targetBoardBlock.isCheckBlock = true;
                        }
                    }
                }
                else
                {
                    for (int y = boardBlock.y + 1; y < boardBlock.y + boardBlock.len[i]; ++y)
                    {
                        BoardBlockObject targetBoardBlock = stageContext.GetBoardBlock(boardBlock.x, y);
                        if (targetBoardBlock)
                        {
                            targetBoardBlock.colorType.Add(boardBlock.colorType[i]);
                            targetBoardBlock.len.Add(boardBlock.len[i]);
                            targetBoardBlock.isHorizon.Add(false);
                            targetBoardBlock.isCheckBlock = true;
                        }
                    }
                }
            }
        }
    }

    private void UpdateCheckBlocks(StageContext stageContext)
    {
        int checkBlockIndex = -1;
        
        foreach (var (boardPosition, boardBlock) in stageContext.BoardBlocks)
        {
            for (int i = 0; i < boardBlock.colorType.Count; ++i)
            {
                if (!boardBlock.isCheckBlock || boardBlock.colorType[i] == ColorType.None)
                {
                    continue;
                }

                // 이 블록이 이미 그룹에 속해있는지 확인
                if (boardBlock.checkGroupIdx.Count <= i)
                {
                    if (boardBlock.isHorizon[i])
                    {
                        // 왼쪽 블록 확인
                        Vector2Int leftPosition = new Vector2Int(boardBlock.x - 1, boardBlock.y);
                        BoardBlockObject leftBlock = stageContext.GetBoardBlock(leftPosition);
                        if (leftBlock &&
                            i < leftBlock.colorType.Count &&
                            leftBlock.colorType[i] == boardBlock.colorType[i] &&
                            leftBlock.checkGroupIdx.Count > i)
                        {
                            int groupIndex = leftBlock.checkGroupIdx[i];
                            stageContext.AddCheckBoardBlock(groupIndex, boardBlock);
                            boardBlock.checkGroupIdx.Add(groupIndex);
                        }
                        else
                        {
                            checkBlockIndex++;
                            stageContext.AddCheckBoardBlock(checkBlockIndex, boardBlock);
                            boardBlock.checkGroupIdx.Add(checkBlockIndex);
                        }
                    }
                    else
                    {
                        // 위쪽 블록 확인
                        Vector2Int upPosition = new Vector2Int(boardBlock.x, boardBlock.y - 1);
                        BoardBlockObject upBlock = stageContext.GetBoardBlock(upPosition);
                        if (upBlock &&
                            i < upBlock.colorType.Count &&
                            upBlock.colorType[i] == boardBlock.colorType[i] &&
                            upBlock.checkGroupIdx.Count > i)
                        {
                            int groupIndex = upBlock.checkGroupIdx[i];
                            stageContext.AddCheckBoardBlock(groupIndex, boardBlock);
                            boardBlock.checkGroupIdx.Add(groupIndex);
                        }
                        else
                        {
                            checkBlockIndex++;
                            stageContext.AddCheckBoardBlock(checkBlockIndex, boardBlock);
                            boardBlock.checkGroupIdx.Add(checkBlockIndex);
                        }
                    }
                }
            }
        }
    }
}
