using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private HidableWall[] hidableWalls;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) HideWalls();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) ShowWalls();
    }

    [ContextMenu("HideWalls")]
    private void HideWalls()
    {
        for (int i = 0; i < hidableWalls.Length; i++)
        {
            if (hidableWalls[i] == null) continue;
            hidableWalls[i].Hide();
        }
    }

    [ContextMenu("ShowWalls")]
    private void ShowWalls()
    {
        for (int i = 0; i < hidableWalls.Length; i++)
        {
            if (hidableWalls[i] == null) continue;
            hidableWalls[i].Show();
        }
    }
}