using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Transform FirePosition;
    // Update is called once per frame
    void Start(){
        InvokeRepeating("ShootBullet", 0f, 1f);
    }
    void ShootBullet()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePosition.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(transform.right * 1000);
    }
}
