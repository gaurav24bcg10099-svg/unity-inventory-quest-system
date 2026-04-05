using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupRange = 3f;
    public Transform holdPoint;

    private Rigidbody heldObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PickupObject();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DropObject();
        }

        HoldObject();
    }

    void PickupObject()
    {
        if (heldObject != null) return;

        Ray ray = new Ray(playerCamera.transform.position,
                          playerCamera.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.isKinematic = true;
            }
        }
    }

    void HoldObject()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
        }
    }

    void DropObject()
    {
        if (heldObject == null) return;

        heldObject.useGravity = true;
        heldObject.isKinematic = false;
        heldObject = null;
    }
}