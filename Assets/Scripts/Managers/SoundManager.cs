using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survival2048
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        #region Inspector Variables

        [Header("Audio Properties")]
        public AudioSource background;
        public AudioSource sfxUnlock;
        public AudioSource sfxStartGame;
        public AudioSource sfxMovingTile;
        public AudioSource sfxCombineTile;
        public AudioSource sfxGameOver;
        public AudioSource sfxSpawnGrid;
        public AudioSource sfxHit;

        #endregion

        public MainMenuUIManager mainMenuUIManager;
        public List<AudioClip> DifferentBackground; 
        public int makevibration = 0; 
        public int backgroundm = 0;
        private bool musicOn = true;
        public void Awake()
        {
            Instance = this;
        }

        public void MusicOn()
        {
            if (sfxUnlock.volume > 0)
            {
                background.volume = 1;
            }
            musicOn = true;

            mainMenuUIManager.musicOn.GetComponent<Button>().interactable = true;
            mainMenuUIManager.musicOff.SetActive(false);

            backgroundm = 1;
            PlayerPrefs.SetInt("backgroundm", backgroundm);
        }

        public void MusicOff()
        {
            background.volume = 0;
            musicOn = false;

            mainMenuUIManager.musicOn.GetComponent<Button>().interactable = false;
            mainMenuUIManager.musicOff.SetActive(true);

            backgroundm = 0;
            PlayerPrefs.SetInt("backgroundm", backgroundm);
        }

        public void VolumeOn()
        {
            if (musicOn)
            {
                background.volume = 1;
            }
            sfxUnlock.volume = 1;
            sfxStartGame.volume = 1;
            sfxMovingTile.volume = 1;
            sfxCombineTile.volume = 1;
            sfxGameOver.volume = 1;
            sfxSpawnGrid.volume = 1;
            sfxHit.volume = 1;
            background.Play();
            FindObjectOfType<AudioListener>().enabled = true;
            mainMenuUIManager.volumeOn.SetActive(true);
            mainMenuUIManager.volumeOff.SetActive(false);
        }
        public void SetBackgroundLevel(int Levelindex)
        {
            int index = 0;
            if (Levelindex == 4) index = 1;
            if (Levelindex == 9) index = 1;
            if (Levelindex == 14) index = 1;
            if (Levelindex == 19) index = 1;
            if (Levelindex == 24) index = 1;
            if (Levelindex == 29) index = 1;
            if (Levelindex == 34) index = 1;
            if (Levelindex == 39) index = 1;
        
            background.clip = DifferentBackground[index];
            background.Play();
        }
        public void VolumeOff()
        {
            background.volume = 0;
            sfxUnlock.volume = 0;
            sfxStartGame.volume = 0;
            sfxMovingTile.volume = 0;
            sfxCombineTile.volume = 0;
            sfxGameOver.volume = 0;
            sfxSpawnGrid.volume = 0;
            sfxHit.volume = 0;
            FindObjectOfType<AudioListener>().enabled = false;

            mainMenuUIManager.volumeOn.SetActive(false);
            mainMenuUIManager.volumeOff.SetActive(true);
        }

        public void VibrateOn()
        {
            mainMenuUIManager.vibrateOn.GetComponent<Button>().interactable = true;
            mainMenuUIManager.vibrateOff.SetActive(false);
            makevibration = 1;
            PlayerPrefs.SetInt("vibrate", makevibration);
        }
        public void VibrateOff()
        {
            mainMenuUIManager.vibrateOn.GetComponent<Button>().interactable = false;
            mainMenuUIManager.vibrateOff.SetActive(true);
            makevibration = 0;
            PlayerPrefs.SetInt("vibrate", makevibration);
        }

        public void PlayUnlockMusic()
        {
            sfxUnlock.Play();
        }

        public void PlayStartGameMusic()
        {
            sfxStartGame.Play();
        }

        public void PlayMovingTileMusic()
        {
            sfxMovingTile.pitch = 1;
            sfxMovingTile.Play();
        }
        public void PlayCombineTileMusic()
        {
            sfxCombineTile.pitch = 1;
            sfxCombineTile.Play();
        }
        public void PlayGameOverMusic()
        {
            sfxGameOver.pitch = 1;
            sfxGameOver.Play();
        }

        public void PlaySpawnGridMusic()
        {
            sfxSpawnGrid.pitch = 1;
            sfxSpawnGrid.Play();
        }

        public void PlayHitMusic()
        {
            sfxHit.Play();
        }

    }
    
}
