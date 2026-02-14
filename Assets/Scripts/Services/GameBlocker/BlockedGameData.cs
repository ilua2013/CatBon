using System;
using UnityEngine;
using UnityEngine.UI;

namespace Services.GameBlocker
{
    [Serializable]
    public class BlockedGameData
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image Lock { get; private set; }
    }
}