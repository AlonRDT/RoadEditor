using Management.API;
using Scene.Roads.API.Factory;
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
        private RoadIllustrationLogic m_RoadIllustration = null; // the visual indication of how the new road will be placed

        public override bool Init()
        {
            bool output = true;

            m_RoadIllustration = RoadFactory.ConstructRoadIllustration(m_FirstJunctionLocation);
            SpawnFirstJunction();

            return output;
        }

        public override void StartRoadEdit()
        {

        }

        private void Update()
        {
            if (m_RoadIllustration != null)
            {
                bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
                Vector3 hitPoint;
                GameObject targetHit = getObjectOnTile(out hitPoint);

                bool eventResolved = false;

                if (targetHit == null || pointerOverUI == true)
                {
                    eventResolved = true;
                    m_RoadIllustration.HideRoadVisual();
                    m_RoadIllustration.HideEndJunctionVisual();
                    ReferenceManager.RoadEditorUI.HideText();
                }

                if (eventResolved == false)
                {
                    eventResolved = handleTerrain(targetHit, hitPoint);
                }

                if (eventResolved == false)
                {
                    eventResolved = handleJunction(targetHit);
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
                    executeJunctionTargetedLogic(road.JunctionOne);
                }
                else if (road.JunctionTwo.IsJuctionIndex(squareIndex) == true)
                {
                    executeJunctionTargetedLogic(road.JunctionTwo);
                }
                else
                {
                    bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hitPoint));

                    if (Input.GetMouseButtonDown(0) == true)
                    {
                        JunctionLogic newJunction = RoadFactory.ConstructJunction(squareIndex);
                        RoadFactory.ConstructRoad(road.JunctionOne, newJunction);
                        RoadFactory.ConstructRoad(newJunction, road.JunctionTwo);
                        SpawnRoad(newJunction);
                        Destroy(road.gameObject);
                    }

                    if (Input.GetMouseButtonDown(1) == true)
                    {

                    }
                }

            }

            return output;
        }

        /// <summary>
        /// checks if object is of type junction logic and if it handles path illustration and user input
        /// </summary>
        /// <param name="target">Object suspected of being junction</param>
        /// <returns>returns false if object does not hold junction component</returns>
        private bool handleJunction(GameObject target)
        {
            bool output = false;

            JunctionLogic junction = target.GetComponentInParent<JunctionLogic>();
            if (junction != null)
            {
                output = true;

                if (junction != m_RoadIllustration.SelectedJunction)
                {
                    executeJunctionTargetedLogic(junction);
                }
                else
                {
                    m_RoadIllustration.HideEndJunctionVisual();
                    m_RoadIllustration.HideRoadVisual();
                }
            }

            return output;
        }

        /// <summary>
        /// handles logic for when raycast hit junction
        /// </summary>
        /// <param name="junction">the junction which will either be set as starting point or will be joined by a road depending on user input</param>
        private void executeJunctionTargetedLogic(JunctionLogic junction)
        {
            bool isNewPathValid = updateNewRoadState(junction.SquareIndex);
            m_RoadIllustration.HideEndJunctionVisual();

            if (Input.GetMouseButtonDown(0) == true && isNewPathValid == true)
            {
                SpawnRoad(junction);
            }

            if (Input.GetMouseButtonDown(1) == true)
            {
                m_RoadIllustration.SelectJunction(junction);
            }
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

                bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hitPoint));

                if (Input.GetMouseButtonDown(0) == true && isNewPathValid)
                {
                    SpawnRoad();
                }
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
        /// Determines whether path between current selected junction and mouse is valid and updates visuals accordingly
        /// </summary>
        /// <param name="squareIndex">Index of terrain square mouse is above</param>
        /// <returns>returns whether new road is valid</returns>
        private bool updateNewRoadState(int[] squareIndex)
        {
            bool output = false;

            if (m_RoadIllustration.SelectedJunction.IsJuctionIndex(squareIndex) == false)
            {
                Vector3 JunctionPosition = m_RoadIllustration.SelectedJunction.transform.position;
                Vector3 targetPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex);

                while (Vector3.Distance(JunctionPosition, targetPosition) > MaxRoadDistance)
                {
                    squareIndex = MapLocation.MoveMapIndexTowardsTargetMapIndex(squareIndex, m_RoadIllustration.SelectedJunction.SquareIndex);
                    targetPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex);
                }

                if (targetPosition.y - JunctionPosition.y > MaxHeightDif)
                {
                    ReferenceManager.RoadEditorUI.UpdateText("No Access");
                }
                else
                {
                    ReferenceManager.RoadEditorUI.UpdateText(HeightCostAdd.ToString());
                    output = true;
                }

                m_RoadIllustration.UpdateVisual(MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex));
            }
            else
            {
                m_RoadIllustration.HideEndJunctionVisual();
                m_RoadIllustration.HideRoadVisual();
            }

            return output;
        }

        /// <summary>
        /// Transforms road illustration into a real road
        /// </summary>
        /// <param name="endPoint">If connected to an existing junction, if not will spawn a new one</param>
        private void SpawnRoad(JunctionLogic endPoint = null)
        {
            if (endPoint == null)
            {
                endPoint = RoadFactory.ConstructJunction(m_RoadIllustration.GetEndPointSuqareIndex());
            }

            RoadLogic newRoad = RoadFactory.ConstructRoad(m_RoadIllustration.SelectedJunction, endPoint);

            m_RoadIllustration.SelectJunction(endPoint);
        }

        /// <summary>
        /// Destroy the selected junction, used mostly when if there are no roads and we are loading a roads preset
        /// </summary>
        public void DestroySelectedJunction()
        {
            Destroy(m_RoadIllustration.SelectedJunction.gameObject);
        }

        /// <summary>
        /// Spawns the first junctionwhen no pre existing roads
        /// </summary>
        public void SpawnFirstJunction()
        {
            m_RoadIllustration.SelectJunction(RoadFactory.ConstructJunction(MapLocation.GetSquareIndexFromPosition(m_FirstJunctionLocation)));
        }

        /// <summary>
        /// Selects target junction for road illustration
        /// </summary>
        public void SelectJunction(JunctionLogic junction)
        {
            m_RoadIllustration.SelectJunction(junction);
        }
    }
}
