using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PIDController : MonoBehaviour
{

    public enum DerivitiveMeasurement
    {
        Velocity,ErrorRateOfChange
    }
    public float pGain,iGain,dGain;
    public float lastError; //Error Value from last time step
    public float lastValue;

    public float iStored;
    public float iSaturation;
    public DerivitiveMeasurement derivitiveMeasurement;
    public bool dInitialized;

    public float outputMin = 0;
    public float outputMax = 1;

    void Start()
    {
        
    }

    // Update is called once per frame
   public float PIDUpdate(float dt,float currentValue,float targetValue)
    {
        //Calculate the absolute distance
        float error = targetValue-currentValue;
        //Calculate P term 
        float P = pGain * error;
        //Calculate I term
        iStored = Mathf.Clamp( iStored + (error * dt),-iSaturation,iSaturation);
        float I = iGain * iStored;

        //Calculate Both D terms
        float errorRateOfChange = (error - lastError) / dt;
        lastError = error;
        float valueRateOfChange = (currentValue - lastValue) / dt;
        lastValue = currentValue;
        float dMeasure=0;
        if (dInitialized)
        {
            if (derivitiveMeasurement == DerivitiveMeasurement.Velocity) { dMeasure = -valueRateOfChange; }
            else { dMeasure = errorRateOfChange; }
        }

        else
        {
            dInitialized = true;
        }

        float D = dGain * dMeasure;

        float result = P +I+ D;
        return Mathf.Clamp(result,outputMin,outputMax);
        
    }
    public void Reset()
    {
        dInitialized = false;
    }
}
