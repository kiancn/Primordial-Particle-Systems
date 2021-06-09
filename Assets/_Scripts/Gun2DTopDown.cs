using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun2DTopDown : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private Transform gunMuzzle;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletInstance;
    [Range(0f, 2f)] [SerializeField] private float bulletFireInterval = 0.2f;

    [SerializeField] private Player2DMoverTopdown playerMover;

    public static bool ShootingAllowed { get; set; } = true;

    private float timeSinceLastFire = 0f;

    private void Start()
    {
        if (playerMover == null)
        {
            playerMover = GetComponentInParent<Player2DMoverTopdown>();
            if (playerMover == null)
            {
                Debug.Log("No Player2DMoverTopdown found by Gun2DTopDown instance. Bad things might happen.");
            }
        }
    }

    private void OnEnable()
    {
        if (gunMuzzle != null)
        {
            bulletInstance = Instantiate(bullet);
            bulletInstance.SetActive(false);
        }
    }

    private void Update()
    {
        timeSinceLastFire += Time.deltaTime;

        // player can fire while game is not paused, after interval - and by pressing left mouse button.
        if (ShootingAllowed && Time.timeScale > 0f && timeSinceLastFire > bulletFireInterval && Input.GetMouseButtonDown(0))
        {
            if (playerStats.CurrentBullets > 1)
            {
                playerStats.BulletNumberChange(-1);
            }
            else
            {
                return;
            }

            // Debug.Log("Bam, gun was ordered to spawn bullet.");
            var newBullet = Instantiate(bulletInstance, gunMuzzle.position, gunMuzzle.rotation);

            newBullet.SetActive(true);

            // ensure that bullet speed is relative to player ship
            var bulletObj = newBullet.GetComponent<Bullet2DTopDown>();
            if (bulletObj != null)
            {
                bulletObj.Speed = bulletObj.Speed + playerMover.CurrentMoveSpeed;
            }

            timeSinceLastFire = 0f;
        }
    }
}