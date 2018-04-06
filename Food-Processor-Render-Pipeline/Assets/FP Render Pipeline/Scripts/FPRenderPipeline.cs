using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;


namespace GCO.FP
{
    public class FPRenderPipeline : RenderPipeline
    {
        private FPRenderPipelineAsset AssetReference;

        public FPRenderPipeline(FPRenderPipelineAsset FPPipelineAsset)
        {
            AssetReference = FPPipelineAsset;
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

        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            var cmd = CommandBufferPool.Get();
            cmd.SetGlobalColor("_ambientColor", AssetReference.AmbientColor);
            renderContext.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            foreach (Camera camera in cameras)
            {
                //Tyler: Sets up Unity's internal camera variables (not well documented) and binds the render target to the camera's target.
                renderContext.SetupCameraProperties(camera);

                if (camera.cameraType == CameraType.Preview)
                {
                    DrawPreviewCamera(renderContext, camera);
                }
                else
                {
                    DrawSimpleForward(renderContext, camera);
                }

#if UNITY_EDITOR
                //This is supposed to make scene icons work like camera and light icons
                // Emit scene view UI
                if (camera.cameraType == CameraType.SceneView)
                {
                    ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
                }
#endif
            } //end foreach
            renderContext.Submit();
        }

        void DrawPreviewCamera(ScriptableRenderContext context, Camera camera)
        {
            var cmd = CommandBufferPool.Get();
            cmd.ClearRenderTarget(true, true, AssetReference.BackgroundColor);
            ScriptableCullingParameters scp = new ScriptableCullingParameters();
            CullResults.GetCullingParameters(camera, out scp);
            var cr = CullResults.Cull(ref scp, context);
            VisibleLight? bestLight = null;
            foreach (var light in cr.visibleLights)
            {
                if (light.lightType == LightType.Directional)
                {
                    if (bestLight == null || bestLight?.finalColor.grayscale < light.finalColor.grayscale)
                    {
                        bestLight = light;
                    }
                }
            }
            if (bestLight != null)
            {
                var bl = bestLight ?? default(VisibleLight);
                cmd.SetGlobalColor("_LightColor", bl.finalColor);
                cmd.SetGlobalVector("_LightDirection", bl.localToWorld.MultiplyVector(Vector3.forward));
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            DrawRendererSettings drs = new DrawRendererSettings(camera, new ShaderPassName("Base"));
            FilterRenderersSettings frs = new FilterRenderersSettings(true)
            {
                renderQueueRange = RenderQueueRange.all,
            };
            context.DrawRenderers(cr.visibleRenderers, ref drs, frs);
        }

        void DrawSimpleForward(ScriptableRenderContext context, Camera camera)
        {
            var cmd = CommandBufferPool.Get();
            cmd.ClearRenderTarget(true, true, AssetReference.BackgroundColor);
            if (FPRenderPipelineAsset.mainDirectionalLight != null)
            {
                cmd.SetGlobalColor("_LightColor", FPRenderPipelineAsset.mainDirectionalLight.color * FPRenderPipelineAsset.mainDirectionalLight.intensity);
                cmd.SetGlobalVector("_LightDirection", FPRenderPipelineAsset.mainDirectionalLight.transform.TransformDirection(Vector3.forward));
            }
            foreach (FPRenderer fpRenderer in FPRenderPipelineAsset.ListOfRenderers)
            {
                fpRenderer.RenderVariableBatch(cmd, fpRenderer);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}