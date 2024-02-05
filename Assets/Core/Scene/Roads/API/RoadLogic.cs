using Management.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.API.Data;

namespace Scene.Roads.API
{
    public class RoadLogic : MonoBehaviour
    {
        private JunctionLogic m_JunctionOne; // junction at one end of road
        public JunctionLogic JunctionOne => m_JunctionOne;
        private JunctionLogic m_JunctionTwo; // junction at other end of road
        public JunctionLogic JunctionTwo => m_JunctionTwo;

        private static string m_UnlitYellowMaterialAdress = "Materials/Unlit-Yellow";
        private static Material m_TargetMaterial; // color indicating its current target for destruction

        private static string m_UnlitGreenMaterialAdress = "Materials/Unlit-Green";
        private static Material m_NormalMaterial; // normal color, has no meaning

        /// <summary>
        /// after spawn initialize position and data
        /// </summary>
        /// <param name="junctionOne">junction at one end of the road</param>
        /// <param name="junctionTwo">junction at second end of the road</param>
        /// <returns>returns false if object does not hold terrain component</returns>
        public void Initialize(JunctionLogic junctionOne, JunctionLogic junctionTwo)
        {
            m_JunctionOne = junctionOne;
            m_JunctionTwo = junctionTwo;
            ReferenceManager.RoadsDatabse.AddRoad(this);
            m_TargetMaterial = DataLoader.LoadFromResources<Material>(m_UnlitYellowMaterialAdress);
            m_NormalMaterial = DataLoader.LoadFromResources<Material>(m_UnlitGreenMaterialAdress);
        }

        /// <summary>
        /// when object is being destroyed it removes itself from database and deletes junctions that were left with no roads connected
        /// </summary>
        private void OnDestroy()
        {
            if(ReferenceManager.RoadsDatabse != null)
            {
                ReferenceManager.RoadsDatabse.RemoveRoad(this);

                if(ReferenceManager.RoadsDatabse.IsJunctionDisconnected(m_JunctionOne) == true)
                {
                    Destroy(m_JunctionOne.gameObject);
                }

                if (ReferenceManager.RoadsDatabse.IsJunctionDisconnected(m_JunctionTwo) == true)
                {
                    Destroy(m_JunctionTwo.gameObject);
                }
            }
        }

        /// <summary>
        /// change color to indicate this section will be destroyed if delete section is used
        /// </summary>
        public void MarkAsTarget()
        {
            GetComponentInChildren<MeshRenderer>().material = m_TargetMaterial;
        }

        /// <summary>
        /// change color to normal after it is no longer a target
        /// </summary>
        public void MarkAsNormal()
        {
            GetComponentInChildren<MeshRenderer>().material = m_NormalMaterial;
        }
    }
}
