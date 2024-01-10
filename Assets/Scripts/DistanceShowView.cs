using TMPro;

using UnityEngine;

namespace DefaultNamespace
{
    public class DistanceShowView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _tmpText;

        public void SetText(string text)
        {
            _tmpText.text = text;
        }
    }
}