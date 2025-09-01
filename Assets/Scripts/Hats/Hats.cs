using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hats : MonoBehaviour
{
    [SerializeField] public static List<HatPicker> hats;

    private void Awake()
    {
        hats = new List<HatPicker>();

        foreach (Transform child in transform)
        {
            child.AddComponent<HatPicker>();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            hats.Add(transform.GetChild(i).GetComponent<HatPicker>());
        }
    }
}
