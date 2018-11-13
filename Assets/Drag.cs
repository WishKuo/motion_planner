using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drag : MonoBehaviour {

    Vector3 prePos;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }
    private void OnMouseDown()
    {
        Vector3 mos;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            mos = Input.mousePosition;
            prePos = Camera.main.ScreenToWorldPoint(new Vector3(mos.x, mos.y, 1));
        }
        else
        {
            mos = Input.mousePosition;
            prePos = Camera.main.ScreenToWorldPoint(new Vector3(mos.x, mos.y, 1));
        }
    }

    void OnMouseDrag()
    {
        Vector3 mos, newPos, Offset;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            mos = Input.mousePosition;
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(mos.x, mos.y, 1));
            Offset = newPos - prePos;
            float angle = Convert.ToSingle(Mathf.Atan2((newPos.y - prePos.y), (newPos.x - prePos.x)) / Mathf.Deg2Rad);
            if (!float.IsNaN(angle))
            {
                gameObject.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            
        }
        else
        {
            mos = Input.mousePosition;
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(mos.x, mos.y, 1));
            Offset = newPos - prePos;
            gameObject.transform.parent.position += Offset;
            prePos = Camera.main.ScreenToWorldPoint(new Vector3(mos.x, mos.y, 1));
        }
    }
}
