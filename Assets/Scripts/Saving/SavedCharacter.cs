using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedCharacter 
{
    public SavedVector3 position, rotation, velocity;
    public int Health;
    public string Guid;

    public SavedCharacter(SavedVector3 position, SavedVector3 rotation, SavedVector3 velocity, int health, string guid)
    {
        this.position = position;
        this.rotation = rotation;
        this.velocity = velocity;
        Health = health;
        Guid = guid;
    }
}

[System.Serializable]
public struct SavedVector3
{
    public float x, y, z;  

    public SavedVector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;  
        z = Z;
    }
}

