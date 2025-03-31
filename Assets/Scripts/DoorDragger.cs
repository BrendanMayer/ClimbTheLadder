using UnityEngine;

public class DoorDragger : MonoBehaviour
{
    private Rigidbody rb;
    private bool isDragging = false;
    private Camera cam;
    Player player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    

    void Update()
    {
        
        
    }
}
