using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCO.FP
{
    [ExecuteInEditMode]
    public class FPLight : MonoBehaviour
    {
        private void OnEnable()
        {
            if (FPRenderPipelineAsset.mainDirectionalLight == null)
            {
                FPRenderPipelineAsset.mainDirectionalLight = GetComponent<Light>();
            }
        }

        private void OnDisable()
        {
            if (FPRenderPipelineAsset.mainDirectionalLight == GetComponent<Light>())
            {
                FPRenderPipelineAsset.mainDirectionalLight = null;
            }
        }
    }
}