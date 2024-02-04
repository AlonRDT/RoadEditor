using Scene.Roads.API.RoadEditor.RoadIllustration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities.API.Data;
using Management.API;

namespace Scene.Roads.API.Factory
{
    public static class RoadFactory
    {
        private static string m_RoadNodeBuiltJunctionPrefabAdress = "Prefabs/Road Node Built Junction";
        private static GameObject m_RoadNodeBuiltJunctionPrefab;

        private static string m_RoadNodeBuiltPrefabAdress = "Prefabs/Road Node Built Prefab";
        private static GameObject m_RoadNodeBuiltPrefab;

        private static string m_RoadNodeUnderConstructionPrefabAdress = "Prefabs/Road Node Under Construction Prefab";
        private static GameObject m_RoadNodeUnderConstructionPrefab;

        private static string m_UnlitYellowMaterialAdress = "Materials/Unlit-Yellow";
        private static Material m_UnlitYellowMaterial;

        private static string m_UnlitOrangeMaterialAdress = "Materials/Unlit-Orange";
        private static Material m_UnlitOrangeMaterial;

        static RoadFactory()
        {
            m_RoadNodeBuiltJunctionPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeBuiltJunctionPrefabAdress);
            m_RoadNodeBuiltPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeBuiltPrefabAdress);
            m_RoadNodeUnderConstructionPrefab = DataLoader.LoadFromResources<GameObject>(m_RoadNodeUnderConstructionPrefabAdress);
            m_UnlitYellowMaterial = DataLoader.LoadFromResources<Material>(m_UnlitYellowMaterialAdress);
            m_UnlitOrangeMaterial = DataLoader.LoadFromResources<Material>(m_UnlitOrangeMaterialAdress);
        }

        // Junctions
        public static JunctionLogic ConstructJunction(int[] squareIndex)
        {
            GameObject newJunction = GameObject.Instantiate(m_RoadNodeBuiltJunctionPrefab, Vector3.zero, Quaternion.identity);
            newJunction.transform.SetParent(ReferenceManager.JunctionsParent);
            JunctionLogic output = newJunction.AddComponent<JunctionLogic>();
            output.Initialize(squareIndex);

            return output;
        }

        public static GameObject ConstructJunctionIllustration()
        {
            GameObject output = GameObject.Instantiate(m_RoadNodeBuiltJunctionPrefab, Vector3.zero, Quaternion.identity);

            // colliders interrupt with raycasting ground, there are many solutions to this problem this is one.
            Collider[] colliders = output.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject.Destroy(colliders[i]);
            }

            // set illustration color
            MeshRenderer[] meshRenderers = output.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = m_UnlitYellowMaterial;
            }

            return output;
        }

        // Roads
        public static GameObject ConstructSectionUnderConstruction()
        {
            // could have also removed from prefab itself because it is not a real game road but didnt want to change assets I was given.
            GameObject output = GameObject.Instantiate(m_RoadNodeUnderConstructionPrefab, Vector3.zero, Quaternion.identity);

            // colliders interrupt with raycasting ground, there are many solutions to this problem this is one.
            Collider[] colliders = output.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject.Destroy(colliders[i]);
            }

            // set illustration color
            MeshRenderer[] meshRenderers = output.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = m_UnlitOrangeMaterial;
            }

            return output;
        }

        public static RoadIllustrationLogic ConstructRoadIllustration(Vector3 position)
        {
            RoadIllustrationLogic output = null;

            Transform newRoadIllustrationObject = new GameObject("Road Illustration").transform;
            newRoadIllustrationObject.SetParent(ReferenceManager.RoadEditorManager.transform);
            newRoadIllustrationObject.transform.position = position;

            output = newRoadIllustrationObject.gameObject.AddComponent<RoadIllustrationLogic>();

            return output;
        }

        public static RoadLogic ConstructRoad(JunctionLogic startJunction, JunctionLogic endJunction)
        {
            GameObject newRoad = GameObject.Instantiate(m_RoadNodeBuiltPrefab);
            newRoad.transform.SetParent(ReferenceManager.RoadsParent);
            newRoad.transform.position = startJunction.transform.position;
            Vector3 roadVector = endJunction.transform.position - startJunction.transform.position;
            newRoad.transform.forward = roadVector;
            newRoad.transform.localScale = new Vector3(1, 1, roadVector.magnitude);
            RoadLogic output = newRoad.AddComponent<RoadLogic>();

            return output;
        }
    }
}
