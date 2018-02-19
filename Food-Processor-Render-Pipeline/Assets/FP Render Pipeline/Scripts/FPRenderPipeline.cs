using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

//[GENZO] Suggestion to change name to something more tangible
namespace FPRenderNamespace
{
	//Rendering Pipeline
    public class FPRenderPipeline : RenderPipeline
    {
		//A reference to an instance of a pipeline asset
        private FPRenderPipelineAsset assetReference;

		//Consturctor that gets a reference to an asset passed in.
        public FPRenderPipeline(FPRenderPipelineAsset FPPipelineAsset)
        {
			//Store a pointer to the asset
            assetReference = FPPipelineAsset; //shouldn't be null
        }

		//Draws to screen?
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            CommandBuffer cb = CommandBufferPool.Get(); //Obtain CommandBuffer queue from pool.

            //Call render for a camera?
            foreach (Camera c in cameras)
            { 
                renderContext.SetupCameraProperties(c);                 //Does some interesting things(?)
                cb.ClearRenderTarget(true, true, assetReference.color); //Set Color.

                foreach(FPRenderComponent FPRenderComponent in FPRenderPipelineAsset.ListOfRenderComponent)
                {
                    cb.DrawRenderer(FPRenderComponent.DefaultUnityMeshRenderer, FPRenderComponent.DefaultUnityMeshRenderer.sharedMaterial, 0);
                }

                renderContext.ExecuteCommandBuffer(cb);                 //Execute commands in queue.
            }

            base.Render(renderContext, cameras);                        //Render cameras.
            renderContext.Submit();                                     //Submit changes.
            cb.Release();                                               //Release obtained CommandBuffer.
        }

    }
}

#region "Unity Pooling System"



namespace UnityEngine.Experimental.Rendering
{
    class ObjectPool<T> where T : new()
    {
        readonly Stack<T> m_Stack = new Stack<T>();
        readonly UnityAction<T> m_ActionOnGet;
        readonly UnityAction<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
    }

    public static class CommandBufferPool
    {
        static ObjectPool<CommandBuffer> s_BufferPool = new ObjectPool<CommandBuffer>(null, x => x.Clear());

        public static CommandBuffer Get()
        {
            var cmd = s_BufferPool.Get();
            cmd.name = "Unnamed Command Buffer";
            return cmd;
        }

        public static CommandBuffer Get(string name)
        {
            var cmd = s_BufferPool.Get();
            cmd.name = name;
            return cmd;
        }

        public static void Release(CommandBuffer buffer)
        {
            s_BufferPool.Release(buffer);
        }
    }
}



#endregion