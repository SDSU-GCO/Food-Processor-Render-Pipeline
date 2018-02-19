using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPRenderNamespace
{
    /// <summary>
    /// This component is designed to track which objects need to get rendered
    /// by adding itself to a static list in FPRenderPipelineAsset.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]//Require all atatched objects to have a default unity MeshRenderer
    [ExecuteInEditMode]//Run even in edit mode
    public class FPRenderComponent : MonoBehaviour {

        //hold a reference to the meshrenderer with a backing field(don't allow external sets)
        public MeshRenderer DefaultUnityMeshRenderer { get; private set; }

        private void OnEnable()
        {
            //update the reference to the current meshRenderer
            DefaultUnityMeshRenderer = GetComponent<MeshRenderer>();

            //add this component to the list
            FPRenderPipelineAsset.ListOfRenderComponent.Add(this);
        }
        private void OnDisable()
        {
            //remove this component from the list
            FPRenderPipelineAsset.ListOfRenderComponent.Remove(this);
        }

    }

}
