using UnityEngine;

public class WorldShiftTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WorldShiftManager manager = FindFirstObjectByType<WorldShiftManager>();
            if (manager != null)
                manager.SetCanShift(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WorldShiftManager manager = FindFirstObjectByType<WorldShiftManager>();
            if (manager != null)
                manager.SetCanShift(false);
        }
    }
}
