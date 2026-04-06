using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeFlash : MonoBehaviour
{
    public Renderer rend;
    public Color flashColor = Color.cyan;
    public float flashTime = 0.08f;

    private Material mat;
    private Coroutine flashCoroutine;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.DisableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", flashColor * 5f);

        yield return new WaitForSeconds(flashTime);

        mat.SetColor("_EmissionColor", Color.black * 0f);
        mat.DisableKeyword("_EMISSION");
    }
}