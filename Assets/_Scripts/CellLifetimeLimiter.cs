using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SPPCell))]
public class CellLifetimeLimiter : MonoBehaviour
{
    [SerializeField] private float secondsToLive = 100f;
    [SerializeField] private float randomLifeModificationMax = 20f;

    [SerializeField] private GameObject onCellDeathPrefab;

    private SPPCell cell;
    private float actualTimeToLive;
    private float livedTime;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        cell = GetComponent<SPPCell>();
        onCellDeathPrefab = Instantiate(onCellDeathPrefab, cell.transform.position,Quaternion.identity);
        onCellDeathPrefab.SetActive(false);
        actualTimeToLive = secondsToLive + Random.Range(0, randomLifeModificationMax);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        livedTime += Time.deltaTime;
        if (livedTime >= actualTimeToLive)
        {
            onCellDeathPrefab.transform.position = cell.transform.position;
            onCellDeathPrefab.SetActive(true);
            Destroy(gameObject);
        }
    }
}