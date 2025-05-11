using System.Collections;
using UnityEngine;

public class WatcherScript : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60f;
    [SerializeField] private float maxAngle = 90f;
    private float currentAngle = 0f;
    private bool rotatingForward = true;
    private bool isDelaying = false;

    void Update()
    {
        if (isDelaying) return;

        float delta = rotateSpeed * Time.deltaTime;
        if (rotatingForward)
        {
            currentAngle += delta;
            if (currentAngle >= maxAngle)
            {
                currentAngle = maxAngle;
                StartCoroutine(Delay());
                rotatingForward = false;
            }
        }
        else
        {
            currentAngle -= delta;
            if (currentAngle <= 0f)
            {
                currentAngle = 0f;
                StartCoroutine(Delay());
                rotatingForward = true;
            }
        }

        transform.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    IEnumerator Delay()
    {
        isDelaying = true;
        yield return new WaitForSeconds(1f);
        isDelaying = false;
    }
}
