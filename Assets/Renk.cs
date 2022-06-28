using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renk : MonoBehaviour
{
    public Color[] renkler;
    Color ilk_renk;
    Color ikinci_renk;
    public Material zemin_material;
    int bir_renk;

    void Start()
    {
        bir_renk = Random.Range(0, renkler.Length);
        zemin_material.color = renkler[bir_renk];
        Camera.main.backgroundColor = renkler[bir_renk];
        ikinci_renk = renkler[Ikinci_renk_belirle()];
    }
    int Ikinci_renk_belirle()
    {
        int ilk_renk;
        if (renkler.Length < 1)
        {
            ilk_renk = bir_renk;
            return ilk_renk;
        }
        ilk_renk = Random.Range(0, renkler.Length);
        while (ilk_renk == bir_renk)
        {
            ilk_renk = Random.Range(0, renkler.Length);
        }
        return ilk_renk;
    }

    void Update()
    {
        Color fark = zemin_material.color - ikinci_renk;
        if (Mathf.Abs(fark.r) + Mathf.Abs(fark.g) + Mathf.Abs(fark.b) < 0.2f)
        {
            ikinci_renk = renkler[Ikinci_renk_belirle()];
        }
        zemin_material.color = Color.Lerp(zemin_material.color, ikinci_renk, 0.003f);
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, ikinci_renk, 0.0007f);
    }
}




