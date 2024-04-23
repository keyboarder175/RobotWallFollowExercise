
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallAvoidWith2SensorsAndWall : MonoBehaviour
{
    public float rayRange = 100f;
    float lws = 5;
    float rws = 5;

    float trans = 0;
    float rot = 0;

    public Transform rightRayTrans;
    public Transform leftRayTrans;

    enum State { DriveStraight, ToFar, ToNear, InTolleranceFromRight, InTolleranceFromLeft }

    State currentState = State.DriveStraight;

    public void setRightWheelSpeed(float speed)
    {
        rws = speed;
    }

    public void setLeftWheelSpeed(float speed)
    {
        lws = speed;
    }

    public void calcNextFrameTranslationAndRotation()
    {
        trans = (lws + rws) / 2f;
        rot = 100*((lws - rws) * 2f) / 4f;
        Debug.Log(rot);
    }

    float RightValue;
    float LeftValue;

    // Update is called once per frame
    void Update()
    {
        calcNextFrameTranslationAndRotation();
        this.transform.Translate(new Vector3(0, 0, trans * Time.deltaTime));
        this.transform.Rotate(new Vector3(0, rot * Time.deltaTime, 0));

        Vector3 raydirection = Vector3.forward;
        
        //right ray
        Vector3 rightRayDirection = Quaternion.Euler(0, 5, 0) *raydirection;
        Ray frontRay = new Ray(rightRayTrans.position, transform.TransformDirection(rightRayDirection * rayRange));
        Debug.DrawRay(rightRayTrans.position, transform.TransformDirection(rightRayDirection * rayRange));

        if (Physics.Raycast(frontRay, out RaycastHit frontHit, rayRange))
        {
            if (frontHit.collider.tag == "Wall")
            {
                RightValue = distanceToValue(frontHit.distance);
            }else{
                RightValue = 0;
            }

        }else{
            RightValue = 0;
        }

        //left ray
        Vector3 leftRayDirection = Quaternion.Euler(0, -5, 0) *raydirection;
        Ray rearRay = new Ray(leftRayTrans.position, transform.TransformDirection(leftRayDirection * rayRange));
        Debug.DrawRay(leftRayTrans.position, transform.TransformDirection(leftRayDirection * rayRange));

        if (Physics.Raycast(rearRay, out RaycastHit readHit, rayRange))
        {
            if (readHit.collider.tag == "Wall")
            {
                LeftValue = distanceToValue(readHit.distance);
            }else{
                LeftValue = 0;
            }
        }else{
            LeftValue = 0;
        }

        adjustSpeeds(RightValue , LeftValue);


        


    }

    float distanceToValue(float distance){
        return Mathf.Pow(10f * 2.7182818284590452353602874713527f, -(distance / 100f));
    }

    float timer = 0;

    void adjustSpeeds(float rightDistValue, float leftDistValue)
    {   Debug.Log("Left: " + leftDistValue + " Right: " + rightDistValue);
        setRightWheelSpeed(-9f * leftDistValue + 9f * rightDistValue + 10f);
        setLeftWheelSpeed(9f * leftDistValue - 9f * rightDistValue + 10f);
        Debug.Log("LeftWheelSpeed: " + lws + "rightWheelSpeed: " + rws);
        //setRightWheelSpeed(rightDistValue - leftDistValue + 0.55f);
        
        //setLeftWheelSpeed(0.5f);
    }
}