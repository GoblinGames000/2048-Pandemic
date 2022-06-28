using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Survival2048
{
    public class MainMenuUIManager : MonoBehaviour
    {
        #region Inspector Variables

        public GameObject settingsPanel;
        public GameObject volumeOn;
        public GameObject volumeOff;
        public GameObject vibrateOn;
        public GameObject vibrateOff;
        public GameObject musicOn;
        public GameObject musicOff;

        #endregion

        [SerializeField]
        private Z2048 z2048;
        private SoundManager soundManager;

        #region Unity Methods
        private void Awake()
        {
            soundManager = GameObject.FindWithTag("Sound Manager").GetComponent<SoundManager>();
            volumeOff.SetActive(false);
            musicOff.SetActive(false);
            
            if(soundManager.makevibration == 0)
            {
                vibrateOff.SetActive(true);
                vibrateOn.GetComponent<Button>().interactable = false;
            }
            else
            {
                vibrateOff.SetActive(false);
                vibrateOn.GetComponent<Button>().interactable = true;
            }
            settingsPanel.SetActive(false);
        }

        #endregion


        #region Public Methods
        public void OpenSettings()
        {
            settingsPanel.SetActive(true);
        }

        public void Playstore()
        {
            Application.OpenURL("url");
        }

        #endregion
    }
}
