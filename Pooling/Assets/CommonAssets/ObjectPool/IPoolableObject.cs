namespace GPoolingSystem {

    public interface IPoolableObject {
        void Login();

        void Logout();

        void Collect();

        void Use();

        //使用可能かどうかのフラグ
        bool Standby {
            get;
        }

        UnityEngine.GameObject SelfObj {
            get;
        }
    }

}
