using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchElementController : MonoBehaviour
{
    public GameObject[] elementPrefabs;
    public Vector2 areaSize;
    public int maxNumberOfElements = 40;
    public float elementCreationInterval = 0.2f;
    public float blinkingInterval = 0.05f;

    private float _previousElementCreationTime = -1;
    private float _previousBlinkTime = -1;
    private List<GameObject> _elements = new List<GameObject>();

    void Update()
    {
        if(elementCreationIntervalPassed && !MaxElementCountReached)
        {
            CreateNewElement();
            ResetPreviousElementCreationTime();
        }

        if(BlinkingIntervalPassed)
        {
            BlinkRandomElement();
            ResetPreviousBlinkTime();
        }
    }

    bool elementCreationIntervalPassed => Time.time > _previousElementCreationTime + elementCreationInterval;

    bool MaxElementCountReached => _elements.Count >= maxNumberOfElements;

    bool BlinkingIntervalPassed => Time.time > _previousBlinkTime + blinkingInterval;

    void ResetPreviousElementCreationTime() => _previousElementCreationTime = Time.time;

    void ResetPreviousBlinkTime() => _previousBlinkTime = Time.time;

    void BlinkRandomElement()
    {
            var index = Random.Range(0, _elements.Count);
            _elements[index].SetActive(!_elements[index].activeSelf);
    }

    void CreateNewElement()
    {
            var elementPosition = new Vector3(
                GetRandom(transform.position.x, areaSize.x), 
                GetRandom(transform.position.y, areaSize.y), 
                0);

            var prefab = elementPrefabs[Random.Range(0, elementPrefabs.Length)];

            _elements.Add(Instantiate(prefab, elementPosition, Quaternion.identity));
    }

    float GetRandom(float centerPoint, float size) => centerPoint + Random.Range(0f, size) - size / 2;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
