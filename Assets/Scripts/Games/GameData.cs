using UnityEngine;

namespace Games
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/Game")]
    public class GameData : ScriptableObject
    {
        [field: SerializeField] public string SceneName { get; private set; }
        [field: SerializeField] public bool IsLooked { get; private set; }
    }
}