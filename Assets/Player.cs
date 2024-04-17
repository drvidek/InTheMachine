using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Test
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        public float speed = 5f;
        public Tilemap testTilemap;
        private Rigidbody2D rb;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 inputThisFrame = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            inputThisFrame.Normalize();

            Move(inputThisFrame * speed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3Int tile = testTilemap.WorldToCell(transform.position);
                testTilemap.SetTile(tile, null);
            }
        }

        private void Move(Vector2 velocity)
        {
            rb.velocity = velocity;
        }


    }
}