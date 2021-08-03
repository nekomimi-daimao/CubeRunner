using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Runtime
{
    public class ConnectToPhoton : MonoBehaviour
    {
        public string RoomName = "RoomName";

        private async UniTaskVoid Start()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            Connect(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnDestroy()
        {
            PhotonNetwork.Disconnect();
        }

        private const int LimitSecond = 30;

        private async UniTaskVoid Connect(CancellationToken destroyToken)
        {
            var cancelSource = CancellationTokenSource.CreateLinkedTokenSource(destroyToken);
            cancelSource.CancelAfterSlim(TimeSpan.FromSeconds(LimitSecond));
            var token = cancelSource.Token;

            PhotonNetwork.ConnectUsingSettings();

            await UniTask.WaitUntil(
                () => PhotonNetwork.NetworkingClient.State == ClientState.ConnectedToMasterServer,
                cancellationToken: token);

            PhotonNetwork.JoinOrCreateRoom(RoomName, new RoomOptions(), TypedLobby.Default);

            await UniTask.WaitUntil(
                () => PhotonNetwork.InRoom,
                cancellationToken: token);

            PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity);

            if (!cancelSource.IsCancellationRequested)
            {
                cancelSource.Cancel();
            }
        }
    }
}
