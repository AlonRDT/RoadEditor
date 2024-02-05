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
                    ReferenceManager.RoadEditorManager.SelectJunction(m_Roads[m_Roads.Count - 1].JunctionTwo);
                }

            }
        }

        public RoadLogic GetRoad(JunctionLogic junctionOne, JunctionLogic junctionTwo)
        {
            RoadLogic output = null;

            output = m_Roads.Find(r => (r.JunctionOne == junctionOne && r.JunctionTwo == junctionTwo) ||
                (r.JunctionOne == junctionTwo && r.JunctionTwo == junctionOne));

            return output;
        }

        public void AddRoad(RoadLogic road)
        {
            m_Roads.Add(road);
        }

        public void RemoveRoad(RoadLogic road)
        {
            m_Roads.Remove(road);
        }

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
    }
}