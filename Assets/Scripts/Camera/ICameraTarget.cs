using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraTarget
{
    Transform GetTransform();
    int GetFacingValue();
    Vector2 GetVelocity();
    Vector2 GetMaxSpeed();
}
