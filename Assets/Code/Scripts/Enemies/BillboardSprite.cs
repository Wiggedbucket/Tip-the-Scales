using UnityEngine;

public class BillboardSprite : MonoBehaviour
{

    [Header("Walk Wobble")]
    public float wobbleAmount = 10f;
    public float wobbleSpeed = 6f;
    public bool isMoving = false;

    private Transform _player;
    [SerializeField] private Material BaseSprite;
    [SerializeField] private Material AttackSprite;
    [SerializeField] private Renderer rend;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (_player == null) return;
        Vector3 direction = transform.position - _player.position;
        direction.y = 0f;
        Quaternion billboardRot = Quaternion.LookRotation(direction);

        if (isMoving)
        {
            float tilt = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            Quaternion wobble = Quaternion.Euler(0f, 0f, tilt);
            transform.rotation = billboardRot * wobble;
            rend.material = BaseSprite;
        }
        else
        {
            transform.rotation = billboardRot;
            rend.material= AttackSprite;
        }
    }
}