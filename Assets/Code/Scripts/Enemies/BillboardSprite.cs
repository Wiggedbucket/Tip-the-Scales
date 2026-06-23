using UnityEngine;

public class BillboardSprite : MonoBehaviour
{

    [Header("Walk Wobble")]
    public float wobbleAmount = 10f;
    public float wobbleSpeed = 6f;
    public bool isMoving = false;

    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (_player == null) return;

        Quaternion billboardRot = Quaternion.LookRotation(transform.position - _player.position);

        if (isMoving)
        {
            float tilt = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            Quaternion wobble = Quaternion.Euler(0f, 0f, tilt);
            transform.rotation = billboardRot * wobble;
        }
        else
        {
            transform.rotation = billboardRot;
        }
    }
}