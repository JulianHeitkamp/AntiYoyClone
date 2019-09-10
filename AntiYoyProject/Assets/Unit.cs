using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "ScriptableObjects/UnitScriptableObject")]
public class Unit : ScriptableObject
{
    public int level;
    public int cost;
    public GameObject objectToSpawn;
    public Sprite sprite;
    public bool type; // true unit, false something else
    
}
