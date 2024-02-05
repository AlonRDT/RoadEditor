using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Management.API;
using UnityEngine.UI;

namespace Scene.Roads.API.RoadEditor.UI
{
    public class RoadEditorUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_Text;
        [SerializeField] private Button m_DeleteSectionButton;

        /// <summary>
        /// Shows text on the left side of the mouse
        /// </summary>
        /// <param name="text">Text to present on screen</param>
        public void UpdateText(string text)
        {
            m_Text.gameObject.SetActive(true);
            m_Text.text = text;
            m_Text.transform.position = Input.mousePosition - new Vector3(10, 0, 0);
        }

        /// <summary>
        /// Hides the text that apears next to mouse when road building mode is active
        /// </summary>
        public void HideText()
        {
            m_Text.gameObject.SetActive(false);
        }

        /// <summary>
        /// Saves the roads now in scene to file
        /// </summary>
        public void Save()
        {
            ReferenceManager.RoadsDatabse.Save();
        }

        /// <summary>
        /// Loads last saved roads from file, if file exists
        /// </summary>
        public void Load()
        {
            ReferenceManager.RoadsDatabse.Load();
        }

        /// <summary>
        /// Deletes current selected section of road if one is selected in road destroyer mode
        /// </summary>
        public void DeleteSection()
        {
            ReferenceManager.RoadEditorManager.DeleteSection();
        }

        /// <summary>
        /// When not in road destroyer mode hide delete section button because no section is selected
        /// </summary>
        public void HideDeleteButton()
        {
            m_DeleteSectionButton.gameObject.SetActive(false);
        }


        /// <summary>
        /// Show delete section button, means that raod destroyer mode is active
        /// </summary>
        public void ShowDeleteButton()
        {
            m_DeleteSectionButton.gameObject.SetActive(true);
        }
    }
}
