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
		//A reference to an instance of a pipeline asset
        private FPRenderPipelineAsset AssetReference;

		//Consturctor that gets a reference to an asset passed in.
        public FPRenderPipeline(FPRenderPipelineAsset FPPipelineAsset)
        {
			//Store a pointer to the asset
            AssetReference = FPPipelineAsset; //shouldn't be null
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

        //Draws to screen?
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            CommandBuffer cb = CommandBufferPool.Get(); //Obtain CommandBuffer queue from pool.

            //Call render for a camera?
            foreach (Camera camera in cameras)
            { 
                renderContext.SetupCameraProperties(camera);                 //Does some interesting things(?)
                cb.ClearRenderTarget(true, true, AssetReference.Color); //Set Color.

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

                renderContext.ExecuteCommandBuffer(cb);                 //Execute commands in queue.
            }

            base.Render(renderContext, cameras);                        //Render cameras.
            renderContext.Submit();                                     //Submit changes.
            cb.Release();                                               //Release obtained CommandBuffer.
        }
    }
}