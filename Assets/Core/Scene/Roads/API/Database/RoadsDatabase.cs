using Management.API;
using Scene.Roads.API.Factory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.API.Data;

namespace Scene.Roads.API.Database
{
    public class RoadsDatabase : MonoBehaviour
    {
        [Serializable]
        public class RoadData
        {
            public int JunctionOneIndexX;
            public int JunctionOneIndexY;
            public int JunctionTwoIndexX;
            public int JunctionTwoIndexY;

            public RoadData(RoadLogic road)
            {
                JunctionOneIndexX = road.JunctionOne.SquareIndex[0];
                JunctionOneIndexY = road.JunctionOne.SquareIndex[1];
                JunctionTwoIndexX = road.JunctionTwo.SquareIndex[0];
                JunctionTwoIndexY = road.JunctionTwo.SquareIndex[1];
            }
        }

        // ToDo: import a decent json library and get rid of this class
        [Serializable]
        public class RoadsData
        {
            public List<RoadData> Roads = new List<RoadData>();
        }

        private List<RoadLogic> m_Roads = new List<RoadLogic>();

        private readonly string m_RoadPresetsAddress = Application.streamingAssetsPath + "/RoadPresets/";

        /// <summary>
        /// saves alll current raods in scene into test.json inside streaming assets
        /// </summary>
        public void Save()
        {
            RoadsData data = new RoadsData();

            foreach (RoadLogic road in m_Roads)
            {
                data.Roads.Add(new RoadData(road));
            }

            string text = JsonUtility.ToJson(data);

            DataSaver.SaveTextToFile(m_RoadPresetsAddress, "test.json", text);
        }

        /// <summary>
        /// Loads a roads preset if one exists, first destroys all existing sections and afterwards spawns new ones from test.json in streaming assets
        /// </summary>
        public void Load()
        {
            string json = DataLoader.ReadTextualFile(m_RoadPresetsAddress + "test.json");

            if (json != null)
            {
                // if no roads were constructed than first junction will not be destroyed since junctions are destroyed inside RoadLogic onDestroy
                if (m_Roads.Count == 0)
                {
                    ReferenceManager.RoadEditorManager.DestroySelectedJunction();
                }

                for (int i = 0; i < m_Roads.Count; i++)
                {
                    Destroy(m_Roads[i].gameObject);
                }
                m_Roads.Clear();

                RoadsData roadsData = JsonUtility.FromJson<RoadsData>(json);
                List<JunctionLogic> junctions = new List<JunctionLogic>();

                foreach (RoadData data in roadsData.Roads)
                {
                    int[] junctionOneIndex = new int[] { data.JunctionOneIndexX, data.JunctionOneIndexY };
                    JunctionLogic junctionOne = junctions.Find(j => j.IsJuctionIndex(junctionOneIndex) == true);
                    if (junctionOne == null)
                    {
                        junctionOne = RoadFactory.ConstructJunction(junctionOneIndex);
                        junctions.Add(junctionOne);
                    }

                    int[] junctionTwoIndex = new int[] { data.JunctionTwoIndexX, data.JunctionTwoIndexY };
                    JunctionLogic junctionTwo = junctions.Find(j => j.IsJuctionIndex(junctionTwoIndex) == true);
                    if (junctionTwo == null)
                    {
                        junctionTwo = RoadFactory.ConstructJunction(junctionTwoIndex);
                        junctions.Add(junctionTwo);
                    }

                    RoadFactory.ConstructRoad(junctionTwo, junctionOne);
                }

                if (m_Roads.Count == 0)
                {
                    ReferenceManager.RoadEditorManager.SpawnFirstJunction();
                }
                else
                {
                    ReferenceManager.RoadEditorManager.EnterRoadBuildingMode(m_Roads[m_Roads.Count - 1].JunctionTwo);
                }

            }
        }

        /// <summary>
        /// returns a section if one exists between two given junctions, used mostly to not create a section twice
        /// </summary>
        /// <param name="junctionOne">junction at one end of section in question</param>
        /// <param name="junctionTwo">junction at other end of section in question</param>
        /// <returns>returns an existing section connecting these two junctions if one exitst, otherwise null</returns>
        public RoadLogic GetRoad(JunctionLogic junctionOne, JunctionLogic junctionTwo)
        {
            RoadLogic output = null;

            output = m_Roads.Find(r => (r.JunctionOne == junctionOne && r.JunctionTwo == junctionTwo) ||
                (r.JunctionOne == junctionTwo && r.JunctionTwo == junctionOne));

            return output;
        }

        /// <summary>
        /// adds a road to the database
        /// </summary>
        /// <param name="road">new road</param>
        public void AddRoad(RoadLogic road)
        {
            m_Roads.Add(road);
        }

        /// <summary>
        /// removes a road from the database
        /// </summary>
        /// <param name="road">road to remove</param>
        public void RemoveRoad(RoadLogic road)
        {
            m_Roads.Remove(road);
        }

        /// <summary>
        /// Determine whether a junction is still connected to any roads, used after deleting a section 
        /// </summary>
        /// <param name="junction">The junction we wanna know if still connected</param>
        /// <returns>wether there are still any sections connected to junction</returns>
        public bool IsJunctionDisconnected(JunctionLogic junction)
        {
            bool output = true;

            foreach (RoadLogic road in m_Roads)
            {
                if (road.JunctionOne == junction || road.JunctionTwo == junction)
                {
                    output = false; 
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// When deleting a road section get the section that was created before as current section
        /// </summary>
        /// <param name="road">The section about to be deleted</param>
        /// <returns>The Section that will become current active section for next deletion, if there is one</returns>
        public RoadLogic GetPreviousRoad(RoadLogic road)
        {
            RoadLogic output = null;

            if(m_Roads.Count > 1)
            {
                int index = m_Roads.FindIndex(r => r == road);
                if (index == 0)
                {
                    output = m_Roads[m_Roads.Count - 1];
                }
                else
                {
                    output = m_Roads[index - 1];
                }
            }

            return output;
        }

        /// <summary>
        /// I was asked to create this method in excersize demands
        /// </summary>
        public int GetNumOfSections()
        {
            return m_Roads.Count;
        }
    }
}