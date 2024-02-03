using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities.API.Data;

namespace Scene.Roads.API.Factory
{
    public static class RoadFactory
    {
        private static string m_RoadNodeBuiltJunctionPrefabAdress = "Prefabs/Road Node Built Junction";
        private static GameObject m_RoadNodeBuiltJunctionPrefab;

        private static string m_RoadNodeBuiltPrefabAdress = "Prefabs/Road Node Built Prefab";
        private static GameObject m_RoadNodeBuiltPrefab;

        private static string m_RoadNodeUnderConstructionAdress = "Prefabs/Road Node Under Construction Prefab";
        private static GameObject m_RoadNodeUnderConstructionPrefab;

        static RoadFactory()
        {
            m_RoadNodeBuiltJunctionPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeBuiltJunctionPrefabAdress);
            m_RoadNodeBuiltPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeBuiltPrefabAdress);
            m_RoadNodeUnderConstructionPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeUnderConstructionAdress);
        }

        public static void ConstructJunction(Vector3 position)
        {
            GameObject.Instantiate(m_RoadNodeBuiltJunctionPrefab, position, Quaternion.identity);
        }

        public static GameObject ConstructSectionUnderConstruction()
        {
            return GameObject.Instantiate(m_RoadNodeUnderConstructionPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
