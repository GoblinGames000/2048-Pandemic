using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Survival2048
{
    public class ZTrophy : MonoBehaviour
    {
        [Header("Trophy Properties")]
        #region Trophy Properties
        public Image trophySprite;
        public Text trophyNumber;
        public Image progressBar;
        public Color unlockedColour;
        #endregion

        #region Local Score
        private float fromScore = 0;
        private float toScore = 100;
        private float maxScore = 100;
        #endregion

        [Space(10)]
        [Header("Effects Properties")]
        #region Effects Properties
        public GameObject efxUnlocked;
        public GameObject efxShining;
        #endregion

        #region Setting trophy status
        public void SetNumber(int _number)
        {
            trophyNumber.text = _number.ToString();
        }

        public void Locked()
        {
            trophySprite.color = Color.black;
            efxShining.SetActive(false);
        }

        public void UnLocked()
        {
            trophySprite.color = unlockedColour;
            efxShining.SetActive(true);
        }
        #endregion

        #region Unlocking Functions
        #region Setting trophy unlock progress
        public void SetupTrophyProgress(int _score, int _maxScore)
        {
            maxScore = _maxScore;
            Value = _score;
        }

        public void SetProgress(int _fromScore, int _toScore, int _maxScore)
        {
            fromScore = (float)_fromScore;
            toScore = (float)_toScore;
            UpdateProgress();
        }

        private int Value
        {
            get
            {
                if (progressBar != null)
                    return (int)(progressBar.fillAmount * 100);
                else
                    return 0;
            }
            set
            {
                if (progressBar != null)
                    progressBar.fillAmount = value / maxScore;
            }
        }

        private void UpdateProgress()
        {
            Hashtable param = new Hashtable();
            param.Add("from", fromScore);
            param.Add("to", toScore);
            param.Add("time", 0.5f);
            param.Add("onupdate", "TweenValue");
            param.Add("onComplete", "OnFullProgress");
            param.Add("onCompleteTarget", gameObject);
            iTween.ValueTo(gameObject, param);
        }

        private void TweenValue(int val)
        {
            Value = val;
        }
        #endregion

        //This is where you'll want to reward your player
        private void OnFullProgress()
        {
            if (toScore >= maxScore)
            {
                UnLocked();
                Vector3 posOffset = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20);
                Instantiate(efxUnlocked, posOffset, transform.rotation, transform);
            }
        }
        #endregion
    }
}
