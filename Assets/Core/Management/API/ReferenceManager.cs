using Scene.Roads.API.Database;
using Scene.Roads.API.RoadEditor.Manager;
using Scene.Roads.API.RoadEditor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.API
{
    public class ReferenceManager : MonoBehaviour
    {
        private static ReferenceManager m_Instance;

        // if I had more time I would create a system where similar objects are kept in lists and are pulled out by enum values, ui componenets, managers, scene parents etc...
        [SerializeField] private RoadEditorManager m_RoadEditorManager;
        public static RoadEditorManager RoadEditorManager
        {
            get => m_Instance != null ? m_Instance.m_RoadEditorManager : null;
        }

        [SerializeField] private RoadEditorUI m_RoadEditorUI;
        public static RoadEditorUI RoadEditorUI
        {
            get => m_Instance != null ? m_Instance.m_RoadEditorUI : null;
        }

        [SerializeField] private Transform m_JunctionsParent;
        public static Transform JunctionsParent
        {
            get => m_Instance != null ? m_Instance.m_JunctionsParent : null;
        }

        [SerializeField] private Transform m_RoadsParent;
        public static Transform RoadsParent
        {
            get => m_Instance != null ? m_Instance.m_RoadsParent : null;
        }

        [SerializeField] private RoadsDatabase m_RoadsDatabse;
        public static RoadsDatabase RoadsDatabse
        {
            get => m_Instance != null ? m_Instance.m_RoadsDatabse : null;
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
