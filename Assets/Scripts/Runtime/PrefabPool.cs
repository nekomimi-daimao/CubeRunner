using Photon.Pun;
using UnityEngine;

namespace Runtime
{
    public class PrefabPool : MonoBehaviour, IPunPrefabPool
    {
        [SerializeField]
        private Player PrefabPlayer;

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            PrefabPlayer.gameObject.SetActive(false);
            var go = GameObject.Instantiate(PrefabPlayer, position, rotation);
            go.transform.SetParent(this.transform);
            return go.gameObject;
        }

        public void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }

        private void OnEnable()
        {
            PhotonNetwork.PrefabPool = this;
        }

        private void OnDisable()
        {
            if (PhotonNetwork.PrefabPool is PrefabPool)
            {
                PhotonNetwork.PrefabPool = new DefaultPool();
            }
        }
    }
}
