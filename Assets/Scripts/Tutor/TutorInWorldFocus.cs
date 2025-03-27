using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorInWorldFocus : MonoBehaviour
{
    [SerializeField] private Collider col;
    [SerializeField] private Transform focusTran;

    public Collider Col => col;
    public Transform FocusTran => focusTran;
}