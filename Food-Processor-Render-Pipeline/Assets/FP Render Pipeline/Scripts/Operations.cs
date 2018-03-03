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

}
