using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HealthCanvas : NetworkBehaviour
{
    [SerializeField] private Image healthBarImg;
    [Networked(OnChanged = nameof(OnHealthChange))] private int healthValue { get; set; }

    private static void OnHealthChange(Changed<HealthCanvas> change)
    {
        change.Behaviour.healthBarImg.fillAmount = (float)change.Behaviour.healthValue / 100;
    }

    private void Start()
    {
        if (!HasStateAuthority) return;
        healthValue = 100;
        FusionConnection.Instance.gameInputs.Health.Decrease.performed += Decrease_performed;
        FusionConnection.Instance.gameInputs.Health.Increase.performed += Increase_performed;
    }

    private void Increase_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (healthValue < 100)
        {
            healthValue += 5;
        }
    }

    private void Decrease_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (healthValue > 0)
        {
            healthValue -= 5;
        }
    }
}
