using UnityEngine;

namespace Damas.UI
{
    public class PieceInfoWindowManager : WindowManager<PieceInfoWindow>
    {
        [SerializeField] private bool staticPositioning;
        [SerializeField] private RectTransform whitePosition;
        [SerializeField] private RectTransform blackPosition;

        protected override void InitializeWindow(PieceInfoWindow newWindow, WindowData data)
        {
            if (newWindow.Initialize(data))
            {
                openWindows.Add(newWindow);
                if (staticPositioning)
                {
                    newWindow.RT.position = (data as PieceInfoWindowData).piece.color switch
                    {
                        PieceColor.White => whitePosition.position,
                        PieceColor.Black => blackPosition.position,
                        _                => whitePosition.position
                    };
                }

                newWindow.Show();
                OnWindowOpened(openWindows[openWindows.Count - 1]);
            }
            else
            {
                // error, couldnt open window
            }
        }
    }
}