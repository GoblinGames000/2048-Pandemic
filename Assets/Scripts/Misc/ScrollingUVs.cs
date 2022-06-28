﻿using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    private string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        gameObject.GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
    }
}
