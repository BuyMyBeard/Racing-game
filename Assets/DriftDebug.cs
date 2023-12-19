using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftDebug : MonoBehaviour
{
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider backLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] float extremumSlip;

    void UpdateExtremumSlip(WheelCollider wc, float extremumSlip)
    {
        var sf = wc.sidewaysFriction;
        sf.extremumSlip = extremumSlip;
        wc.forwardFriction = sf;
    }
    private void OnValidate()
    {
        UpdateExtremumSlip(frontLeft, extremumSlip);
        UpdateExtremumSlip(frontRight, extremumSlip);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
