using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Robot2RayControll : MonoBehaviour
{
    public float rayRange = 100f;
    float lws = 5;
    float rws = 5;

    float trans = 0;
    float rot = 0;

    public Transform frontRayTrans;
    public Transform rearRayTrans;

    enum State {DriveStraight, ToFar, ToNear, InTolleranceFromRight, InTolleranceFromLeft}

    State currentState = State.DriveStraight;

    public void setRightWheelSpeed(float speed){
        rws = speed;
    }

    public void setLeftWheelSpeed(float speed){
        lws = speed;
    }

    public void calcNextFrameTranslationAndRotation(){
        trans = (lws+rws)/2f;
        rot = ((lws-rws)*2f)/4f;
    }

    float frontDistance;
    float rearDistance;

    // Update is called once per frame
    void Update()
    {
        calcNextFrameTranslationAndRotation();
        this.transform.Translate(new Vector3(0,0,trans*Time.deltaTime));
        this.transform.Rotate(new Vector3(0,rot*Time.deltaTime,0));
       
        Vector3 raydirection = Vector3.right;
        //front ray
        Ray frontRay = new Ray(frontRayTrans.position, transform.TransformDirection(raydirection*rayRange));
        Debug.DrawRay(frontRayTrans.position, transform.TransformDirection(raydirection * rayRange));

        if(Physics.Raycast(frontRay, out RaycastHit frontHit, rayRange)){
            if(frontHit.collider.tag == "Wall"){
                frontDistance = frontHit.distance;
            }
            
        }

        //front ray
        
        Ray rearRay = new Ray(rearRayTrans.position, transform.TransformDirection(raydirection*rayRange));
        Debug.DrawRay(rearRayTrans.position, transform.TransformDirection(raydirection * rayRange));

        if(Physics.Raycast(rearRay, out RaycastHit readHit, rayRange)){
            if(readHit.collider.tag == "Wall"){
                rearDistance = readHit.distance;
            }
            
        }

        float distance = (rearDistance + frontDistance)/2;
        float angle;

        float distDiff = rearDistance - frontDistance;
        Vector3 sensorVectr = frontRayTrans.position - rearRayTrans.position;
        float sensorDist = sensorVectr.magnitude;
        angle = Mathf.Rad2Deg*Mathf.Atan(distDiff/sensorDist);

        adjustSpeeds(distance, angle);
        
        
        Debug.Log("dist front: " + frontDistance + " read: " + rearDistance);
        Debug.Log("Status " + currentState);
        Debug.Log("ANgle: " + angle);
        Debug.Log("Middledistance: " + distance);
        

    }

    float timer = 0;

    void adjustSpeeds(float distance, float angle){
        switch(currentState){
            case State.DriveStraight:
                setLeftWheelSpeed(5);
                setRightWheelSpeed(5);
                if(distance<9.6){
                    currentState = State.ToNear;
                }
                if(distance>10.4){
                    currentState = State.ToFar;
                }
                break;
            case State.ToFar:
                if(angle>15){
                    setLeftWheelSpeed(5);
                    setRightWheelSpeed(5);
                }else{
                    setLeftWheelSpeed(9);
                    setRightWheelSpeed(5);
                }
                
                if(distance<10.4){
                    currentState = State.InTolleranceFromLeft;
                }
                break;
            case State.ToNear:
                if(angle<-15){
                    setLeftWheelSpeed(5);
                    setRightWheelSpeed(5);
                }else{
                    setLeftWheelSpeed(5);
                    setRightWheelSpeed(9);
                }
                if(distance>9.6){
                    currentState = State.InTolleranceFromRight;
                }
                break;
            case State.InTolleranceFromLeft:
                if (Mathf.Abs(angle) > 2){
                    setLeftWheelSpeed(5);
                    setRightWheelSpeed(12);
                }else{
                    currentState = State.DriveStraight;
                }
                break;
            case State.InTolleranceFromRight:
                if (Mathf.Abs(angle) > 2)
                {
                    setLeftWheelSpeed(12);
                    setRightWheelSpeed(5);
                }
                else
                {
                    currentState = State.DriveStraight;
                }
                break;

            default:
                setLeftWheelSpeed(5);
                setRightWheelSpeed(5);
                break;


        }
        
    }
}
