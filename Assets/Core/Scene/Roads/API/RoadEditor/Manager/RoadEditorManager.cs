using Management.API;
using Scene.Roads.API.Factory;
using Scene.Roads.API.RoadEditor.Manager.Modes;
using Scene.Roads.API.RoadEditor.RoadIllustration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.API.Map;

namespace Scene.Roads.API.RoadEditor.Manager
{
    public class RoadEditorManager : RoadEditorManager_Base
    {
        private Vector3 m_FirstJunctionLocation = new Vector3(250, 0, -200); // if no prior data spawn first junction at this point 
        private RoadBuildingMode m_RoadBuildingMode;
        private RoadDestroyerMode m_RoadDestroyerMode;
        private RoadEditorManagerMode m_CurrentMode = null;

        public override bool Init()
        {
            bool output = true;

            m_RoadBuildingMode = new RoadBuildingMode();
            m_RoadDestroyerMode = new RoadDestroyerMode();

            return output;
        }

        public override void StartRoadEdit()
        {
            SpawnFirstJunction();
        }

        public void EnterRoadBuildingMode(JunctionLogic junction)
        {
            if(m_CurrentMode != null)
            {
                m_CurrentMode.ExitMode();
            }

            m_RoadBuildingMode.EnterMode(junction);
            m_CurrentMode = m_RoadBuildingMode;
        }

        public void EnterRoadDestroyerMode(RoadLogic road)
        {
            if (m_CurrentMode != null)
            {
                m_CurrentMode.ExitMode();
            }

            m_RoadDestroyerMode.EnterMode(road);
            m_CurrentMode = m_RoadDestroyerMode;
        }

        private void Update()
        {
            if (m_CurrentMode != null)
            {
                bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
                Vector3 hitPoint;
                GameObject targetHit = getObjectOnTile(out hitPoint);

                bool eventResolved = false;

                if (targetHit == null || pointerOverUI == true)
                {
                    m_CurrentMode.ExecuteNullObjectLogic();
                    eventResolved = true;
                }

                if (eventResolved == false)
                {
                    eventResolved = handleTerrain(targetHit, hitPoint);
                }

                if (eventResolved == false)
                {
                    eventResolved = handleJunction(targetHit, hitPoint);
                }

                if (eventResolved == false)
                {
                    eventResolved = handleRoad(targetHit, hitPoint);
                }
            }
        }

        /// <summary>
        /// checks if object is of type terrain and if it handles path illustration and spawn on user input
        /// </summary>
        /// <param name="target">Object suspected of being terrain</param>
        /// <param name="hitPoint">Point raycast hit object</param>
        /// <returns>returns false if object does not hold terrain component</returns>
        private bool handleRoad(GameObject target, Vector3 hitPoint)
        {
            bool output = false;

            RoadLogic road = target.GetComponentInParent<RoadLogic>();
            if (road != null)
            {
                output = true;

                int[] squareIndex = MapLocation.GetSquareIndexFromPosition(hitPoint);

                if (road.JunctionOne.IsJuctionIndex(squareIndex) == true)
                {
                    m_CurrentMode.ExecuteJunctionObjectLogic(road.JunctionOne, hitPoint);
                }
                else if (road.JunctionTwo.IsJuctionIndex(squareIndex) == true)
                {
                    m_CurrentMode.ExecuteJunctionObjectLogic(road.JunctionTwo, hitPoint);
                }
                else
                {
                    m_CurrentMode.ExecuteRoadObjectLogic(road, hitPoint);
                }

            }

            return output;
        }

        /// <summary>
        /// checks if object is of type junction logic and if it handles path illustration and user input
        /// </summary>
        /// <param name="target">Object suspected of being junction</param>
        /// <returns>returns false if object does not hold junction component</returns>
        private bool handleJunction(GameObject target, Vector3 hitPoint)
        {
            bool output = false;

            JunctionLogic junction = target.GetComponentInParent<JunctionLogic>();
            if (junction != null)
            {
                output = true;

                m_CurrentMode.ExecuteJunctionObjectLogic(junction, hitPoint);
            }

            return output;
        }

        /// <summary>
        /// checks if object is of type terrain and if it handles path illustration and spawn on user input
        /// </summary>
        /// <param name="target">Object suspected of being terrain</param>
        /// <param name="hitPoint">Point raycast hit object</param>
        /// <returns>returns false if object does not hold terrain component</returns>
        private bool handleTerrain(GameObject target, Vector3 hitPoint)
        {
            bool output = false;

            Terrain terrain = target.GetComponentInParent<Terrain>();
            if (terrain != null)
            {
                output = true;

                m_CurrentMode.ExecuteTerrainObjectLogic(terrain, hitPoint);
            }

            return output;
        }

        /// <summary>
        /// raycasts from mouse to field and returns approriate result
        /// </summary>
        /// <param name="hitPoint">The point the ray cast hit the object is saved here</param>
        /// <returns>returns null when off terrain, if terrain square has an object on it returns the object otherwise returns the terrain</returns>
        private GameObject getObjectOnTile(out Vector3 hitPoint)
        {
            GameObject output = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) == true)
            {
                output = hit.collider.gameObject;
                hitPoint = hit.point;

                // check if there is already a road or junction on square
                if (hit.collider.GetComponentInParent<Terrain>() != null)
                {
                    Vector3 boxCenter = MapLocation.GetSquareCenterFromPosition(hit.point);
                    float boxDimensionSize = MapLocation.SquareSize / 2;
                    boxCenter.y += boxDimensionSize * 2 + 5;
                    Vector3 boxHalfExtents = new Vector3(boxDimensionSize, boxDimensionSize, boxDimensionSize);
                    Vector3 boxDirection = Vector3.down;
                    RaycastHit boxHit;

                    // does not recognize objects already overlapping at the start so need to start from high altitude
                    if (Physics.BoxCast(boxCenter, boxHalfExtents, boxDirection, out boxHit, Quaternion.identity, 5) == true)
                    {
                        output = boxHit.collider.gameObject;
                    }
                }
            }
            else
            {
                hitPoint = Vector3.zero;
            }

            return output;
        }

        /// <summary>
        /// Destroy the selected junction, used mostly when if there are no roads and we are loading a roads preset
        /// </summary>
        public void DestroySelectedJunction()
        {
            m_RoadBuildingMode.DestroySelectedJunction();
        }

        /// <summary>
        /// Spawns the first junctionwhen no pre existing roads
        /// </summary>
        public void SpawnFirstJunction()
        {
            EnterRoadBuildingMode(RoadFactory.ConstructJunction(MapLocation.GetSquareIndexFromPosition(m_FirstJunctionLocation)));
        }

        /// <summary>
        /// deletes selected section if one is selected
        /// </summary>
        public void DeleteSection()
        {
            m_RoadDestroyerMode.DestroyRoad();
        }
    }
}
