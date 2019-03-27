using System.Collections;
using UnityEngine;

public class DebugPoolableObject : CommonPoolableObject {

    private Rigidbody body;

    public void Shoot(Vector3 Pos) {
        //使用中のフラグをオン
        Use();

        transform.position = Pos;

        gameObject.SetActive(true);

        if(body == null) {
            body = GetComponent<Rigidbody>();
        }

        body.AddForce(Vector3.up * 100f);

        StartCoroutine("AutoDisable");
    }

    private void OnDisable() {
        if(body != null)
            body.velocity = Vector3.zero;

        //再利用するために回収する
        Collect();
    }

    IEnumerator AutoDisable() {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}