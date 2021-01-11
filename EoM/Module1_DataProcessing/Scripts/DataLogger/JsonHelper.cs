using System;
using UnityEngine;
/// Class to convert an array (not a List<>) into JSON
/// EXAMPLE:
// Player[] playerInstance = new Player[2];

// playerInstance[0] = new Player();
// playerInstance[0].playerId = "8484239823";
// playerInstance[0].playerLoc = "Powai";

// playerInstance[1] = new Player();
// playerInstance[1].playerId = "512343283";
// playerInstance[1].playerLoc = "User2";

// //Convert to JSON
// string playerToJson = JsonHelper.ToJson(playerInstance, true);
// Debug.Log(playerToJson);
// // Deserialize
// Player[] player = JsonHelper.FromJson<Player>(jsonString);
// Debug.Log(player[0].playerLoc);
// Debug.Log(player[1].playerLoc);
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}