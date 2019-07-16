using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPanel : MonoBehaviour {

    Vector3 point;

    private GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
      
        Event currentEvent = Event.current;
        //Vector2 mousePos = new Vector2();
        guiStyle.fontSize = 30;
        guiStyle.normal.textColor = Color.white;
        GUILayout.BeginArea(new Rect(20, 20, 500, 300));
        GUILayout.Label("mouse: " + GetWorldPositionOnPlane(Input.mousePosition, 0), guiStyle);
        GUILayout.Label("FPS: " + 1 / Time.deltaTime, guiStyle);
       
       // float a = Camera.main.pixelWidth;
        //GUILayout.Label("a" + furthestMous, guiStyle);

        GUILayout.EndArea();
    }



    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    
}
