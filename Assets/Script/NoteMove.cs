using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
    }
}
