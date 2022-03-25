using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetToFollow;
    [SerializeField]
    Vector3 offset, velocity;

    [SerializeField]
    bool isLevelStart, isLevelPaused, isLevelEnd;
    [SerializeField]
    float smoothTime;

    void Start()
    {
        velocity = Vector3.zero;
        GameManager.game.LevelStartEv += LevelStarter;
        GameManager.game.LevelPauseEV += LevelPauser;
        GameManager.game.LevelEndEV += LevelEnder;
        if (offset == Vector3.zero && targetToFollow != null)
        {
            offset = Camera.main.transform.position - targetToFollow.position;
        }
    }

    void LateUpdate()
    {
        if (!isLevelPaused && !isLevelEnd && isLevelStart)
        {
            var t = targetToFollow.position + offset;
            Camera.main.transform.position = Vector3.SmoothDamp(transform.position, t, ref velocity, smoothTime);
        }
        else if(isLevelEnd)
        {
            // Debug.Log("there is some problem");
        }

    }

    void LevelStarter()
    {
        isLevelStart = true;
    }

    void LevelPauser()
    {
        isLevelPaused = true;
    }

    void LevelEnder()
    {
        isLevelEnd = true;
        StartCoroutine(LevelEndOperation());

    }

    IEnumerator LevelEndOperation()
    {
        //Camera.main.transform.LookAt(targetToFollow);
        yield return new WaitForSeconds(1f);
        //var tempTr = transform;
        //tempTr.LookAt(targetToFollow);
        var dir = targetToFollow.position - Camera.main.transform.position;
        while(Camera.main.fieldOfView > 42)
        {
            Camera.main.fieldOfView -= 0.1f;
            //Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, dir, 5 * Time.deltaTime);
            Camera.main.transform.forward = Vector3.Slerp(Camera.main.transform.forward, dir, 1 * Time.deltaTime);
            //Camera.main.transform.LookAt(targetToFollow);
            yield return null;
        }
    }

    void LevelRestarter()
    {
        isLevelPaused = false;
    }
}
