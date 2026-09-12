using System;
using UnityEngine.Scripting;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.ZoneClaim;

[Preserve]
public class TEFeatureZoneControl : TEFeatureAbs
{
    public const int Version = 1;

    private const string BorderMaterialProperty = "BorderMaterial";
    private bool _showBounds;

    public string BorderMaterialPath
    {
        get; private set;
    }

    public bool ShowBounds
    {
        get => _showBounds;
        set
        {
            _showBounds = value;
            SetModified();
        }
    }

    public override void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
    {
        base.Init(_parent, _featureData);
        BorderMaterialPath = _featureData.Props.GetString(BorderMaterialProperty);
        ModLogger.DebugLog($"Zone claim border material: {BorderMaterialPath}");
    }

    public override void CopyFromInternal(TileEntityComposite _other)
    {
        if (_other.TryGetSelfOrFeature<TEFeatureZoneControl>(out var feature))
        {
            _showBounds = feature._showBounds;
        }
    }

    public override string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
    {
        base.GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
        return string.Format(Localization.Get("activeBlockPrompt"), _blockValue.Block.GetLocalizedBlockName());
    }

    public override void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
    {
        base.InitBlockActivationCommands(_addCallback);
        _addCallback(new BlockActivationCommand("show_bounds", "frames", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
        _addCallback(new BlockActivationCommand("hide_bounds", "frames", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
        _addCallback(new BlockActivationCommand("remove", "x", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
    }

    public override bool AllowBlockActivationCommand(ITileEntityFeature _module, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
    {
        if (!base.AllowBlockActivationCommand(_module, _commandName, _world, _blockPos, _blockValue, _entityFocusing))
        {
            return false;
        }

        bool isOwner = Parent.LocalPlayerIsOwner && IsPrimary();
        if (!Equals(_module))
        {
            return true;
        }

        if (CommandIs(_commandName, "show_bounds"))
        {
            return isOwner && !ShowBounds;
        }

        if (CommandIs(_commandName, "hide_bounds"))
        {
            return isOwner && ShowBounds;
        }

        CommandIs(_commandName, "remove");
        return isOwner;
    }

    public override bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
    {
        base.OnBlockActivated(_commandName, _world, _blockPos, _blockValue, _player);

        if (CommandIs(_commandName, "show_bounds") || CommandIs(_commandName, "hide_bounds"))
        {
            ShowBounds = !ShowBounds;
            return true;
        }

        if (CommandIs(_commandName, "remove"))
        {
            _world.SetBlockRPC(_blockPos, BlockValue.Air);
            return true;
        }

        return false;
    }

    public override void OnAdded(Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnAdded(_blockPos, _blockValue);
        ZoneClaimRegistry.Register(ToWorldPos());
    }

    public override void OnLoad()
    {
        base.OnLoad();
        ZoneClaimRegistry.Register(ToWorldPos());
    }

    public override void OnRemove(World _world)
    {
        base.OnRemove(_world);
        ZoneClaimRegistry.Unregister(ToWorldPos());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ZoneClaimRegistry.Unregister(ToWorldPos());
    }

    public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
    {
        base.ReplacedBy(_bvOld, _bvNew, _teNew);
        ZoneClaimRegistry.Unregister(ToWorldPos());
    }

    public bool IsPrimary()
    {
        return ZoneClaimRegistry.Contains(ToWorldPos());
    }

    public override void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
    {
        base.Read(_br, _eStreamMode);
        if (_eStreamMode == TileEntity.StreamModeRead.Persistency)
        {
            if (Parent.UseLocalVersioning())
            {
                _br.ReadUInt16();
            }
            else
            {
                Parent.GetLegacyForkVersion();
            }
        }

        _showBounds = _br.ReadBoolean();
    }

    public override void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
    {
        base.Write(_bw, _eStreamMode);
        if (_eStreamMode == TileEntity.StreamModeWrite.Persistency)
        {
            _bw.Write((ushort)Version);
        }

        _bw.Write(_showBounds);
    }
}
