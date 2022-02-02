using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class GameplayControllerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 8;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _deckSeed;
		public event FieldEvent<int> deckSeedChanged;
		public Interpolated<int> deckSeedInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int deckSeed
		{
			get { return _deckSeed; }
			set
			{
				// Don't do anything if the value is the same
				if (_deckSeed == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_deckSeed = value;
				hasDirtyFields = true;
			}
		}

		public void SetdeckSeedDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_deckSeed(ulong timestep)
		{
			if (deckSeedChanged != null) deckSeedChanged(_deckSeed, timestep);
			if (fieldAltered != null) fieldAltered("deckSeed", _deckSeed, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			deckSeedInterpolation.current = deckSeedInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _deckSeed);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_deckSeed = UnityObjectMapper.Instance.Map<int>(payload);
			deckSeedInterpolation.current = _deckSeed;
			deckSeedInterpolation.target = _deckSeed;
			RunChange_deckSeed(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _deckSeed);

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
				if (deckSeedInterpolation.Enabled)
				{
					deckSeedInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					deckSeedInterpolation.Timestep = timestep;
				}
				else
				{
					_deckSeed = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_deckSeed(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (deckSeedInterpolation.Enabled && !deckSeedInterpolation.current.UnityNear(deckSeedInterpolation.target, 0.0015f))
			{
				_deckSeed = (int)deckSeedInterpolation.Interpolate();
				//RunChange_deckSeed(deckSeedInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public GameplayControllerNetworkObject() : base() { Initialize(); }
		public GameplayControllerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public GameplayControllerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
