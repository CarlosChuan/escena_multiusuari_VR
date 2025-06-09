using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ReturnToSlot : MonoBehaviour
{
    private Transform assignedSlot;
    private XRGrabInteractable grab;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
            grab.selectExited.AddListener(OnRelease);
    }

    public void AssignSlot(Transform slot)
    {
        assignedSlot = slot;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        StartCoroutine(MoveBackToSlot());
    }

    private IEnumerator MoveBackToSlot()
    {
        rb.isKinematic = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float time = 0f;
        float duration = 0.5f;

        Vector3 targetPos = assignedSlot.position;
        Quaternion targetRot = assignedSlot.rotation;

        while (time < duration)
        {
            float t = time / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        transform.SetParent(assignedSlot); // <-- this keeps it attached again!
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        rb.isKinematic = false;
    }


    private void OnDestroy()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnRelease);
    }
}
