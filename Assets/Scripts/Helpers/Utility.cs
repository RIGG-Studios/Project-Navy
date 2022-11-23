using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class Utility : MonoBehaviour
{
    public static void SafeDestroy(params UnityEngine.Object[] components)
    {
        if (!Application.isPlaying)
        {
            foreach (var component in components)
            {
                if (component != null) UnityEngine.Object.DestroyImmediate(component);
            }

        }
        else
        {
            foreach (var component in components)
            {
                if (component != null) UnityEngine.Object.Destroy(component);
            }
        }
    }
    
    public static object[] BuildPackage(EventCodes code, object[] arguements)
    {
        if (code == EventCodes.AddPlayer) return new object[] { (string)arguements[0], 
            (int)arguements[1], (short)0, (short)0, (int)arguements[2] };
        
        if (code == EventCodes.ChangePlayerStats) return new object[] { (int)arguements[0],
            (int)arguements[1], (int)arguements[2] };
        
        if (code == EventCodes.DamageEntity)  return new object[] { (int)arguements[0], (int)arguements[1], 
            (float)arguements[2] };
        
        if (code == EventCodes.RemovePlayer) return new object[] { (int)arguements[0] };

        if (code == EventCodes.StartMatch) return new object[] { };

        if (code == EventCodes.UpdatePlayers)
        {
            List<NetworkPlayer> info = (List<NetworkPlayer>)arguements[1];

            object[] package = new object[info.Count + 1];

            package[0] = arguements[0];
            for (int i = 0; i < info.Count; i++)
            {
                object[] piece = new object[6];

                piece[0] = info[i].playerName;
                piece[1] = info[i].actorID;
                piece[2] = info[i].kills;
                piece[3] = info[i].deaths;
                piece[4] = info[i].viewID;

                package[i + 1] = piece;
            }

            return package;
        }

        Debug.Log("Couldn't build package.");
        return null;
    }
}
