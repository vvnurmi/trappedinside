using UnityEngine;

public class HornetSpawner : MonoBehaviour
{

    public GameObject hornet;
    public float instantiateTimeout = 5.0f;
    public int maxNumberOfHornets = 5;

    private float previousInstantiateTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - previousInstantiateTime > instantiateTimeout)
        {
            var numberOfHornets = GameObject.FindGameObjectsWithTag("Hornet").Length;
            if (numberOfHornets < maxNumberOfHornets)
            {
                Instantiate(hornet, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
                previousInstantiateTime = Time.time;
            }
        }
    }
}
