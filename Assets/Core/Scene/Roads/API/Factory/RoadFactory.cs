using Scene.Roads.API.RoadEditor.RoadIllustration;
using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// spawns junction at given index
        /// </summary>
        /// <param name="squareIndex">Index where to spawn new junction</param>
        /// <returns>The logic component of the new junction</returns>
        public static JunctionLogic ConstructJunction(int[] squareIndex)
        {
            GameObject newJunction = GameObject.Instantiate(m_RoadNodeBuiltJunctionPrefab, Vector3.zero, Quaternion.identity);
            newJunction.transform.SetParent(ReferenceManager.JunctionsParent);
            JunctionLogic output = newJunction.AddComponent<JunctionLogic>();
            output.Initialize(squareIndex);

            return output;
        }

        /// <summary>
        /// spawns a tube that lloks like a junction, indicates where new junction will be spawned if user calls new road spawn action
        /// </summary>
        /// <returns>returns junction shape colored differently</returns>
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

        /// <summary>
        /// Spawns a tube to be a part of the visual indication of new paths in road illustration
        /// </summary>
        /// <returns>returns new tube</returns>
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

        /// <summary>
        /// Spawns the component responsible to visualize the raod that will be spawned when user calls such action
        /// </summary>
        /// <returns>returns new road indicator</returns>
        public static RoadIllustrationLogic ConstructRoadIllustration()
        {
            RoadIllustrationLogic output = null;

            Transform newRoadIllustrationObject = new GameObject("Road Illustration").transform;
            newRoadIllustrationObject.SetParent(ReferenceManager.RoadEditorManager.transform);

            output = newRoadIllustrationObject.gameObject.AddComponent<RoadIllustrationLogic>();

            return output;
        }

        /// <summary>
        /// spawns a new road between junctions if one does not already exists between them
        /// </summary>
        /// <param name="startJunction">One end of new section</param>
        /// <param name="endJunction">Other end of new Section</param>
        /// <returns>old or new road the stretches between these junctions</returns>
        public static RoadLogic ConstructRoad(JunctionLogic startJunction, JunctionLogic endJunction)
        {
            RoadLogic output = ReferenceManager.RoadsDatabse.GetRoad(startJunction, endJunction);

            if (output == null)
            {
                GameObject newRoad = GameObject.Instantiate(m_RoadNodeBuiltPrefab);
                newRoad.transform.SetParent(ReferenceManager.RoadsParent);
                newRoad.transform.position = startJunction.transform.position;
                Vector3 roadVector = endJunction.transform.position - startJunction.transform.position;
                newRoad.transform.forward = roadVector;
                newRoad.transform.localScale = new Vector3(1, 1, roadVector.magnitude);
                output = newRoad.AddComponent<RoadLogic>();
                output.Initialize(startJunction, endJunction);
            }

            return output;
        }
    }
}
