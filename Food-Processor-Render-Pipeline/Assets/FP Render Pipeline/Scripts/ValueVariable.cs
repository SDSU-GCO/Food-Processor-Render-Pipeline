using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace gs
{
    //generic template for dynamic variables
    public class ValueVariable<T> : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif

        [SerializeField]
        private T value;

        public T Value
        {
            get
            {
                return value;
            }
            set 
            {
                this.value=value;
            }
        }
        
    }

    /// <summary>
    /// Use this to create a dynamic String
    /// </summary>
    [CreateAssetMenu(fileName = "New String Variable.asset", menuName = "Dynamic Variable/String")]
    public class DynamicStringVariable : ValueVariable<string>
    {
        DynamicStringVariable()
        {
            Value = "";
        }
    }

    /// <summary>
    /// Use this to create a dynamic Int
    /// </summary>
    [CreateAssetMenu(fileName = "New Int Variable.asset", menuName = "Dynamic Variable/Int")]
    public class DynamicIntVariable : ValueVariable<int>
    {
        DynamicIntVariable()
        {
            Value = 0;
        }
    }

    /// <summary>
    /// Use this to create a dynamic Char
    /// </summary>
    [CreateAssetMenu(fileName = "New Char Variable.asset", menuName = "Dynamic Variable/Char")]
    public class DynamicCharVariable : ValueVariable<char>
    {
        DynamicCharVariable()
        {
            Value = '\0';
        }
    }

    /// <summary>
    /// Use this to create a dynamic Vector3
    /// </summary>
    [CreateAssetMenu(fileName = "New Vector3 Variable.asset", menuName = "Dynamic Variable/Vector3")]
    public class DynamicVector3Variable : ValueVariable<Vector3>
    {
        DynamicVector3Variable()
        {
            Value = new Vector3(0,0,0);
        }
    }
}