using Management.API;
using Scene.Roads.API.Factory;
using Scene.Roads.API.RoadEditor.RoadIllustration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            m_RoadIllustration.SelectJunction(RoadFactory.ConstructJunction(MapLocation.GetSquareIndexFromPosition(m_FirstJunctionLocation)));

            return output;
        }

        public override void StartRoadEdit()
        {

        }

        private void Update()
        {
            if (m_RoadIllustration != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponentInParent<Terrain>() != null)
                    {
                        bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hit.point));

                        if (Input.GetMouseButtonDown(0) == true && isNewPathValid)
                        {
                            SpawnRoad();
                        }
                    }
                    else if(hit.collider.GetComponentInParent<JunctionLogic>() != null)
                    {
                        m_RoadIllustration.HideVisual();
                        ReferenceManager.RoadEditorUI.HideText();
                    }
                }

                else
                {
                    m_RoadIllustration.HideVisual();
                    ReferenceManager.RoadEditorUI.HideText();
                }
            }
        }

        /// <summary>
        /// Determines whether path between current selected junction and mouse is valid and updates visuals accordingly
        /// </summary>
        /// <param name="squareIndex">Index of terrain square mouse is above</param>
        /// <returns>returns whether new road is valid</returns>
        private bool updateNewRoadState(int[] squareIndex)
        {
            bool output = false;

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

            return output;
        }

        /// <summary>
        /// Transforms road illustration into a real road
        /// </summary>
        /// <param name="endPoint">If connected to an existing junction, if not will spawn a new one</param>
        private void SpawnRoad(JunctionLogic endPoint = null)
        {
            if(endPoint == null)
            {
                endPoint = RoadFactory.ConstructJunction(m_RoadIllustration.GetEndPointSuqareIndex());
            }

            RoadLogic newRoad = RoadFactory.ConstructRoad(m_RoadIllustration.SelectedJunction, endPoint);

            m_RoadIllustration.SelectJunction(endPoint);
        }
    }
}
