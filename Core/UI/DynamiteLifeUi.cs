using Core.Data;
using Mehanics.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class DynamiteLifeUi : MonoBehaviour
    {
        private Image _dynamiteFill;
        private PlayerDeath _playerDeath;

        private void Start()
        {
            _dynamiteFill = GameObject.Find("DynamiteFill").GetComponent<Image>();
            _playerDeath = FindObjectOfType<PlayerDeath>();
        }

        private void Update()
        {
            _dynamiteFill.fillAmount =
                Mathf.Clamp(DataController.Health, 0, _playerDeath.LifeTime) / _playerDeath.LifeTime;
        }
    }
}