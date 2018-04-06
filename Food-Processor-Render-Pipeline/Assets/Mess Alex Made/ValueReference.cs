using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace gs
{
    [Serializable]
    public class ValueReference<T>
    {
        public bool UseLocalValue = true;
        public T LocalValue;
        virtual public ValueVariable<T> Variable { get; }

        public ValueReference()
        { }

        public ValueReference(T value)
        {
            UseLocalValue = true;
            LocalValue = value;
        }

        public T Value
        {
            get { return UseLocalValue ? LocalValue : Variable.Value; }
        }

        public static implicit operator T(ValueReference<T> reference)
        {
            return reference.Value;
        }
    }


    [Serializable]
    public class CharReference : ValueReference<char>
    {
        public DynamicCharVariable reference;
        public override ValueVariable<char> Variable
        {
            get
            {
                return reference;
            }
        }
    }

    [Serializable]
    public class StringReference : ValueReference<string>
    {
        public DynamicStringVariable reference;
        public override ValueVariable<string> Variable
        {
            get
            {
                return reference;
            }
        }
    }

    [Serializable]
    public class IntReference : ValueReference<int>
    {
        public DynamicIntVariable reference;
        public override ValueVariable<int> Variable
        {
            get
            {
                return reference;
            }
        }
    }

    [Serializable]
    public class Vector3Reference : ValueReference<Vector3>
    {
        public DynamicVector3Variable reference;
        public override ValueVariable<Vector3> Variable
        {
            get
            {
                return reference;
            }
        }
    }
}
