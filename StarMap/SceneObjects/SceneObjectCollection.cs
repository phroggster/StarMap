using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.SceneObjects
{
    public class SceneObjectCollection : List<SceneObject>
    {
        public const int GL_MAX_VERTEX_UNIFORM_BLOCKS = 48;

        public SceneObjectCollection()
        {
            for (int i = 0; i < m_pvmdv_UBOArray.Length - 1; i++)
                m_pvmdv_UBOArray[i] = new ProjViewModelDiffuseViewport_UBOData();
        }
        
        public void Render()
        {
            if (m_LastEntryIndex >= 0)
            {

            }
        }

        public void Update(double delta)
        {
            if (m_LastEntryIndex >= 0)
            {
                for (int i = 0; i < Count - 1; i++)
                {
                    this[i].Update(delta, ref m_pvmdv_UBOArray[i]);
                }
            }

            // now upload any changed entries
        }

        private int m_LastEntryIndex = -1;
        private ProjViewModelDiffuseViewport_UBOData[] m_pvmdv_UBOArray = new ProjViewModelDiffuseViewport_UBOData[GL_MAX_VERTEX_UNIFORM_BLOCKS];

        private class SceneObjectAndUBO
        {
            public SceneObject SceneObject { get; private set; }
            public bool WasUBOUpdated { get; private set; }

            public int gl_UBO_BufferID = -1;
            public ProjViewModelDiffuseViewport_UBOData UboData = new ProjViewModelDiffuseViewport_UBOData();

            public SceneObjectAndUBO(SceneObject so)
            {
                SceneObject = so;
            }

            public void ClearUpdatedFlag()
            {
                WasUBOUpdated = false;
            }

            public void Update(double delta)
            {
                WasUBOUpdated = SceneObject.Update(delta, ref UboData);
            }
        }
    }
}
