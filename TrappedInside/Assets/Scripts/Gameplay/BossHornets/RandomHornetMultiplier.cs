using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHornetMultiplier : MonoBehaviour
{
    public GameObject hornetPrefab;
    public Vector2 areaSize;
    public int maxNumberOfHornets = 20;
    public float hornetCreationInterval = 0.2f;
    public float blinkingInterval = 0.05f;

    private float _previousHornetCreationTime = -1;
    private float _previousBlinkTime = -1;
    private List<GameObject> _hornets = new List<GameObject>();

    void Update()
    {
        if(HornetCreationIntervalPassed && !MaxHornetCountReached)
        {
            CrateNewHornet();
            ResetPreviousHornetCreationTime();
        }

        if(BlinkingIntervalPassed)
        {
            BlinkRandomHornet();
            ResetPreviousBlinkTime();
        }
    }

    bool HornetCreationIntervalPassed => Time.time > _previousHornetCreationTime + hornetCreationInterval;

    bool MaxHornetCountReached => _hornets.Count >= maxNumberOfHornets;

    bool BlinkingIntervalPassed => Time.time > _previousBlinkTime + blinkingInterval;

    void ResetPreviousHornetCreationTime() => _previousHornetCreationTime = Time.time;

    void ResetPreviousBlinkTime() => _previousBlinkTime = Time.time;

    void BlinkRandomHornet()
    {
            var index = (int)Random.Range(0, _hornets.Count - 1);
            _hornets[index].SetActive(!_hornets[index].activeSelf);
    }

    void CrateNewHornet()
    {
            var hornetPosition = new Vector3(
                GetRandom(transform.position.x, areaSize.x), 
                GetRandom(transform.position.y, areaSize.y), 
                0);

            _hornets.Add(Instantiate(hornetPrefab, hornetPosition, Quaternion.identity));
    }

    float GetRandom(float centerPoint, float size) => centerPoint + Random.Range(0, size) - size / 2;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
