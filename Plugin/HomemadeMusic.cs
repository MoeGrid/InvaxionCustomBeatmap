using UnityEngine;

namespace InvaxionCustomSpectrumPlugin
{
    class HomemadeMusic : MonoBehaviour
    {
        private void Start()
        {
            
        }
        
        private void OnGUI()
        {
            ShowWatermark();
        }

        private void Update()
        {
        }

        private void ShowWatermark()
        {
            string text = "音灵 - 自制谱面";
            Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(Screen.width - vector.x - 10f, Screen.height - vector.y - 10f, vector.x, vector.y), text);
        }
    }
}