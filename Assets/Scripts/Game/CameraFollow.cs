using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Riferimento al transform del player
    public Vector3 offset; // Offset della posizione della telecamera rispetto al player
    public float smoothTime = 0.2f; // Tempo di smoothing per il movimento della telecamera
    private Vector3 velocity = Vector3.zero; // Variabile di appoggio per SmoothDamp

    private void FixedUpdate()
    {
        // Calcola la posizione desiderata della telecamera
        Vector3 targetPos = target.position + offset;

        // Interpola in modo fluido la posizione corrente della telecamera con quella desiderata
        Vector3 lerpedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Aggiorna la posizione della telecamera
        transform.position = lerpedPos;
    }
}