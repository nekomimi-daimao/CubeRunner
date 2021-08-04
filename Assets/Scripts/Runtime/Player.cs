using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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
            var actorNr = info.photonView.CreatorActorNr;

            Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);

            if (cubeRenderer != null)
            {
                var color = colors[actorNr % colors.Length];
                cubeRenderer.material.color = color;
            }

            var anchorWithMemo = GetComponentInChildren<AnchorWithMemo>();
            if (anchorWithMemo != null)
            {
                anchorWithMemo.Memo = actorNr.ToString();
            }

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNr)
            {
                RunnerLoop(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        public void OnPreNetDestroy(PhotonView rootView)
        {
            // NOP
        }

        private readonly Color[] colors = {Color.blue, Color.cyan, Color.green, Color.red, Color.magenta,};

        [SerializeField]
        private Renderer cubeRenderer;

        private const float SpeedMin = 4f;
        private const float SpeedMax = 10f;
        public float Speed = SpeedMin;

        private const float ChangeDistance = 0.2f * 0.2f;

        private const float RotateMin = 30f;
        private const float RotateMax = 100f;

        private async UniTaskVoid RunnerLoop(CancellationToken token)
        {
            var relayRoot = GameObject.FindWithTag("Respawn")?.transform;
            if (relayRoot == null)
            {
                return;
            }

            var relayPoints = Enumerable.Range(0, relayRoot.childCount)
                .Select(i => relayRoot.GetChild(i).position)
                .Select(vector3 =>
                {
                    vector3.y = 0.5f;
                    return vector3;
                })
                .ToArray();

            var index = 0;
            var rotate = RotateMin;

            var ts = this.transform;

            while (true)
            {
                await UniTask.Yield();

                if (token.IsCancellationRequested)
                {
                    break;
                }

                var relayPoint = relayPoints[index];
                var moveTowards = Vector3.MoveTowards(
                    ts.position,
                    relayPoint,
                    Time.deltaTime * Speed);

                ts.position = moveTowards;
                ts.Rotate(Vector3.up, Time.deltaTime * rotate);

                if (Vector3.SqrMagnitude(relayPoint - moveTowards) <= ChangeDistance)
                {
                    index++;
                    if (relayPoints.Length <= index)
                    {
                        index = 0;
                    }

                    rotate = UnityEngine.Random.Range(RotateMin, RotateMax) * ((UnityEngine.Random.value > 0.5f) ? 1 : -1);
                }
            }
        }
    }
}
