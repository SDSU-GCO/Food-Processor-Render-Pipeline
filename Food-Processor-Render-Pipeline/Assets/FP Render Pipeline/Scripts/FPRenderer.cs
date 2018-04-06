using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

namespace GCO.FP
{
    public abstract class FPRenderer : MonoBehaviour
    {
        public abstract int RenderVariableBatch(CommandBuffer cmd, FPRenderer renderer);
        //returns number of noodles processed.
        //public abstract int RenderVariableBatch(CommandBuffer cmd, FPRender renderer, NativeList<NoodleData> noodles, int noodleStart);
    }
}