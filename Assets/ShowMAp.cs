using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class ShowMAp : MonoBehaviour
{
    public static ShowMAp Instance;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject Map;
    public GameObject[] MapsGreen;
  //  public dotwee
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Image FilImg;
    public void ShowBar(float index)
    {
        //if (index % 5 == 0)
        //{
        //    FilImg.fillAmount = 1;
        //}
        //else
        //{
        //    FilImg.fillAmount = (index % 5f) / 5f;
        //}
    }
    public TextMeshProUGUI Count;
    public void ShowMap(int index)
    {
        ShowBar(index);
        int on = 0;
        for (int i = 1; i <= index; i++)
        {

            if (index == 1)
            {
                Map.SetActive(true);
                this.Invoke(() => { Map.SetActive(false); }, 5f);
            }
            if (i % 5 == 0)
            {
                Count.text = i.ToString();

                int bar = 0;
                if (index == 5)
                {
                    bar = 1;
                }
                else if (index == 10) bar = 2;
                else if (index == 15) bar = 3;
                else if (index == 20) bar = 4;
                else if (index == 25) bar = 5;
                else if (index == 30) bar = 6;
                else if (index == 35) bar = 7;
                else if (index == 40) bar = 8;

                FilImg.fillAmount = (bar) / 8f;
                if (on < MapsGreen.Length)
                {
                    MapsGreen[on].GetComponent<Image>().color = Color.white;
                    MapsGreen[on].GetComponent<DOTweenVisualManager>().enabled = true;
                    on++;
                }

            }
            if (index % 5 == 0)

            {
                Map.SetActive(true);
                this.Invoke(() => { Map.SetActive(false); }, 5f);
            }
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
