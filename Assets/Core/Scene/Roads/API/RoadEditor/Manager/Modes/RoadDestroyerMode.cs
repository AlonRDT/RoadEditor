using Management.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager.Modes
{
    public class RoadDestroyerMode : RoadEditorManagerMode
    {
        private RoadLogic m_CurrentTarget;

        public RoadDestroyerMode()
        {
            ReferenceManager.RoadEditorUI.HideDeleteButton();
        }

        public void EnterMode(RoadLogic road)
        {
            road.MarkAsTarget();
            m_CurrentTarget = road;
            ReferenceManager.RoadEditorUI.ShowDeleteButton();
        }

        public override void ExitMode()
        {
            if (m_CurrentTarget != null)
            {
                m_CurrentTarget.MarkAsNormal();
                m_CurrentTarget = null;
            }

            ReferenceManager.RoadEditorUI.HideDeleteButton();
        }

        public void DestroyRoad()
        {
            if (m_CurrentTarget != null)
            {
                RoadLogic newTarget = ReferenceManager.RoadsDatabse.GetPreviousRoad(m_CurrentTarget);

                GameObject.Destroy(m_CurrentTarget.gameObject);

                if (newTarget == null)
                {
                    ReferenceManager.RoadEditorManager.SpawnFirstJunction();
                }
                else
                {
                    m_CurrentTarget = newTarget;
                    m_CurrentTarget.MarkAsTarget();
                }
            }
        }

        public override void ExecuteRoadObjectLogic(RoadLogic road, Vector3 hitPoint)
        {
            base.ExecuteRoadObjectLogic(road, hitPoint);

            if (Input.GetMouseButtonDown(1) == true)
            {
                if (m_CurrentTarget != road)
                {
                    m_CurrentTarget.MarkAsNormal();
                    m_CurrentTarget = road;
                    m_CurrentTarget.MarkAsTarget();
                }
            }
        }

        public override void ExecuteJunctionObjectLogic(JunctionLogic junction, Vector3 hitPoint)
        {
            base.ExecuteJunctionObjectLogic(junction, hitPoint);

            if (Input.GetMouseButtonDown(1) == true)
            {
                ReferenceManager.RoadEditorManager.EnterRoadBuildingMode(junction);
            }
        }
    }
}
