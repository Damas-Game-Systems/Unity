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
                if (whitePieceWindow != null)
                {
                    CloseWindow(whitePieceWindow);
                }
                whitePieceWindow = Instantiate(whitePieceWindowPrefab);
                newWindow = whitePieceWindow;
            }
            else if (piece.color == PieceColor.Black)
            {
                if (blackPieceWindow != null)
                {
                    CloseWindow(blackPieceWindow);
                }
                blackPieceWindow = Instantiate(blackPieceWindowPrefab);
                newWindow = blackPieceWindow;
            }
            newWindow.Open(piece, duration);
            return newWindow;
        }

        public void CloseWindow(PieceInfoWindow window)
        {
            if (window == null) { return; }
            Destroy(window.gameObject);
        }

        public void CloseAllWindows()
        {
            CloseWindow(whitePieceWindow);
            CloseWindow(blackPieceWindow);
        }
    }
}
