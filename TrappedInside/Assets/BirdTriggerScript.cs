using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdTriggerScript : MonoBehaviour
{
    public Transform birdPrefab;

    private List<Transform> birds = new List<Transform>();
    private Vector3 birdSpeed = Vector3.zero;
    private float triggerEnteredTime = 0.0f;
    private readonly float birdLifeTime = 10.0f;
    private bool alreadyTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        birdSpeed = new Vector3(1.5f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var bird in birds)
        {
            bird.Translate(birdSpeed * Time.deltaTime);
        }

        if (alreadyTriggered && Time.time - triggerEnteredTime > birdLifeTime)
        {
            foreach (var bird in birds)
            {
                Destroy(bird.gameObject);
            }
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyTriggered)
            return;

        if (collision.gameObject.tag == "Player")
        {
            alreadyTriggered = true;
            triggerEnteredTime = Time.time;
            StartCoroutine(InstantiateBirds());
        }
    }

    private IEnumerator InstantiateBirds()
    {
        for (int i = 0; i < 8; i++)
        {
            var rndX = RandomNumber.Next(-5, 5) / 10.0f;
            var rndY = RandomNumber.Next(-5, 5) / 20.0f;
            birds.Add(Instantiate(birdPrefab, transform.position - new Vector3(3f + rndX, 0.5f + rndY), Quaternion.identity));
            //Don't create birds exacly at the same time so that they don't flap wings simultaneously
            yield return new WaitForSeconds(0.1f);
        }
    }
}
