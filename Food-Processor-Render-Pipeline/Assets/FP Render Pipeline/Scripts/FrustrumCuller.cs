using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCuller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    bool PointInFrustum()
    {
        bool inFrustrum = true;
        //test all 6 frustum planes
        for (int i = 0; i < 6; i++)
        {
            


        }
        return inFrustrum;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
