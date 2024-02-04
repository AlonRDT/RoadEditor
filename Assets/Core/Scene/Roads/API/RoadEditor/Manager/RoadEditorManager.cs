using Scene.Roads.API.Factory;
using Scene.Roads.API.RoadEditor.RoadIllustration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager
{
    public class RoadEditorManager : RoadEditorManager_Base
    {
        private Vector3 m_CurrentJunctionLocation = new Vector3(250, 0, -200);
        private RoadIllustrationLogic m_RoadIllustration = null;

        public override bool Init()
        {
            bool output = true;

            RoadFactory.ConstructJunction(m_CurrentJunctionLocation);
            m_RoadIllustration = RoadFactory.ConstructRoadIllustration(m_CurrentJunctionLocation);

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
                    m_RoadIllustration.UpdateVisual(hit.point);
                }
            }
        }
    }
}
