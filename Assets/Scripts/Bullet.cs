using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   private void OnCollisionEnter(Collision objectWeHit){
        if (objectWeHit.gameObject.CompareTag("Target")){  // Vérifiez aussi l'orthographe de "Target"
            print("hit " + objectWeHit.gameObject.name + "!");
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.CompareTag("Wall")){  // Vérifiez aussi l'orthographe de "Target"
            print("hit Wall");

            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
    }
    void CreateBulletImpactEffect(Collision objectWeHit){
ContactPoint contact = objectWeHit.contacts[0];
 GameObject hole = Instantiate(
    GlobalReferences.Instance.bulletImpactEffectPrefab,
    contact.point,
    Quaternion.LookRotation(contact.normal)
);

hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}

