using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

namespace GCO.FP
{
    /// <summary>
    /// Alex: This component is designed to track which objects need to get rendered
    /// by adding itself to a static list in FPRenderPipelineAsset.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]//Alex: Require all atatched objects to have a default unity MeshRenderer
    [ExecuteInEditMode]//Alex: Run even in edit mode
    public class FPMeshRenderer : FPRenderer
    {

        //Alex: hold a reference to the meshrenderer with a backing field(don't allow external sets)
        //Tyler: Don't allow external mutations.
        public MeshRenderer UnityMeshRenderer { get; private set; }



        private void OnEnable()
        {
            //update the reference to the current meshRenderer
            //Tyler: Grab Unity's built-in mesh renderer
            UnityMeshRenderer = GetComponent<MeshRenderer>();

            //Alex: add this component to the list
            FPRenderPipelineAsset.ListOfRenderers.Add(this);


            //FPCameraDataLists.FPNoodleData TemporaryFPNoodleData = new FPCameraDataLists.FPNoodleData
            //{
            //    index = ,
            //    noodle = ,
            //};

            //FPCameraDataLists.FPNoodleDataList.Add(TemporaryFPNoodleData);

            //FPCameraDataLists.FPCameraStrainerData CurrentFPCameraStrainerData = new FPCameraDataLists.FPCameraStrainerData
            //{
            //    bounds = DefaultUnityMeshRenderer.bounds,
            //    index = 0,
            //    layer = 0,
            //    maxBounds = ,
            //    minBounds = ,
            //    noodlesCount = ,
            //    noodlesStart = ,
            //    visible = 1,
            //};

            //FPCameraDataLists.FPCameraStrainerDataList.Add(CurrentFPCameraStrainerData);

            //FPCameraDataLists.FPNoodleDataList;
        }

        private void OnDisable()
        {
            //Alex: remove this component from the list
            FPRenderPipelineAsset.ListOfRenderers.Remove(this);
        }

        public override int RenderVariableBatch(CommandBuffer cmd, FPRenderer renderer)
        {
            cmd.DrawRenderer(UnityMeshRenderer, UnityMeshRenderer.sharedMaterial);
            return 1;
        }
    }

}
