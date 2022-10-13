using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Serializer 
{

    static string path => Application.persistentDataPath + "portfolio.save";

    public static void Serialize<T>(T obj)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.OpenOrCreate);
        formatter.Serialize(stream, obj);
        stream.Close();
    }

    public static T Deserialize<T>()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.Open);
        var res = formatter.Deserialize(stream);
        stream.Close();
        return (T)res;
    }
}
