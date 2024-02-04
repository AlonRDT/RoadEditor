using Scene.Roads.API.RoadEditor.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.API
{
    public class ReferenceManager : MonoBehaviour
    {
        private static ReferenceManager m_Instance;

        [SerializeField] private RoadEditorManager m_RoadEditorManager;
        public static RoadEditorManager RoadEditorManager
        {
            get
            {
                if(m_Instance != null)
                {
                    return m_Instance.m_RoadEditorManager;
                }
                else
                {
                    return null;
                }
            }
        }

        //important! if other objects that are already in scene use refenrece maanger on awake it might not be there
        //it is wise to avoid changing script execution
        //use on enable and start for guranteed success
        private void Awake()
        {
            m_Instance = this;
        }

        private void OnDestroy()
        {
            m_Instance = null;
        }
    }
}
