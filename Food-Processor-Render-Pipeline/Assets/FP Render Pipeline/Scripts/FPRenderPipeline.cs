using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;


namespace GCO.FP
{
	//Rendering Pipeline
    public class FPRenderPipeline : RenderPipeline
    {
		//Alex: A reference to an instance of a pipeline asset
        //Tyler: A reference to the asset that created this instance Factory-style
        private FPRenderPipelineAsset AssetReference;

		//Alex: Consturctor that gets a reference to an asset passed in.
        //Tyler: Constructor that gets only the creating asset reference passed in.
        public FPRenderPipeline(FPRenderPipelineAsset FPPipelineAsset)
        {
			//Alex: Store a pointer to the asset
            //Tyler: Initialize the asset reference with no null checking.
            AssetReference = FPPipelineAsset; //Alex: shouldn't be null
#if UNITY_EDITOR
            SupportedRenderingFeatures.active = new SupportedRenderingFeatures()
            {
                defaultMixedLightingMode = SupportedRenderingFeatures.LightmapMixedBakeMode.None,
                reflectionProbeSupportFlags = SupportedRenderingFeatures.ReflectionProbeSupportFlags.None,
                rendererSupportsLightProbeProxyVolumes = false, //will be true later
                rendererSupportsMotionVectors = false,          //will be true later
                rendererSupportsReceiveShadows = false,         //will be true later
                rendererSupportsReflectionProbes = false,       //will be true later
                supportedLightmapBakeTypes = 0,
                supportedLightmapsModes = LightmapsMode.NonDirectional,
                supportedMixedLightingModes = SupportedRenderingFeatures.LightmapMixedBakeMode.None,//indirect only later
            };
#endif

        }

        public override void Dispose()
        {
            base.Dispose();
#if UNITY_EDITOR
            SupportedRenderingFeatures.active = new SupportedRenderingFeatures();
#endif
        }

        //Alex: Draws to screen?
        //Tyler: Draws to whatever the camera is drawing to (which can be set in the editor.
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            //Tyler: Performs sanity checks (not well documented)
            base.Render(renderContext, cameras);

            //Tyler: Obtains a CommandBuffer instance from the pool to save on GC pressure.
            CommandBuffer cb = CommandBufferPool.Get(); //Alex: Obtain CommandBuffer queue from pool.

            //Alex: Call render for a camera?
            //Tyler: Process rendering for each camera in the order provided.
            foreach (Camera camera in cameras)
            {
                //Tyler: Sets up Unity's internal camera variables (not well documented) and binds the render target to the camera's target.
                renderContext.SetupCameraProperties(camera);                 //Alex: Does some interesting things(?)
                cb.ClearRenderTarget(true, true, AssetReference.Color); //Alex: Set Color.  Tyler: Clear background color to the color in the FP Asset and clear out the depth buffer.

                foreach(FPRenderComponent FPRenderComponent in FPRenderPipelineAsset.ListOfRenderComponent)
                {
                    cb.DrawRenderer(FPRenderComponent.DefaultUnityMeshRenderer, FPRenderComponent.DefaultUnityMeshRenderer.sharedMaterial, 0);

                }

#if UNITY_EDITOR
                // Emit scene view UI
                if (camera.cameraType == CameraType.SceneView)
                {
                    ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
                }
#endif

                renderContext.ExecuteCommandBuffer(cb);                 //Alex: Execute commands in queue.
            }

            renderContext.Submit();                                     //Alex: Submit changes.
            cb.Release();                                               //Alex: Release obtained CommandBuffer.
        }
    }
}