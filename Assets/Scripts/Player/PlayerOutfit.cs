using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutfit : NetworkBehaviour
{
    public static PlayerOutfit Instance;
    [Networked(OnChanged = nameof(OnHatChange))] public int hatIndex { get; set; }
    [SerializeField] private List<GameObject> prefabHats = new List<GameObject>();
    
    static void OnHatChange(Changed<PlayerOutfit> change)
    {
        if (change.Behaviour.hatIndex == -1) return;
        HatPicker _currentHat = Hats.hats[change.Behaviour.hatIndex];
        change.LoadOld();
        HatPicker _previousHat = null;
        if (change.Behaviour.hatIndex > -1)
        {
            _previousHat = Hats.hats[change.Behaviour.hatIndex];
        }

        foreach(GameObject hat in change.Behaviour.prefabHats)
        {
            if (_previousHat != null)
            {
                if (hat.name == _previousHat.name)
                {
                    if (hat.gameObject.activeInHierarchy)
                    {
                        hat.gameObject.SetActive(false);
                        _previousHat.ActivateHat();
                    }
                }
            }

            if(hat.name == _currentHat.name)
            {
                _currentHat?.DeactivateHat();
                hat.gameObject.SetActive(true);
            }
        }
    }

    private void Start()
    {
        if (this.HasStateAuthority)
        {
            Instance = this;
            hatIndex = -1;
        }
    }
}
