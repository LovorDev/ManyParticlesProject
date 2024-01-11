using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class LoadFirstScene : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadSceneAsync("FirstScene", LoadSceneMode.Additive);
        }
    }
}