using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// Retorna un objecte a la seva posició original.
/// </summary>
public class ReturnToSlot : MonoBehaviour
{
    // Assignació de la posició original.
    private Transform assignedSlot;

    // Component de agafament.
    private XRGrabInteractable grab;

    // Component de física.
    private Rigidbody rb;

    // Inicialització.
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
            grab.selectExited.AddListener(OnRelease);
    }

    /// <summary>
    /// Assigna la posició original.
    /// </summary>
    /// <param name="slot">Posició original.</param>
    public void AssignSlot(Transform slot)
    {
        assignedSlot = slot;
    }

    /// <summary>
    /// Quan es deixa anar l'objecte.
    /// </summary>
    /// <param name="args">Arguments de l'esdeveniment.</param>
    private void OnRelease(SelectExitEventArgs args)
    {
        // Mou l'objecte de nou a la posició original.
        StartCoroutine(MoveBackToSlot());
    }

    /// <summary>
    /// Mou l'objecte de nou a la posició original.
    /// </summary>
    private IEnumerator MoveBackToSlot()
    {
        // Desactiva la física.
        rb.isKinematic = true;

        // Obté la posició i rotació inicials.
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // Obté el temps i la durada.
        float time = 0f;
        float duration = 0.5f;

        // Obté la posició i rotació finals.
        Vector3 targetPos = assignedSlot.position;
        Quaternion targetRot = assignedSlot.rotation;

        // Mou l'objecte.
        while (time < duration)
        {
            float t = time / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            time += Time.deltaTime;
            yield return null;
        }

        // Mou l'objecte a la posició i rotació finals.
        transform.position = targetPos;
        transform.rotation = targetRot;

        // Assigna el pare.
        transform.SetParent(assignedSlot);

        // Mou l'objecte a la posició i rotació zero.
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Activa la física.
        rb.isKinematic = false;
    }

    /// <summary>
    /// Quan es destrueix l'objecte.
    /// </summary>
    private void OnDestroy()
    {
        // Elimina el listener de l'esdeveniment de sortida.
        if (grab != null)
            grab.selectExited.RemoveListener(OnRelease);
    }
}
