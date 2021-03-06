using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"int\", \"bool\"][][][\"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"playerIndex\", \"isHost\"][][][\"deckString\"]]")]
	public abstract partial class GameplayControllerBehavior : NetworkBehavior
	{
		public const byte RPC_ADD_NETWORK_PLAYER = 0 + 5;
		public const byte RPC_START_GAME = 1 + 5;
		public const byte RPC_RESTART_GAME = 2 + 5;
		public const byte RPC_UPDATE_DECK_STRING = 3 + 5;
		
		public GameplayControllerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (GameplayControllerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("AddNetworkPlayer", AddNetworkPlayer, typeof(int), typeof(bool));
			networkObject.RegisterRpc("StartGame", StartGame);
			networkObject.RegisterRpc("RestartGame", RestartGame);
			networkObject.RegisterRpc("UpdateDeckString", UpdateDeckString, typeof(string));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new GameplayControllerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new GameplayControllerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// int playerIndex
		/// bool isHost
		/// </summary>
		public abstract void AddNetworkPlayer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void StartGame(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void RestartGame(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void UpdateDeckString(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}