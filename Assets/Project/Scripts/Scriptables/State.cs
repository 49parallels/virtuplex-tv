using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class State : ScriptableObject
{
    public bool IsPanrotating = false;
    public bool IsRelocating = false;
    public bool IsSpawned = false;
}
