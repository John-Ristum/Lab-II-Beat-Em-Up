using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : GameBehaviour
{
    public Image healthFill;

    public void UpdateHealthBar(int _health, int _maxHealth)
    {
        healthFill.fillAmount = MapTo01(_health, 0, _maxHealth);
    }
}
