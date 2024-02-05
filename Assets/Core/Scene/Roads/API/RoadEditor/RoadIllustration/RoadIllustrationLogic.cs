using Management.API;
using Scene.Roads.API.Factory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.API.Map;

namespace Scene.Roads.API.RoadEditor.RoadIllustration
{
    public class RoadIllustrationLogic : MonoBehaviour
    {
        private Transform m_RoadEndJunctionTransform; // the indicator that whows where new junction will be spawned
        private Transform m_TubesParent; // an empty object that aggregates all tubes
        private Transform[] m_TubeTransforms; // the transforms of the tubes which the path illustration consists of
        private JunctionLogic m_SelectedJunction = null;
        public JunctionLogic SelectedJunction => m_SelectedJunction;

        // visual illustration parameters
        private float m_SpaceBetweenTubes = 7f;
        private float m_TubeSize = 10f;


        // I would rather use static propeties instead of using reference manager for this information
        void Awake()
        {
            m_RoadEndJunctionTransform = RoadFactory.ConstructJunctionIllustration().transform;
            m_RoadEndJunctionTransform.SetParent(transform);

            m_TubesParent = new GameObject("Tubes Parent").transform;
            m_TubesParent.SetParent(transform);
            m_TubesParent.localPosition = Vector3.zero;

            int tubeAmount = Mathf.CeilToInt((ReferenceManager.RoadEditorManager.MaxRoadDistance - m_SpaceBetweenTubes) / (m_SpaceBetweenTubes + m_TubeSize));
            m_TubeTransforms = new Transform[tubeAmount];
            for (int i = 0; i < tubeAmount; i++)
            {
                m_TubeTransforms[i] = RoadFactory.ConstructSectionUnderConstruction().transform;
                m_TubeTransforms[i].SetParent(m_TubesParent);
                m_TubeTransforms[i].localPosition = new Vector3(0, 0, m_SpaceBetweenTubes + i * (m_SpaceBetweenTubes + m_TubeSize));
                m_TubeTransforms[i].localScale = new Vector3(1, 1, m_TubeSize);
            }

            HideRoadVisual();
            HideEndJunctionVisual();
        }

        /// <summary>
        /// Changes the junction from which new road stretches towards mouse
        /// </summary>
        /// <param name="junction">The new junction to be the first point of the new road to be spawned</param>
        public void SelectJunction(JunctionLogic junction)
        {
            m_SelectedJunction = junction;
            transform.position = m_SelectedJunction.transform.position;
            HideRoadVisual();
            HideEndJunctionVisual();
        }

        /// <summary>
        /// called from frame and makes sure to visulaize the size of a new path to be created if user calls for it
        /// </summary>
        /// <param name="mousePosition">Real world position of mouse</param>
        public void UpdateVisual(Vector3 mousePosition)
        {
            m_RoadEndJunctionTransform.position = mousePosition;
            Vector3 pathVector = mousePosition - transform.position;
            float distance = pathVector.magnitude;
            m_TubesParent.forward = pathVector;

            m_RoadEndJunctionTransform.gameObject.SetActive(true);

            int lastTubeInViewIndex = Mathf.FloorToInt((distance - m_SpaceBetweenTubes) / (m_SpaceBetweenTubes + m_TubeSize));
            for (int i = 0; i < m_TubeTransforms.Length; i++)
            {
                if(i < lastTubeInViewIndex)
                {
                    m_TubeTransforms[i].gameObject.SetActive(true);
                    m_TubeTransforms[i].localScale = new Vector3(1, 1, m_TubeSize);
                }
                else if (i == lastTubeInViewIndex)
                {
                    m_TubeTransforms[i].gameObject.SetActive(true);
                    float distanceFromLatsFullTube = distance - (m_SpaceBetweenTubes + i * (m_SpaceBetweenTubes + m_TubeSize));
                    m_TubeTransforms[i].localScale = new Vector3(1, 1, Mathf.Min(m_TubeSize, distanceFromLatsFullTube));
                }
                else
                {
                    m_TubeTransforms[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Hides the road indicator that stretches from selected junction to mouse
        /// </summary>
        public void HideRoadVisual()
        {
            foreach (Transform t in m_TubeTransforms)
            {
                t.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Hides the new junction visual, usually when hovering above an existing juncture
        /// </summary>
        public void HideEndJunctionVisual()
        {
            m_RoadEndJunctionTransform.gameObject.SetActive(false);
        }

        /// <summary>
        /// get the index of the point where mouse indicates
        /// </summary>
        /// <returns>returns two dimenstional index of other end of path taht should be spawned</returns>
        public int[] GetEndPointSuqareIndex()
        {
            return MapLocation.GetSquareIndexFromPosition(m_RoadEndJunctionTransform.position);
        }
    }
}
