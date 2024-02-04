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
                            hit = boxHit;
                        }
                    }

                    if (hit.collider.GetComponentInParent<Terrain>() != null)
                    {
                        bool isNewPathValid = updateNewRoadState(MapLocation.GetSquareIndexFromPosition(hit.point));

                        if (Input.GetMouseButtonDown(0) == true && isNewPathValid)
                        {
                            SpawnRoad();
                        }
                    }
                    else if (hit.collider.GetComponentInParent<JunctionLogic>() != null)
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
            if (endPoint == null)
            {
                endPoint = RoadFactory.ConstructJunction(m_RoadIllustration.GetEndPointSuqareIndex());
            }

            RoadLogic newRoad = RoadFactory.ConstructRoad(m_RoadIllustration.SelectedJunction, endPoint);

            m_RoadIllustration.SelectJunction(endPoint);
        }

        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DrawBox(bottomBox, color);
            DrawBox(topBox, color);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color);
        }
        public static void DrawBox(Box box, Color color)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }
            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }
}
