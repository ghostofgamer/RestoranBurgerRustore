using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;
using Zenject;

public class DebitCard : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;

    private Buyer owner;
    public Buyer Owner => owner;

    private LevelController level;

    /*
    [Inject]
    private void Construct(LevelController level)
    {
        this.level = level;
    }
    */

    public void Init(Buyer owner)
    {
        this.owner = owner;

        touchInteractive.OnClickEvent += () =>
        {
            //level.OnClickDebitCard(this);
        };
    }
}