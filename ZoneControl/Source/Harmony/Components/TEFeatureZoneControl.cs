using System;
using UnityEngine;
using UnityEngine.Scripting;
using ZoneControl.Configuration;
using ZoneControl.Game.ZoneClaim;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Components;

[Preserve]
public class TEFeatureZoneControl : TEFeatureAbs
{
    public const int Version = 1;

    private const string BorderMaterialProperty = "BorderMaterial";

    private bool _showBounds;
    private Vector3i _poiMin;
    private Vector3i _poiMax;

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

    public bool UsePoiZone
    {
        get; private set;
    }

    private Material BorderMaterial
    {
        get
        {
            if (field == null && !string.IsNullOrEmpty(BorderMaterialPath))
            {
                field = DataLoader.LoadAsset<Material>(BorderMaterialPath);
            }
            return field;
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
            UsePoiZone = feature.UsePoiZone;
            _poiMin = feature._poiMin;
            _poiMax = feature._poiMax;
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
        _addCallback(new BlockActivationCommand("set_area", "frames", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
        _addCallback(new BlockActivationCommand("set_poi", "map_cursor", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
        _addCallback(new BlockActivationCommand("remove", "x", _enabled: false), TileEntityComposite.EBlockCommandOrder.Normal, FeatureData);
    }

    public override bool AllowBlockActivationCommand(ITileEntityFeature _module, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
    {
        //const string d_MethodName = "AllowBlockActivationCommand";

        if (!base.AllowBlockActivationCommand(_module, _commandName, _world, _blockPos, _blockValue, _entityFocusing))
        {
            return false;
        }

        bool isOwner = Parent.LocalPlayerIsOwner && IsPrimary();
        if (!Equals(_module))
        {
            return true;
        }

        //ModLogger.DebugLog($"{d_MethodName}: _commandName={_commandName.ToString()}");

        if (CommandIs(_commandName, "show_bounds"))
        {
            return isOwner && !ShowBounds;
        }

        if (CommandIs(_commandName, "hide_bounds"))
        {
            return isOwner && ShowBounds;
        }

        if (CommandIs(_commandName, "set_area"))
        {
            return isOwner;
        }

        if (CommandIs(_commandName, "set_poi"))
        {
            return isOwner && TryGetContainingPoi(out _, out _);
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
            RefreshBoundsHelper();
            return true;
        }

        if (CommandIs(_commandName, "set_area"))
        {
            UsePoiZone = false;
            SetModified();
            RegisterZone();
            RefreshBoundsHelper();
            return true;
        }

        if (CommandIs(_commandName, "set_poi"))
        {
            if (TryGetContainingPoi(out var min, out var max))
            {
                UsePoiZone = true;
                _poiMin = min;
                _poiMax = max;
                SetModified();
                RegisterZone();
                RefreshBoundsHelper();
                return true;
            }
            return false;
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
        RegisterZone();
        RefreshBoundsHelper();
    }

    public override void OnLoad()
    {
        base.OnLoad();
        RegisterZone();
        RefreshBoundsHelper();
    }

    public override void OnUnload(World _world)
    {
        base.OnUnload(_world);
        ZoneClaimBoundsHelper.RemoveBoundsHelper(ToWorldPos());
    }

    public override void OnRemove(World _world)
    {
        base.OnRemove(_world);
        ZoneClaimBoundsHelper.RemoveBoundsHelper(ToWorldPos());
        ZoneClaimRegistry.Unregister(ToWorldPos());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ZoneClaimBoundsHelper.RemoveBoundsHelper(ToWorldPos());
        ZoneClaimRegistry.Unregister(ToWorldPos());
    }

    public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
    {
        base.ReplacedBy(_bvOld, _bvNew, _teNew);
        ZoneClaimBoundsHelper.RemoveBoundsHelper(ToWorldPos());
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
        UsePoiZone = _br.ReadBoolean();
        _poiMin = new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
        _poiMax = new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
    }

    public override void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
    {
        base.Write(_bw, _eStreamMode);
        if (_eStreamMode == TileEntity.StreamModeWrite.Persistency)
        {
            _bw.Write((ushort)Version);
        }

        _bw.Write(_showBounds);
        _bw.Write(UsePoiZone);
        _bw.Write(_poiMin.x);
        _bw.Write(_poiMin.y);
        _bw.Write(_poiMin.z);
        _bw.Write(_poiMax.x);
        _bw.Write(_poiMax.y);
        _bw.Write(_poiMax.z);
    }

    private bool TryGetContainingPoi(out Vector3i min, out Vector3i max)
    {
        min = Vector3i.zero;
        max = Vector3i.zero;

        var world = GameManager.Instance?.World;
        if (world == null)
        {
            return false;
        }

        var poi = world.GetPOIAtPosition(ToWorldCenterPos());
        if (poi == null)
        {
            return false;
        }

        min = poi.boundingBoxPosition;
        max = poi.boundingBoxPosition + poi.boundingBoxSize;
        return true;
    }

    private void GetZoneBounds(out Vector3i min, out Vector3i max)
    {
        if (UsePoiZone)
        {
            min = _poiMin;
            max = _poiMax;
        }
        else
        {
            var blockPos = ToWorldPos();
            int half = ModConfig.ZoneControlSize() / 2;
            min = new Vector3i(blockPos.x - half, 0, blockPos.z - half);
            max = new Vector3i(blockPos.x + half, 0, blockPos.z + half);
        }
    }

    private void RegisterZone()
    {
        GetZoneBounds(out var min, out var max);
        ZoneClaimRegistry.Register(ToWorldPos(), min, max);
    }

    private void GetZoneFootprint(out Vector3 center, out Vector3 size)
    {
        if (UsePoiZone)
        {
            var min = _poiMin.ToVector3();
            var max = _poiMax.ToVector3();
            size = max - min;
            center = (min + max) * 0.5f;
        }
        else
        {
            float side = ModConfig.ZoneControlSize();
            size = new Vector3(side, side, side);
            center = ToWorldPos().ToVector3() + new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    private void RefreshBoundsHelper()
    {
        if (!Parent.LocalPlayerIsOwner)
        {
            return;
        }

        GetZoneFootprint(out var center, out var size);
        var helper = ZoneClaimBoundsHelper.GetBoundsHelper(ToWorldPos(), center, size, BorderMaterial);
        helper.gameObject.SetActive(ShowBounds);
    }
}
