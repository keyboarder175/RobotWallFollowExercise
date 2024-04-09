using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RobotDriver : MonoBehaviour
{
    public float rayRange = 5;
    float lws = 5;
    float rws = 5;

    float trans = 0;
    float rot = 0;

    enum State {DriveStraight, ToFarFromWall, ToNearToWall, WasToFar, WasToNear}

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



    // Update is called once per frame
    void Update()
    {
        calcNextFrameTranslationAndRotation();
        this.transform.Translate(new Vector3(0,0,trans*Time.deltaTime));
        this.transform.Rotate(new Vector3(0,rot*Time.deltaTime,0));
       
       
        Vector3 raydirection = Vector3.right;
        Ray theRay = new Ray(transform.position, transform.TransformDirection(raydirection*rayRange));
        Debug.DrawRay(transform.position, transform.TransformDirection(raydirection * rayRange));

        if(Physics.Raycast(theRay, out RaycastHit hit, rayRange)){
            if(hit.collider.tag == "Wall"){
                Debug.Log(currentState.ToString() + " " + hit.distance);

                adjustSpeeds(hit.distance);
            }
            
        }

    }

    float timer = 0;

    void adjustSpeeds(float distance){
        switch(currentState){
            case State.DriveStraight:
                setLeftWheelSpeed(5);
                setRightWheelSpeed(5);
                if(distance<9.9){
                    currentState = State.ToNearToWall;
                }
                if(distance>10.1){
                    currentState = State.ToFarFromWall;
                }
                break;
            case State.ToFarFromWall:
                setLeftWheelSpeed(7);
                setRightWheelSpeed(5);
                if(distance<10.1){
                    currentState = State.WasToFar;
                    timer = 0;
                }
                break;
            case State.ToNearToWall:
                setLeftWheelSpeed(5);
                setRightWheelSpeed(7);
                if(distance>9.9){
                    currentState = State.WasToNear;
                    timer = 0;
                }
                break;
            case State.WasToFar:
                setLeftWheelSpeed(5);
                setRightWheelSpeed(9);
                timer = timer + Time.deltaTime;
                if(timer>=1){
                    currentState = State.DriveStraight;
                }
                break;
            case State.WasToNear:
                setLeftWheelSpeed(9);
                setRightWheelSpeed(5);
                timer = timer + Time.deltaTime;
                if(timer>=1){
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
