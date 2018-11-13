using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PotentialField()
    {

        GameObject canvasGO = new GameObject();
        RectTransform canvasRT = canvasGO.AddComponent<RectTransform>();
        Canvas canvasCV = canvasGO.AddComponent<Canvas>();
        canvasCV.renderMode = RenderMode.ScreenSpaceCamera;
        Vector3 pos = Camera.main.transform.position;
        pos += Camera.main.transform.forward * 10.0f;
        canvasCV.worldCamera = Camera.main;

        GameObject buttonGO = new GameObject();
        RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
        buttonRT.SetParent(canvasRT);
        buttonRT.sizeDelta = new Vector2(128.0f, 128.0f);
        buttonRT.localPosition = new Vector3(0f, 0f, -309.375f);
        Button buttonBU = buttonGO.AddComponent<Button>();
        buttonBU.onClick.AddListener(() => { Debug.Log("button clicked"); });
        Image buttonIM = buttonGO.AddComponent<Image>();
        buttonIM.sprite = Resources.Load("buttonSprite", typeof(Sprite)) as Sprite;

        // Create a new texture and assign it to the renderer's material


        Texture2D texture = new Texture2D(128, 128);
        buttonIM.material.mainTexture = texture;
        // Fill the texture with Sierpinski's fractal pattern!
        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                Color color = ((x & y) != 0 ? Color.white : Color.gray);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }
}
