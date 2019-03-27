using UnityEngine;

public class DebugShooter : MonoBehaviour {

    [SerializeField]
    private DebugPoolableObject bulletPrefab = null;

    private void Update() {
        if(Input.GetKey(KeyCode.S)) {
            Shoot();
        }
    }

    private void Shoot() {
        DebugPoolableObject bullet;

        Pool.Instance.RequestUseObject(out bullet, bulletPrefab.SelfObj);

        bullet.Shoot(transform.position);
    }

}