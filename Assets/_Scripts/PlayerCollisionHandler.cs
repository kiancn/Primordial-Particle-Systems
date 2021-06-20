using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private GameObject onDestructiveHullContactPrefab;
    [SerializeField] private GameObject onDestructiveHullContactPreSpawn;
    
    void Awake()
    {
        if (onDestructiveHullContactPrefab == null)
        {
            Debug.Log("No destructive impact object assigned. Bailing for safety's sake.");
            this.gameObject.SetActive(false);
        }
        onDestructiveHullContactPreSpawn =
            Instantiate(onDestructiveHullContactPrefab, new Vector3(100, 100, 0), Quaternion.identity);
        onDestructiveHullContactPreSpawn.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        playerStats.HullPointChange(-1);
   
        ContactPoint2D[] contactPoints = new ContactPoint2D[10];

        other.GetContacts(contactPoints);

        foreach (var point in contactPoints)
        {
            if (!point.point.Equals(Vector2.zero))
            {
                var impactObject = Instantiate(onDestructiveHullContactPreSpawn, point.point, Quaternion.identity);
                impactObject.SetActive(true);
            }
        }
    }

}
