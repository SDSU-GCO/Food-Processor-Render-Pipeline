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
    /// <summary>
    /// Alex: This class is responsible for actually creating and initializing assets to plug into unities rendering pipeline.
    /// </summary>
    public class FPRenderPipelineAsset : RenderPipelineAsset
    {
		//Alex: create a variable that holds a color.  This can be set via the editor.
        public Color Color;

        public static List<FPRenderComponent> ListOfRenderComponent = new List<FPRenderComponent>();

//if we are working in the unity editor
#if UNITY_EDITOR
        /// <summary>
        /// This us setup to run when the menue item is clicked.  It creates an instance of our FPPipelineAsset.
        /// It uses CreateFPPipelineAsset to accomplish this.
        /// </summary>
        [MenuItem("Assets/Create/Food Processor Rendering/Render Pipeline Asset")]//create a menue item
	    public static void MenuCreateFPRenderAsset()//that runs this function
	    {

			//Create a link to a 2d sprite Icon in the "Gizmos" folder.
	        Texture2D icon = EditorGUIUtility.FindTexture("FoodProcessor.png");

			//Alex: Create a new scriptable object rendering pipeline witht the icon we just made in the assets folder.
	        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance <CreateFPPipelineAsset>(), "FPRender.asset", icon, null);
	    }
#endif

        //Alex: This returns an instance(copy) of our pipeline... I don't actually know why we need this.  It overrides something in Unity that Unity calls.
        //Tyler: So assets act as a sort of Factory for actual pipelines. Theoretically, we could use polymorphism for our different RenderPipelines and let 
        //the asset choose which one based on settings. Can be especially useful for different hardware targets requiring different optimization techniques.
        protected override IRenderPipeline InternalCreatePipeline()
        {
			//Alex: returns a copy of itself(pass by value)
            //Tyler: Creates an instance of an FPRenderPipeline in FPRenderPipeline.cs and gives it an instance of this asset. Then returns a reference of the
            //new FPRenderPipeline instance back to Unity.
			return new FPRenderPipeline(this);
        }

        /// Alex: Create a new pipeline asset that inherits from EndNameEditAction.
        /// I have no clue what EndNameEditAction actually is or what it does.
        /// This is where we setup the piepline defaults like color.

        /// <summary>
        /// Tyler: This is a callback class that generates an FPRenderPipelineAsset after the user has finished editing the name in the Project Window.
        /// </summary>
        /// 
#if UNITY_EDITOR
        public class CreateFPPipelineAsset : EndNameEditAction
        {
			//Alex: I don't know what calls this.  I also don't know what it does.
            //Tyler: This is the actual method Unity calls where we create the asset after the naming operation is done.
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                //Alex: we store an instance of our cheese pipeline asset into instance
                FPRenderPipelineAsset instance = CreateInstance<FPRenderPipelineAsset>();

				//Alex: we default the color to black
                instance.Color = Color.black;

				//Alex: I don't know what this paths to.
                AssetDatabase.CreateAsset(instance, pathName);

				//Alex: this MIGHT be updating the project folder??????
                //Tyler: This just highlights the asset after it was created. Useful since it might move in the view due to ABC sorting.
                ProjectWindowUtil.ShowCreatedAsset(instance);
            }
        }
#endif
    }



}
