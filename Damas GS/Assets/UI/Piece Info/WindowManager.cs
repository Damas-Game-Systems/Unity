using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Damas.UI
{
    public abstract class WindowManager<T> : MonoBehaviour where T : Window
    {
        [field: SerializeField, ReadOnly] protected List<T> openWindows = new();

        public event Action<T> WindowOpened;

        [SerializeField] protected T windowPrefab;

        public void RequestOpenWindow(WindowData data)
        {
            T newWindow = Instantiate(windowPrefab, transform);
            
            Assert.IsNotNull(newWindow);

            InitializeWindow(newWindow, data);
        }

        public void RequestCloseWindow(T windowToClose)
        {
            Assert.IsNotNull(openWindows);
            Assert.IsTrue(openWindows.Count > 0);
            Assert.IsNotNull(windowToClose);

            if (openWindows.Contains(windowToClose))
            {
                openWindows.Remove(windowToClose);
                Destroy(windowToClose.gameObject);
            }
            else
            {
                // error window doesn't exist in this manager
            }
        }

        protected abstract void InitializeWindow(T newWindow, WindowData data);

        protected virtual void OnWindowOpened(T window)
        {
            WindowOpened?.Invoke(window);
            Debug.Log(window.ToString(), window);
        }
    }
}