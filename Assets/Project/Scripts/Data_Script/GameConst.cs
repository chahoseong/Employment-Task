using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/GameConst", fileName = "GameConst")]
public class GameConst : ScriptableObject
{
    [field: SerializeField] public float BlockDistance { get; private set; }
}
