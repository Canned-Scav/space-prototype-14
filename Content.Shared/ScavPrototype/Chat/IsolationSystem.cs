using System.Linq;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Utility;
using static Content.Shared.Interaction.SharedInteractionSystem;

namespace Content.Shared.ScavPrototype.Chat;

public sealed class IsolationSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly OccluderSystem _occluder = default!;

    public float CalculateIsolationLevel(MapCoordinates  origin, MapCoordinates  other)
    {
        if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
            return 0f;

        var dir = other.Position - origin.Position;
        var length = dir.Length();

        if (length <= 0.01f) return 0f;

        var ray = new Ray(origin.Position, dir.Normalized());
        List<RayCastResults> rayResults = new List<RayCastResults>();
        _occluder.IntersectRay(rayResults, origin.MapId, ray, length);

        if (rayResults.Count == 0) return 0f;

        var totalIsolation = length / 10f;
        foreach (var result in rayResults)
        {
            if (!TryComp(result.HitEntity, out IsolationComponent? isolationComp))
                continue;

            totalIsolation += isolationComp.Isolation;
        }

        return totalIsolation;
    }

    public float CalculateIsolationLevel(EntityUid origin, EntityUid other)
    {
        var originPos = _transform.GetMapCoordinates(origin);
        var otherPos = _transform.GetMapCoordinates(other);

        return CalculateIsolationLevel(originPos, otherPos);
    }
}
