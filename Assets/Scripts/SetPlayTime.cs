using UnityEngine;

public class SetPlayTime : MonoBehaviour
{
    public static float min ;
    public static float sec ;
    public class TimeSet
    {

        public static float timerVal;

        static TimeSet()//this is a static constructor- it is used to initialize a static variable only once(beginning of the game)
        {
            
            timerVal = min * 60 + sec;
            Debug.Log(timerVal);    
        }
    }

}
