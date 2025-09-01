using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HatPicker : MonoBehaviour
{
    private Renderer hatRenderer;
    private SphereCollider sphereCollider;

    private void Awake()
    {
        hatRenderer = GetComponent<Renderer>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void OnMouseOver()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()){
            return;
        }
        transform.localScale = new Vector3(0.65f, .65f, .65f);
    }

    private void OnMouseExit()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        PlayerOutfit.Instance.hatIndex = Hats.hats.IndexOf(this);
    }

    public void DeactivateHat()
    {
        Color color = hatRenderer.material.color;
        color.a = .3f;
        hatRenderer.material.color = color;
        sphereCollider.enabled = false;
    }

    public void ActivateHat()
    {
        sphereCollider.enabled = true;
        Color color = hatRenderer.material.color;
        color.a = 1f;
        hatRenderer.material.color = color;
    }
}
