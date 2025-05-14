using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Service/ParticleEffect", fileName = "New Particle Effect Service")]
public class ParticleEffectService : ScriptableObject
{
    [SerializeField] private ParticleSystem destroyParticlePrefab;
    [SerializeField] private MaterialService materialService;

    private GameObject root;
    
    public void Startup()
    {
        root = new GameObject();
    }

    public GameObject CreateBlockDestroyEffect(ColorType color)
    {
        var particleRoot = Instantiate(destroyParticlePrefab);
        var particleChildren = particleRoot.GetComponentsInChildren<ParticleSystem>();
        
        foreach (var element in particleChildren)
        {
            var particleRenderer = element.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = materialService.GetWallMaterial(color);
        }

        return particleRoot.gameObject;
    }
}
