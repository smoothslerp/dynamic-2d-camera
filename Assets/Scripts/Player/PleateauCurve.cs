using UnityEngine;

// given a time, calculates the speed using a quadratic equation
class PlateauCurve {
    
    public static float getSpeed(float t, float max, float min, float k) {
        return max - (max - min) * Mathf.Exp(-k*t);
    }

    public static float getTime(float s, float max, float min, float k) {
        float Y = (max - s)/(max - min);
        float res = -Mathf.Log(Y)/(k);

        return res;
    }
}