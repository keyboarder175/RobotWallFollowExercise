
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

    float RightDistance;
    float LeftDistance;

    // Update is called once per frame
    void Update()
    {
        calcNextFrameTranslationAndRotation();
        this.transform.Translate(new Vector3(0, 0, trans * Time.deltaTime));
        this.transform.Rotate(new Vector3(0, rot * Time.deltaTime, 0));

        Vector3 raydirection = Vector3.forward;
        //right ray
        Ray frontRay = new Ray(rightRayTrans.position, transform.TransformDirection(raydirection * rayRange));
        Debug.DrawRay(rightRayTrans.position, transform.TransformDirection(raydirection * rayRange));

        if (Physics.Raycast(frontRay, out RaycastHit frontHit, rayRange))
        {
            if (frontHit.collider.tag == "Wall")
            {
                RightDistance = frontHit.distance;
            }

        }

        //left ray

        Ray rearRay = new Ray(leftRayTrans.position, transform.TransformDirection(raydirection * rayRange));
        Debug.DrawRay(leftRayTrans.position, transform.TransformDirection(raydirection * rayRange));

        if (Physics.Raycast(rearRay, out RaycastHit readHit, rayRange))
        {
            if (readHit.collider.tag == "Wall")
            {
                LeftDistance = readHit.distance;
            }

        }

        float distance = (LeftDistance + RightDistance) / 2;
        float angle;

        float distDiff = LeftDistance - RightDistance;
        Vector3 sensorVectr = rightRayTrans.position - leftRayTrans.position;
        float sensorDist = sensorVectr.magnitude;
        angle = Mathf.Rad2Deg * Mathf.Atan(distDiff / sensorDist);

        adjustSpeeds(Mathf.Pow(10f * 2.7182818284590452353602874713527f, -(RightDistance / 100f)) , Mathf.Pow(10f * 2.7182818284590452353602874713527f , -(LeftDistance / 100f)));


        


    }

    float timer = 0;

    void adjustSpeeds(float rightDistValue, float leftDistValue)
    {
        setRightWheelSpeed(leftDistValue - rightDistValue+0.6f);
        //setLeftWheelSpeed(-13f * leftDistValue + 94f * rightDistValue);
        setLeftWheelSpeed(0.5f);
    }
}