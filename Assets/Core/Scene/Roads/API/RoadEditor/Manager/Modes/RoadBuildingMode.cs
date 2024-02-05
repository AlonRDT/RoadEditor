using Management.API;
using Scene.Roads.API.Factory;
using Scene.Roads.API.RoadEditor.RoadIllustration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.API.Map;

namespace Scene.Roads.API.RoadEditor.Manager.Modes
{
    public class RoadBuildingMode : RoadEditorManagerMode
    {
        private RoadIllustrationLogic m_RoadIllustration = null; // the visual indication of how the new road will be placed

        public RoadBuildingMode()
        {
            m_RoadIllustration = RoadFactory.ConstructRoadIllustration();
            HideEverything();
        }

        public void EnterMode(JunctionLogic junction)
        {
            m_RoadIllustration.SelectJunction(junction);
        }

        public override void ExitMode()
        {
            HideEverything();
        }

        private void HideEverything()
        {
            m_RoadIllustration.HideRoadVisual();
            m_RoadIllustration.HideEndJunctionVisual();
            ReferenceManager.RoadEditorUI.HideText();
        }

        public override void ExecuteNullObjectLogic()
        {
            base.ExecuteNullObjectLogic();
            HideEverything();
        }

        public override void ExecuteTerrainObjectLogic(Terrain terrain, Vector3 hitPoint)
        {
            base.ExecuteTerrainObjectLogic(terrain, hitPoint);

            bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hitPoint));

            if (Input.GetMouseButtonDown(0) == true && isNewPathValid)
            {
                SpawnRoad();
            }
        }

        public override void ExecuteJunctionObjectLogic(JunctionLogic junction, Vector3 hitPoint)
        {
            base.ExecuteJunctionObjectLogic(junction, hitPoint);

            if (junction != m_RoadIllustration.SelectedJunction)
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
            else
            {
                m_RoadIllustration.HideEndJunctionVisual();
                m_RoadIllustration.HideRoadVisual();
            }
        }

        public override void ExecuteRoadObjectLogic(RoadLogic road, Vector3 hitPoint)
        {
            base.ExecuteRoadObjectLogic(road, hitPoint);

            int[] squareIndex = MapLocation.GetSquareIndexFromPosition(hitPoint);
            bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hitPoint));

            if (Input.GetMouseButtonDown(0) == true && isNewPathValid == true)
            {
                JunctionLogic newJunction = RoadFactory.ConstructJunction(squareIndex);
                RoadFactory.ConstructRoad(road.JunctionOne, newJunction);
                RoadFactory.ConstructRoad(newJunction, road.JunctionTwo);
                SpawnRoad(newJunction);
                GameObject.Destroy(road.gameObject);
            }

            if (Input.GetMouseButtonDown(1) == true)
            {
                ReferenceManager.RoadEditorManager.EnterRoadDestroyerMode(road);
            }
        }

        /// <summary>
        /// Destroy the selected junction, used mostly when if there are no roads and we are loading a roads preset
        /// </summary>
        public void DestroySelectedJunction()
        {
            GameObject.Destroy(m_RoadIllustration.SelectedJunction.gameObject);
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

                while (Vector3.Distance(JunctionPosition, targetPosition) > ReferenceManager.RoadEditorManager.MaxRoadDistance)
                {
                    squareIndex = MapLocation.MoveMapIndexTowardsTargetMapIndex(squareIndex, m_RoadIllustration.SelectedJunction.SquareIndex);
                    targetPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex);
                }

                if (targetPosition.y - JunctionPosition.y > ReferenceManager.RoadEditorManager.MaxHeightDif)
                {
                    ReferenceManager.RoadEditorUI.UpdateText("No Access");
                }
                else
                {
                    float cost = (Mathf.Max(0, targetPosition.y - JunctionPosition.y)) * ReferenceManager.RoadEditorManager.HeightCostAdd;
                    ReferenceManager.RoadEditorUI.UpdateText(cost.ToString("F0"));
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
    }
}