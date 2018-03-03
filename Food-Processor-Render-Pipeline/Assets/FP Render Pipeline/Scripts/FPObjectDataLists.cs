using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

public static class FPCameraDataLists
{
    public static NativeList<FPCameraStrainerData> FPCameraStrainerDataList = new NativeList<FPCameraStrainerData>();
    public static NativeList<FPNoodleData> FPNoodleDataList = new NativeList<FPNoodleData>();

    public struct FPCameraStrainerData
    {
        // This provides a means to get back to our FPRenderer component.
        // It might not actually be necessary if we make our algorithm rock-solid.
        public int index;

        // This provides a means to get to the Noodles in a C# Job
        public int noodlesStart;
        public int noodlesCount;

        // This is optional depending on the C# Job execution techniques used.
        public bool visible;

        // Filter information
        public int layer;

        // ...

        // Bounds: Can either be UnityEngine.Bounds
        public Bounds bounds;

        // or a subset
        public Vector3 minBounds;
        public Vector3 maxBounds;

        // As a side note, using the early-out plane testing is actually not optimal 
        // in this algorithm due to the number of potential cameras and additional 
        // required bandwidth. BVH culling will provide enough optimization for us.

        // BVH data
        // Whatever BVH data is needed goes here...
    }

    public struct FPNoodleData
    {
        // This index back to the FPRenderer IS necessary.
        public System.Int32 index;

        // This will either be 64 or 128 bits split into 2 64 bit numbers.
        // Needs to be unsigned for sorting to work with encoded enums
        public Key noodle;
    }

    public struct Key
    {
        public System.Int64 A;
        public System.Int64 B;

        public bool operator>(Key A, Key B)
        {
            return true;//one for now
        }
        public bool operator <(Key A, Key B)
        {
            return false;//one for now
        }
    }
}
