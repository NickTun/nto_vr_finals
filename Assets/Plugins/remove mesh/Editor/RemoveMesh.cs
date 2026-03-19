using UnityEngine;
using UnityEditor;

public static class CleanMeshColliders
{
    [MenuItem("Tools/Colliders/Remove Root MeshColliders if children have colliders %&r")]
    static void RemoveRootMeshColliders_WhenChildrenHaveColliders()
    {
        int count = 0;
        Undo.SetCurrentGroupName("Remove unnecessary root MeshColliders");

        foreach (GameObject go in Selection.gameObjects)
        {
            // Skip if object has no MeshCollider at root level
            MeshCollider rootMC = go.GetComponent<MeshCollider>();
            if (rootMC == null) continue;

            // Check if ANY child (direct or deep) has ANY kind of Collider
            Collider[] childColliders = go.GetComponentsInChildren<Collider>(true);
            bool hasChildCollider = false;

            foreach (var col in childColliders)
            {
                // Exclude the root one itself
                if (col.gameObject == go) continue;
                hasChildCollider = true;
                break;
            }

            if (hasChildCollider)
            {
                Undo.DestroyObjectImmediate(rootMC);
                count++;
                Debug.Log($"Removed root MeshCollider from → {go.name}", go);
            }
        }

        if (count == 0)
        {
            Debug.Log("No root MeshColliders were removed (no matching objects or no child colliders found).");
        }
        else
        {
            Debug.Log($"Cleaned {count} root MeshColliders.");
        }
    }

    // Bonus: remove ALL root MeshColliders regardless (more aggressive)
    [MenuItem("Tools/Colliders/Remove ALL Root MeshColliders (dangerous)")]
    static void RemoveAllRootMeshColliders()
    {
        int count = 0;
        foreach (GameObject go in Selection.gameObjects)
        {
            MeshCollider mc = go.GetComponent<MeshCollider>();
            if (mc != null)
            {
                Undo.DestroyObjectImmediate(mc);
                count++;
            }
        }
        Debug.Log($"Removed {count} root MeshColliders (no conditions checked).");
    }
}