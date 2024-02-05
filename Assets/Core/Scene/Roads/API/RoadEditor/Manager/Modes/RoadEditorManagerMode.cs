using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager.Modes
{
    public abstract class RoadEditorManagerMode
    {
        /// <summary>
        /// logic for when switching out of mode
        /// </summary>
        public abstract void ExitMode();

        /// <summary>
        /// handles logic for when raycast hit nothing or over ui
        /// </summary>
        public virtual void ExecuteNullObjectLogic()
        {
        }

        /// <summary>
        /// handles logic for when raycast hit terrain
        /// </summary>
        /// <param name="terrain">the terrain hit by raycasting</param>
        /// <param name="hitPoint">point at which raycast hit object</param>
        public virtual void ExecuteTerrainObjectLogic(Terrain terrain, Vector3 hitPoint)
        {
        }

        /// <summary>
        /// handles logic for when raycast hit junction
        /// </summary>
        /// <param name="junction">the junction hit by raycasting</param>
        /// <param name="hitPoint">point at which raycast hit object</param>
        public virtual void ExecuteJunctionObjectLogic(JunctionLogic junction, Vector3 hitPoint)
        {
        }

        /// <summary>
        /// handles logic for when raycast hit road
        /// </summary>
        /// <param name="road">the road hit by raycasting</param>
        /// <param name="hitPoint">point at which raycast hit object</param>
        public virtual void ExecuteRoadObjectLogic(RoadLogic road, Vector3 hitPoint)
        {
        }
    }
}
