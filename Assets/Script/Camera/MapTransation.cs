using Unity.Cinemachine;
using UnityEngine;

public class MapTransanction : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private PolygonCollider2D mapBoundary;   
    [SerializeField] private Direction direction;           
    public float pushForce = 5f;
    private CinemachineConfiner2D confiner;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundary;

            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += pushForce;
                break;

            case Direction.Down:
                newPos.y -= pushForce;
                break;

            case Direction.Left:
                newPos.x -= pushForce;
                break;

            case Direction.Right:
                newPos.x += pushForce;
                break;
        }

        player.transform.position = newPos;
    }
}
