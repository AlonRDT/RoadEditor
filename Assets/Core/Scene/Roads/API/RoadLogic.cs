using Management.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API
{
    public class RoadLogic : MonoBehaviour
    {
        private JunctionLogic m_JunctionOne; // junction at one end of road
        public JunctionLogic JunctionOne => m_JunctionOne;
        private JunctionLogic m_JunctionTwo; // junction at other end of road
        public JunctionLogic JunctionTwo => m_JunctionTwo;

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
        }

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
    }
}
