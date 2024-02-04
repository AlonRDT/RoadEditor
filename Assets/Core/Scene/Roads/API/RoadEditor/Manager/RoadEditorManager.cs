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
        private Vector3 m_FirstJunctionLocation = new Vector3(250, 0, -200);
        private int[] m_CurretnJunctionLocationIndex;
        private RoadIllustrationLogic m_RoadIllustration = null;

        public override bool Init()
        {
            bool output = true;

            RoadFactory.ConstructJunction(m_FirstJunctionLocation);
            m_CurretnJunctionLocationIndex = MapLocation.GetSquareIndexFromPosition(m_FirstJunctionLocation);

            m_RoadIllustration = RoadFactory.ConstructRoadIllustration(m_FirstJunctionLocation);

            return output;
        }

        public override void StartRoadEdit()
        {
            
        }

        private void Update()
        {
            if(m_RoadIllustration != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    int[] squareIndex = MapLocation.GetSquareIndexFromPosition(hit.point);
                    Vector3 JunctionPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(m_CurretnJunctionLocationIndex);
                    Vector3 targetPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex);

                    while (squareIndex != m_CurretnJunctionLocationIndex && Vector3.Distance(JunctionPosition, targetPosition) > MaxRoadDistance)
                    {
                        squareIndex = MapLocation.MoveMapIndexTowardsTargetMapIndex(squareIndex, m_CurretnJunctionLocationIndex);
                        targetPosition = MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex);
                    }

                    if(targetPosition.y - JunctionPosition.y > MaxHeightDif)
                    {
                        ReferenceManager.RoadEditorUI.UpdateText("No Access");
                    }
                    else
                    {
                        ReferenceManager.RoadEditorUI.UpdateText(HeightCostAdd.ToString());
                    }

                    m_RoadIllustration.UpdateVisual(MapLocation.GetWorldSquareCenterFromSquareIndex(squareIndex));
                }
                else
                {
                    m_RoadIllustration.HideVisual();
                    ReferenceManager.RoadEditorUI.HideText();
                }
            }
        }
    }
}
