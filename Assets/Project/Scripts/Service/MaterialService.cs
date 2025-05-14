using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Service/Material", fileName = "New Material Service")]
public class MaterialService : ScriptableObject
{
    [SerializeField] private Material[] blockMaterials;
    [SerializeField] private Material[] testBlockMaterials;
    [SerializeField] private Material[] wallMaterials;

    public Material GetBlockMaterial(ColorType colorType)
    {
        return blockMaterials[(int)colorType];
    }

    public Material GetTestBlockMaterial(ColorType colorType)
    {
        return testBlockMaterials[(int)colorType];
    }
    
    public Material GetWallMaterial(ColorType colorType)
    {
        return wallMaterials[(int)colorType];
    }
}
