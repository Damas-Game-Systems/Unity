using UnityEngine;

namespace Damas
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad;

        public static T Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this as T;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(Instance);
                }
            }
        }
    }
}