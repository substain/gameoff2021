using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShip : MonoBehaviour
{
    private const float MOVE_DUR = 2.0f;
    private Timer timer;
    private float moveDir = 1.0f;

    [SerializeField]
    private AudioClip onShipClip;

    [SerializeField]
    private AudioClip offShipClip;

    void Start()
    {
        timer = gameObject.AddComponent<Timer>();
        MoveSide();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(0, 0, moveDir).normalized * Time.fixedDeltaTime * 0.5f;
        gameObject.transform.Translate(movement);
    }

    private void MoveSide()
    {
        moveDir *= -1;
        timer.Init(MOVE_DUR, MoveSide);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.transform.parent = gameObject.transform;
            GameManager.Instance.StartBackgroundMusicClip(onShipClip);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.transform.parent = null;
            GameManager.Instance.StartBackgroundMusicClip(offShipClip);
        }
    }
}
