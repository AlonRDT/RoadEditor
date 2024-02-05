using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager.Modes
{
    public class RoadDestroyerMode : RoadEditorManagerMode
    {
        private RoadLogic m_CurrentTarget;

        public void EnterMode(RoadLogic road)
        {
            road.MarkAsTarget();
            m_CurrentTarget = road;
        }

        public override void ExitMode()
        {
            
        }
    }
}
