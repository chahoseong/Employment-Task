using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Builder/Quad", fileName = "New Quad Builder")]
public class QuadBuilder : ScriptableObject
{
    [SerializeField] private GameObject quadPrefab;
    [SerializeField] private float yOffset = 0.625f;
    [SerializeField] private float wallOffset = 0.225f;

    public async Task Build(StageContext stageContext)
    {
        for (int i = -3; i <= stageContext.BoardWidth + 3; i++)
        {
            for (int j = -3; j <= stageContext.BoardHeight + 3; j++)
            {
                BoardBlockObject boardBlock = stageContext.GetBoardBlock(i, j);
                if (boardBlock) continue;

                float xValue = i;
                float zValue = j;
                if (i == -1 && j <= stageContext.BoardHeight) xValue -= wallOffset;
                if (i == stageContext.BoardWidth + 1 && j <= stageContext.BoardHeight + 1) xValue += wallOffset;
                
                if (j == -1 && i <= stageContext.BoardWidth) zValue -= wallOffset;
                if (j == stageContext.BoardHeight + 1 && i <= stageContext.BoardWidth + 1) zValue += wallOffset;
                
                GameObject quad = Instantiate(quadPrefab, stageContext.QuadsRoot);
                quad.transform.position = stageContext.BlockDistance * new Vector3(xValue, yOffset, zValue);
            }
        }
    }
}
