using System.Collections;
using UnityEngine;

namespace Survival2048
{
    public class ZUnit : MonoBehaviour
    {
        [Header("2048 Properties")]
        #region 2048 Single Unit Properties / Grid Unit Properties
        public TextMesh textNumber;
        public SpriteRenderer bg;
        public Color[] textColour = new Color[11];
        public Color[] unitColour = new Color[11];
        private int currentColourIndex = 0;
        public int gridValue = 0;
        #endregion

        [Space(10)]
        [Header("Survival Mode Properties")]
        #region Survival Mode Properties
        public GameObject Player;
        public GameObject Enemy;
        #endregion

        [Space(10)]
        [Header("Effects Properties")]
        #region Effects Properties
        public GameObject parClash;
        public GameObject parMerge;
        public GameObject parHit;
        public Animation animZunit;
        public Animation animGlow;
        private bool castingEfx = false;
        #endregion

        #region 2048 Functions
        public void SetUnitValue(int value, float delay)
        {
            StartCoroutine(SetUnitValueCoroutine(value, delay));
        }

        private IEnumerator SetUnitValueCoroutine(int value, float delay)
        {
            yield return new WaitForSeconds(delay);

            gridValue = value;

            if (value == 1)
            {
                textNumber.text = "";
                Player.SetActive(true);
                Enemy.SetActive(false);
            }
            else if (value == 3)
            {
                textNumber.text = "";
                Player.SetActive(false);
                Enemy.SetActive(true);
            }
            else
            {
                textNumber.text = value.ToString();
                textNumber.color = textColour[getColourIndex(value)];
                bg.color = unitColour[getColourIndex(value)];
            }
        }
        #endregion

        #region Beautify + Particle Effects (where you'll want to modify for your own game)
        private int getColourIndex(int gridValue)
        {
            int _colourIndex;

            switch (gridValue)
            {
                case 2:
                    _colourIndex = 0;
                    break;
                case 4:
                    _colourIndex = 1;
                    break;
                case 8:
                    _colourIndex = 2;
                    break;
                case 16:
                    _colourIndex = 3;
                    break;
                case 32:
                    _colourIndex = 4;
                    break;
                case 64:
                    _colourIndex = 5;
                    break;
                case 128:
                    _colourIndex = 6;
                    break;
                case 256:
                    _colourIndex = 7;
                    break;
                case 512:
                    _colourIndex = 8;
                    break;
                case 1024:
                    _colourIndex = 9;
                    break;
                case 2048:
                    _colourIndex = 10;
                    break;
                default:
                    _colourIndex = 11;
                    break;
            }

            currentColourIndex = _colourIndex;
            return _colourIndex;
        }

        public void ShowHitEfx(Z2048.Direction dir, float delay)
        {
            if (!castingEfx)
            {
                castingEfx = true;
                StartCoroutine(ShowHitEfxCoroutine(dir, delay));
            }
        }

        private IEnumerator ShowHitEfxCoroutine(Z2048.Direction dir, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 _spawnPoint = new Vector3();

            if (dir == Z2048.Direction.up)
            {
                _spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 0.5f);
            }

            if (dir == Z2048.Direction.down)
            {
                _spawnPoint = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z - 0.5f);
            }

            if (dir == Z2048.Direction.left)
            {
                _spawnPoint = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f);
            }

            if (dir == Z2048.Direction.right)
            {
                _spawnPoint = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z - 0.5f);
            }

            GameObject _tempParClash = Instantiate(parClash, _spawnPoint, Quaternion.identity);
            Destroy(_tempParClash, 1);
            castingEfx = false;
        }

        public void ShowMergeEfx(float delay)
        {
            StartCoroutine(ShowMergeEfxCoroutine(delay));
        }

        private IEnumerator ShowMergeEfxCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Vector3 _spawnPoint = new Vector3();
            _spawnPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            GameObject _tempParMerge = Instantiate(parMerge, _spawnPoint, Quaternion.identity);
            Destroy(_tempParMerge, 1);
            animZunit.Play("zUnit_ScaleBounce");
            animGlow.Play("zUnit_Glow");
            iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f), 0.15f);
            Invoke("RestoreMaterial", 0.15f);
        }

        public void ShowHitEfx()
        {
            Instantiate(parHit, transform.position, transform.rotation);
        }
        
        private void RestoreMaterial()
        {
            iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), 0.15f);
        }
        #endregion
    }
}
