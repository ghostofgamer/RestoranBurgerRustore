using UnityEngine;

public class PackingPaperItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Dragger dragger;

    public Item Item => item;
    public Dragger Dragger => dragger;
}