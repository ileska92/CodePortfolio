using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomOut : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cmCamera;
    [SerializeField] private float zoomSpeed = 2.5f; //the higher number the slower speed
    [SerializeField] private float lensOrthoSizeStart = 3f;
    [SerializeField] private float lensOrthoSizeEnd = 10f;
    static float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefsSettings.instance.zoomOut == 1)
        {
            cmCamera.m_Lens.OrthographicSize = lensOrthoSizeStart;
            StartCoroutine(ZoomOutLerp(lensOrthoSizeStart, lensOrthoSizeEnd));
        }
    }

    private IEnumerator ZoomOutLerp(float start, float end)
    {
        t = 0f;
        while(cmCamera.m_Lens.OrthographicSize != end)
        {
            cmCamera.m_Lens.OrthographicSize = Mathf.Lerp(start, end, t);
            t += Time.deltaTime / zoomSpeed;
            yield return null;
        }
        yield return null;
    }
}
