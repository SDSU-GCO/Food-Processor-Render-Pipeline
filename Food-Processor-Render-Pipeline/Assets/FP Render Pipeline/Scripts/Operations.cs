using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Operations{
    
    public static void swap<T>(ref T A, ref T B)
    {
        T C = A;
        A = B;
        B = C;
    }


    //static void Call()
    //{
    //    int var1, var2;
    //    var1 = var2 = 9;
    //    Operations.swap<int>(ref var1, ref var2);
    //}

}
