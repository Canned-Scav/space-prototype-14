using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Item;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Robust.Shared.Containers;

namespace Robust.Shared.Console.Commands;

[AdminCommand(AdminFlags.Debug)]
internal sealed class TpAreaItems : LocalizedEntityCommands
{
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override string Command => "tpareaitems";
    public override bool RequireServerOrSingleplayer => true;

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { AttachedEntity: { } entity })
            return;

        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific", ("properAmount", 2), ("currentAmount", args.Length)));
            return;
        }

        MapId mapId;
        if (int.TryParse(args[0], out var intMapId))
            mapId = new MapId(intMapId);
        else
            return;

        if (!_mapSystem.MapExists(mapId) || !_mapSystem.TryGetMap(mapId, out var mapEnt) || mapEnt == null)
        {
            shell.WriteError($"Map {mapId} doesn't exist!");
            return;
        }

        if (!int.TryParse(args[1], out var range))
        {
            shell.WriteError($"Second argument should be a number!");
            return;
        }

        int counter = 0;
        foreach (var item in _lookup.GetEntitiesInRange(entity, range))
        {
            if (!_entities.TryGetComponent(item, out ItemComponent? _) && !_entities.TryGetComponent(item, out StorageComponent? _) && !_entities.TryGetComponent(item, out EntityStorageComponent? _))
                continue;

            if(_container.IsEntityInContainer(item))
                continue;

            var itemTransform = _entities.GetComponent<TransformComponent>(item);

            _transform.SetParent(item, itemTransform, mapEnt.Value);
            _transform.AttachToGridOrMap(item, itemTransform);
            counter++;
        }


        shell.WriteLine($"Teleported {counter} items to map {mapId}.");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHint("<MapId>"),
            2 => CompletionResult.FromHint("<range>"),
            _ => CompletionResult.Empty
        };
    }
}
