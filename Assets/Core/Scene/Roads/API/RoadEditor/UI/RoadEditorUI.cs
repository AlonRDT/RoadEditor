using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Management.API;

namespace Scene.Roads.API.RoadEditor.UI
{
    public class RoadEditorUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_Text;

        public void UpdateText(string text)
        {
            m_Text.gameObject.SetActive(true);
            m_Text.text = text;
            m_Text.transform.position = Input.mousePosition - new Vector3(10, 0, 0);
        }

        public void HideText()
        {
            m_Text.gameObject.SetActive(false);
        }

        public void Save()
        {
            ReferenceManager.RoadsDatabse.Save();
        }

        public void Load()
        {
            ReferenceManager.RoadsDatabse.Load();
        }
    }
}
