using UnityEngine;

namespace Damas.UI
{
    public class UiManager : Singleton<UiManager>
    {
        [field: SerializeField] public PieceInfoWindowManager PieceInfoUI { get; private set; }
    }
}