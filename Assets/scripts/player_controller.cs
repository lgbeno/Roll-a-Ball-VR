using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class player_controller : MonoBehaviour {
    public float speed;
    private Rigidbody rb;
    private int count;
    public Text countText;
    public Text WinText;

    void Start () {
            rb = GetComponent<Rigidbody>();
            count = 0;
            setCountText();
            WinText.text = "";
        }
    void FixedUpdate () {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(speed*moveHorizontal, 0, speed*moveVertical);

            rb.AddForce(movement);
        }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            setCountText();
            if (count>=21)
            {
                WinText.text = "You won!";
            }
        }
            
    }
    
    void setCountText()
    {
        countText.text = "Count: " + count.ToString();
    }
}

//Destroy(other.gameObject);