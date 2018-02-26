using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCO.FP
{
    /// <summary>
    /// Alex: This component is designed to track which objects need to get rendered
    /// by adding itself to a static list in FPRenderPipelineAsset.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]//Alex: Require all atatched objects to have a default unity MeshRenderer
    [ExecuteInEditMode]//Alex: Run even in edit mode
    public class FPRenderComponent : MonoBehaviour
    {

        //Alex: hold a reference to the meshrenderer with a backing field(don't allow external sets)
        //Tyler: Don't allow external mutations.
        public MeshRenderer DefaultUnityMeshRenderer { get; private set; }

        private void OnEnable()
        {
            //update the reference to the current meshRenderer
            //Tyler: Grab Unity's built-in mesh renderer
            DefaultUnityMeshRenderer = GetComponent<MeshRenderer>();

            //Alex: add this component to the list
            FPRenderPipelineAsset.ListOfRenderComponent.Add(this);
        }

        private void OnDisable()
        {
            //Alex: remove this component from the list
            FPRenderPipelineAsset.ListOfRenderComponent.Remove(this);
        }

    }

}
