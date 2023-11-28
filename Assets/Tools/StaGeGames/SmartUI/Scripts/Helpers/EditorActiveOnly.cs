using UnityEngine;

namespace StaGeGames.BestFit
{
    public class EditorActiveOnly : MonoBehaviour
    {
        [SerializeField]
        private bool isActiveInEditor = true;
        void Awake()
        {
            if (!isActiveInEditor) gameObject.SetActive(false);
#if !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }
    }
}
