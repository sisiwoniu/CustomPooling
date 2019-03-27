using UnityEngine;
using GPoolingSystem;

//プール利用可能のゲームオブジェクト
//継承して利用する
public class CommonPoolableObject : MonoBehaviour, IPoolableObject {

    [SerializeField]
    private string TargetPoolName = "defaultpool";

    public bool Standby { get; private set; } = true;

    public GameObject SelfObj {
        get {
            return gameObject;
        }
    }

    public void Login() {
        Pool.Instance.LoginToPool(this, TargetPoolName);
    }

    public void Logout() {
        Pool.Instance.LogoutFromPool(this, TargetPoolName);
    }

    //使用する直前呼び出す
    public void Use() {
        Standby = false;
    }

    //再利用可能の場合これを呼び出す
    public void Collect() {
        Standby = true;
    }

    private void OnDestroy() {
        Logout();
    }
}