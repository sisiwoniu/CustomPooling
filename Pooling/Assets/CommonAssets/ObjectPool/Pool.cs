using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GPoolingSystem;

//プール本体、基本シングルトーンとして使う
//基本プールで扱えるのは「IPoolableObject」だけ
sealed public class Pool : MonoBehaviour {

    private const string DefaultPoolName = "defaultpool";

    private Dictionary<string, List<IPoolableObject>> poolDict = new Dictionary<string, List<IPoolableObject>>();

    private static Pool instance;

    public static Pool Instance {
        get {
            return instance ?? (instance = FindObjectOfType<Pool>());
        }
    }

    //NOTE:要素数が増えるとパフォーマンスに影響が出るかもしれないので、できる限り初期化の時点で済ませておくべきか
    //一回でログインする数を小さくするか
    public void LoginToPool(IPoolableObject AddObj, string PoolName = "") {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        if(poolDict.ContainsKey(poolName)) {
            if(!poolDict[poolName].Contains(AddObj))
                poolDict[poolName].Add(AddObj);

            Debug.Log($"{poolName}更新後の要素数 == " + poolDict[poolName].Count);
        }
    }

    //NOTE:要素数が増えるとパフォーマンスに影響が出るかもしれないので、できる限り初期化ロード時点で済ませておくべきか
    //一回で除外する数を小さくするか
    public void LogoutFromPool(IPoolableObject RemoveObj, string PoolName = "") {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        if(poolDict.ContainsKey(poolName)) {
            if(poolDict[poolName].Contains(RemoveObj))
                poolDict[poolName].Remove(RemoveObj);

            Debug.Log($"{poolName}の残り要素数 == " + poolDict[poolName].Count);
        }
    }

    //オブジェクトを取得するリクエスト
    //使えるものがなければnullを返す
    //プール名がなければデフォルトのプールを利用
    public void RequestUseObject<T>(out T Result, string PoolName = "") where T : Object {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        T item = null;

        if(poolDict.ContainsKey(poolName)) {
            item = poolDict[poolName].Where(obj => obj is T && obj.Standby).Select(obj => obj).FirstOrDefault() as T;
        }

        Result = item;
    }

    //オブジェクトを取得するリクエスト
    //使えるものがなければ「CreateObject」を新規作成して、プールに入れておく、nullを返さない
    //プール名がなければデフォルトのプールを利用
    public void RequestUseObject<T>(out T Result, GameObject CreateObject, string PoolName = "") where T : Object {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        T item = null;

        if(poolDict.ContainsKey(poolName)) {
            item = poolDict[poolName].Where(obj => obj is T && obj.Standby).Select(obj => obj).FirstOrDefault() as T;
        }

        //使えるものがなければ、作成しておく
        if(item == null) {
            item = Instantiate(CreateObject).GetComponent<T>();

            var poolableObj = item as IPoolableObject;

            poolableObj.Login();
        }

        Result = item;
    }

    //新規にプール種類を追加する
    public void AddNewPool(string PoolName) {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        if(!poolDict.ContainsKey(poolName)) {
            poolDict.Add(poolName, new List<IPoolableObject>());
        }
    }

    //NOTE:[DontDeleteObj]がtrueの場合、要素全員がプールから監視はずされるが、削除されないまま残る
    //falseの場合、監視を外し、削除される、GCが走る
    public void ClearTargetPool(string PoolName, bool RemoveList, bool DontDeleteObj = false) {
        string poolName = string.IsNullOrEmpty(PoolName) ? DefaultPoolName : PoolName;

        if(poolDict.ContainsKey(poolName)) {
            if(DontDeleteObj) {
                poolDict[poolName].Clear();
            } else {
                foreach(var poolObj in poolDict[poolName]) {
                    if(poolObj.SelfObj != null) {
                        Destroy(poolObj.SelfObj);
                    }
                }

                poolDict[poolName].Clear();

                Debug.Log($"{poolName}.count == " + poolDict[poolName].Count);
            }

            //プールリストそのものを除外
            if(RemoveList) {
                poolDict.Remove(poolName);
            }
        }
    }

    private void Awake() {
        //自動削除しない
        DontDestroyOnLoad(gameObject);

        AddNewPool(DefaultPoolName);
    }

    private void OnDestroy() {
        foreach(var key in poolDict.Keys) {
            poolDict[key].Clear();
        }

        poolDict.Clear();
    }
}