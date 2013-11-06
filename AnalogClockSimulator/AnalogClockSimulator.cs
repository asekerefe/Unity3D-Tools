using UnityEngine;
using System;

/*
 * AnalogClockSimulator
 *
 * This class is responsible for simulating
 * an analog clock which starts from the desired time.
 *
 * Hand objects need to be assigned for visualization.
 * Also, all of them must be facing the same direction.
 *
 * Initial time values are used to set the starting
 * time of the clock. However, if 'useSystemTime' flag
 * is checked, these values are ignored and the
 * clock will use the system time.
 *
 * written by Alican Şekerefe
*/

public class AnalogClockSimulator : MonoBehaviour
{
    //the game objects that are going to be used for visualization
    public GameObject hourHand=null;
    public GameObject minuteHand=null;
    public GameObject secondHand=null;

    //values used to set the starting time of the clock
    public int initialHours=0;
    public int initialMinutes=0;
    public int initialSeconds=0;

    //if true, clock uses the system time and ignores the inital time data
    //else, clock will be initialized with the given time
    public bool useSystemTime=true;
    //simulates the second hand with the millisecond resolution
    public bool simulateMilliseconds=true;
    //reverses the rotation direction of the hands.
    public bool reverseRotation=false;
    
    //holds the current time
    private double currentTimeInMilliseconds=0;

    //precalculated values to be used in time conversions
    private const float MAX_SECONDS_IN_MINUTE=  60f;
    private const float MAX_SECONDS_IN_HOUR=    60f*60f;
    private const float MAX_SECONDS_IN_DAY=     60f*60f*12f;

    private void Start()
    {
        //set the current time variable

        if(useSystemTime)
        {
            //find the epoch time
            TimeSpan timeSince1970=DateTime.Now-DateTime.MinValue;
            //set it as the initial time
            currentTimeInMilliseconds=timeSince1970.TotalSeconds;
        }
        else
        {
            //use given initial values and initialize the clock timer
            currentTimeInMilliseconds=  ((initialHours  %12f) * MAX_SECONDS_IN_HOUR)  +
                                        ((initialMinutes%60f) * MAX_SECONDS_IN_MINUTE)+
                                          initialSeconds%60f;
        }
    }

    private void Update()
    {
        //add the time elapsed to the latest calculated time
        currentTimeInMilliseconds += Time.deltaTime;
        
        //calculate and set angles
        setHandAngle(hourHand,  getHourAngle(currentTimeInMilliseconds));
        setHandAngle(minuteHand,getMinuteAngle(currentTimeInMilliseconds));

        //check if the second hand will be simulated in milliseconds.
        //if yes, get rid of the millisecond data
        setHandAngle(secondHand,getSecondAngle(simulateMilliseconds?currentTimeInMilliseconds:Math.Round(currentTimeInMilliseconds)));
    }

    //returns the hour hand angle in degrees by the given time in seconds
    private float getHourAngle(double timeInMilliseconds)
    {
        return (float)((timeInMilliseconds%MAX_SECONDS_IN_DAY)/(double)MAX_SECONDS_IN_DAY)*360f;
    }

    //returns the minute hand angle in degrees by the given time in seconds
    private float getMinuteAngle(double timeInMilliseconds)
    {
        return (float)((timeInMilliseconds%MAX_SECONDS_IN_HOUR)/(double)MAX_SECONDS_IN_HOUR)*360f;
    }

    //returns the second hand angle in degrees by the given time in seconds
    private float getSecondAngle(double timeInMilliseconds)
    {
        return (float)((timeInMilliseconds%MAX_SECONDS_IN_MINUTE)/(double)MAX_SECONDS_IN_MINUTE)*360f;
    }

    //sets the z-axis angle for the given hand object using the target angle in degrees
    private void setHandAngle(GameObject hand, float targetAngle)
    {
        if(hand!=null)
            //set the euler angles by protecting the original x & y axis values
            //if required, reverse the input angle
            hand.transform.eulerAngles=new Vector3(hand.transform.eulerAngles.x,
                                                   hand.transform.eulerAngles.y,
                                                   targetAngle*(reverseRotation?-1f:1f));
    }
}
