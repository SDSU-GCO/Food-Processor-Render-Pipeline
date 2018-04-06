using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif



namespace GCO.FP
{
    public class FPRenderPipelineAsset : RenderPipelineAsset
    {
		//Background color
        public Color BackgroundColor;
        public Color AmbientColor;

        public static List<FPRenderer> ListOfRenderers = new List<FPRenderer>();
        public static Light mainDirectionalLight = null;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Food Processor Rendering/Render Pipeline Asset")]
	    public static void MenuCreateFPRenderAsset()
	    {
	        Texture2D icon = EditorGUIUtility.FindTexture("FoodProcessor.png");
	        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance <CreateFPPipelineAsset>(), "FPRender.asset", icon, null);
	    }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
			return new FPRenderPipeline(this);
        }

#if UNITY_EDITOR
        //This is a stateful deferred callback for when creating an asset and getting the name. The asset is not created until the name editing is finished.
        public class CreateFPPipelineAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                FPRenderPipelineAsset instance = CreateInstance<FPRenderPipelineAsset>();
                instance.BackgroundColor = Color.black;
                instance.AmbientColor = Color.black;
                AssetDatabase.CreateAsset(instance, pathName);
                ProjectWindowUtil.ShowCreatedAsset(instance);
            }
        }
#endif
    }



}
