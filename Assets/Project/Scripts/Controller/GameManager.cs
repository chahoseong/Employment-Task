using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageBuilder stageBuilder;
    [SerializeField] private BoardController boardController;
    [SerializeField] private Transform quadsTransform;
    
    [SerializeField] private MaterialService materialService;
    [SerializeField] private ParticleEffectService particleEffectService;

    private int currentStageNumber = 0;
    private StageContext stageContext;

    private GameObject wallsRootObject;
    private GameObject boardBlocksRootObject;
    private GameObject blocksRootObject;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
    private void Start()
    {
        StartServices();
        StartStage(0);
    }
    
    private void StartServices()
    {
        ServiceLocator.Provide(materialService);
        ServiceLocator.Provide(particleEffectService);
    }

    private async void StartStage(int stageNumber)
    {
        InitRootTransforms();
        
        StageInitParams stageInitParams = new StageInitParams
        {
            stageNumber = stageNumber,
            wallsRootTransform = boardBlocksRootObject.transform,
            boardBlocksRootTransform = blocksRootObject.transform,
            blocksRootTransform = blocksRootObject.transform
        };
        stageContext = await stageBuilder.Build(stageInitParams);
        
        currentStageNumber = stageNumber;
        
        boardController.Init(stageContext);
        
        foreach (var (boardPosition, boardBlock) in stageContext.BoardBlocks)
        {
            boardBlock.controller = boardController;
        }
    }
    
    private void InitRootTransforms()
    {
        // board block
        if (boardBlocksRootObject)
        {
            Destroy(boardBlocksRootObject);
        }
        boardBlocksRootObject = new GameObject("BoardParent");
        boardBlocksRootObject.transform.SetParent(transform);
        
        // wall
        wallsRootObject = new GameObject("CustomWallParent");
        wallsRootObject.transform.SetParent(boardBlocksRootObject.transform);

        // block
        if (blocksRootObject)
        {
            Destroy(blocksRootObject);
        }
        blocksRootObject = new GameObject("PlayingBlockParent");
        
        // quad
        while (quadsTransform.childCount > 0)
        {
            Destroy(quadsTransform.GetChild(quadsTransform.childCount - 1).gameObject);
        }
    }
    
    public void GoToPreviousLevel()
    {
        if (stageBuilder.IsFirstStage(currentStageNumber)) return;
        
        StartStage(--currentStageNumber);
        
        StartCoroutine(Wait());
    }

    public void GotoNextLevel()
    {
        if (stageBuilder.IsLastStage(currentStageNumber)) return;
        
        StartStage(++currentStageNumber);
        
        StartCoroutine(Wait());
    }
    

    private IEnumerator Wait()
    {
        yield return null;
        
        Vector3 camTr = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(1.5f + 0.5f * (stageContext.BoardWidth - 4),camTr.y,camTr.z);
    }
}
