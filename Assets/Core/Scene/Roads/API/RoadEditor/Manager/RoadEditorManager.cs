using Scene.Roads.API.Factory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager
{
    public class RoadEditorManager : RoadEditorManager_Base
    {
        private Vector3 m_CurrentJunctionLocation = new Vector3(250, 0, -200);
        private GameObject m_RoadSectionUnderConstruction = null;

        public override bool Init()
        {
            bool output = true;

            RoadFactory.ConstructJunction(m_CurrentJunctionLocation);
            m_RoadSectionUnderConstruction = RoadFactory.ConstructSectionUnderConstruction();

            return output;
        }

        public override void StartRoadEdit()
        {
            
        }

        private void Update()
        {
            if(m_RoadSectionUnderConstruction != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Position the object at the hit point
                    m_RoadSectionUnderConstruction.transform.position = hit.point;
                }
            }
        }
    }
}
