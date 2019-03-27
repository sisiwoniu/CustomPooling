using UnityEngine;

public class DebugPoolManager : MonoBehaviour {

    [SerializeField]
    private DebugPoolableObject Prefab = null;

    [SerializeField, Range(1, 2000)]
    private int CreateInstanceCount = 10;

    public void ClearPool() {
        Pool.Instance.ClearTargetPool(string.Empty, false);
    }

    public void CreatePoolInstance() {
        CreateInstance();
    }

    private void CreateInstance() {
        for(int i = 0; i < CreateInstanceCount; i++) {
            var obj = Instantiate(Prefab);

            obj.transform.localPosition = Vector3.zero;

            obj.Login();
        }
    }
}