using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Roads.API.RoadEditor.Manager.Modes
{
    public abstract class RoadEditorManagerMode
    {
        public virtual void ExecuteNullObjectLogic()
        {
        }

        public virtual void ExecuteTerrainObjectLogic()
        {
        }

        public virtual void ExecuteJunctionObjectLogic()
        {
        }

        public virtual void ExecuteRoadObjectLogic()
        {
        }
    }
}
