using Photon.Pun;
using UnityEngine;
using Share;

namespace Runtime
{
    [RequireComponent(typeof(PhotonView))]
    public class Player : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
    {
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            var anchorWithMemo = GetComponentInChildren<AnchorWithMemo>();
            if (anchorWithMemo != null)
            {
                anchorWithMemo.Memo = info.photonView.CreatorActorNr.ToString();
            }
        }

        public void OnPreNetDestroy(PhotonView rootView)
        {
            // NOP
        }
    }
}
