using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0]")]
	public partial class PlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 8;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _playerIndex;
		public event FieldEvent<int> playerIndexChanged;
		public Interpolated<int> playerIndexInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int playerIndex
		{
			get { return _playerIndex; }
			set
			{
				// Don't do anything if the value is the same
				if (_playerIndex == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_playerIndex = value;
				hasDirtyFields = true;
			}
		}

		public void SetplayerIndexDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_playerIndex(ulong timestep)
		{
			if (playerIndexChanged != null) playerIndexChanged(_playerIndex, timestep);
			if (fieldAltered != null) fieldAltered("playerIndex", _playerIndex, timestep);
		}
		[ForgeGeneratedField]
		private int _playerMoney;
		public event FieldEvent<int> playerMoneyChanged;
		public Interpolated<int> playerMoneyInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int playerMoney
		{
			get { return _playerMoney; }
			set
			{
				// Don't do anything if the value is the same
				if (_playerMoney == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_playerMoney = value;
				hasDirtyFields = true;
			}
		}

		public void SetplayerMoneyDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_playerMoney(ulong timestep)
		{
			if (playerMoneyChanged != null) playerMoneyChanged(_playerMoney, timestep);
			if (fieldAltered != null) fieldAltered("playerMoney", _playerMoney, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			playerIndexInterpolation.current = playerIndexInterpolation.target;
			playerMoneyInterpolation.current = playerMoneyInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _playerIndex);
			UnityObjectMapper.Instance.MapBytes(data, _playerMoney);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_playerIndex = UnityObjectMapper.Instance.Map<int>(payload);
			playerIndexInterpolation.current = _playerIndex;
			playerIndexInterpolation.target = _playerIndex;
			RunChange_playerIndex(timestep);
			_playerMoney = UnityObjectMapper.Instance.Map<int>(payload);
			playerMoneyInterpolation.current = _playerMoney;
			playerMoneyInterpolation.target = _playerMoney;
			RunChange_playerMoney(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _playerIndex);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _playerMoney);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (playerIndexInterpolation.Enabled)
				{
					playerIndexInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					playerIndexInterpolation.Timestep = timestep;
				}
				else
				{
					_playerIndex = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_playerIndex(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (playerMoneyInterpolation.Enabled)
				{
					playerMoneyInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					playerMoneyInterpolation.Timestep = timestep;
				}
				else
				{
					_playerMoney = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_playerMoney(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (playerIndexInterpolation.Enabled && !playerIndexInterpolation.current.UnityNear(playerIndexInterpolation.target, 0.0015f))
			{
				_playerIndex = (int)playerIndexInterpolation.Interpolate();
				//RunChange_playerIndex(playerIndexInterpolation.Timestep);
			}
			if (playerMoneyInterpolation.Enabled && !playerMoneyInterpolation.current.UnityNear(playerMoneyInterpolation.target, 0.0015f))
			{
				_playerMoney = (int)playerMoneyInterpolation.Interpolate();
				//RunChange_playerMoney(playerMoneyInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerNetworkObject() : base() { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
