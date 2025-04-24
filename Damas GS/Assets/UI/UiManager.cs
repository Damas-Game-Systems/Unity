using System.Collections.Generic;
using UnityEngine;

namespace Damas.UI
{
    public class UiManager : Singleton<UiManager>
    {
        [Header("Piece Info Windows")]
        [SerializeField] private PieceInfoWindow whitePieceWindowPrefab;
        [SerializeField] private PieceInfoWindow blackPieceWindowPrefab;

        [SerializeField] private PieceInfoWindow whitePieceWindow;
        [SerializeField] private PieceInfoWindow blackPieceWindow;

        public PieceInfoWindow OpenWindow(Piece piece)
        {
            return InstantiateWindow(piece, -1f);
        }

        public PieceInfoWindow OpenWindow(Piece piece, float duration)
        {
            return InstantiateWindow(piece, duration);
        }

        private PieceInfoWindow InstantiateWindow(Piece piece, float duration)
        {
            PieceInfoWindow newWindow = null;
            if (piece.color == PieceColor.White)
            {
                newWindow = Instantiate(whitePieceWindowPrefab);
            }
            else if (piece.color == PieceColor.Black)
            {
                newWindow = Instantiate(blackPieceWindowPrefab);
            }
            newWindow.Open(piece, duration);
            return newWindow;
        }

        public void CloseWindow(PieceInfoWindow window)
        {
            Destroy(window.gameObject);
        }
    }
}
