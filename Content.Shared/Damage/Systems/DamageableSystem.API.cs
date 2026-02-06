using System.Linq;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

//SpacePrototype Changes
using System.Linq;
using Content.Shared.ScavPrototype.NewMedical.Damage;
using Content.Shared.ScavPrototype.NewMedical.Targeting;
using Content.Shared.ScavPrototype.NewMedical.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Robust.Shared.Random;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;

namespace Content.Shared.Damage.Systems;

public sealed partial class DamageableSystem
{
    /// <summary>
    ///     Directly sets the damage in a damageable component.
    ///     This method keeps the damage types supported by the DamageContainerPrototype in the component.
    ///     If a type is given in <paramref name="damage"/>, but not supported then it will not be set.
    ///     If a type is supported but not given in <paramref name="damage"/> then it will be set to 0.
    /// </summary>
    /// <remarks>
    ///     Useful for some unfriendly folk. Also ensures that cached values are updated and that a damage changed
    ///     event is raised.
    /// </remarks>
    public void SetDamage(Entity<DamageableComponent?> ent, DamageSpecifier damage)
    {
        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
            return;

        foreach (var type in ent.Comp.Damage.DamageDict.Keys)
        {
            if (damage.DamageDict.TryGetValue(type, out var value))
                ent.Comp.Damage.DamageDict[type] = value;
            else
                ent.Comp.Damage.DamageDict[type] = 0;
        }

        OnEntityDamageChanged((ent, ent.Comp));
    }

    /// <summary>
    ///     Directly sets the damage specifier of a damageable component.
    ///     This will overwrite the complete damage dict, meaning it will bulldoze the supported damage types.
    /// </summary>
    /// <remarks>
    ///     This may break persistance as the supported types are reset in case the component is initialized again.
    ///     So this only makes sense if you also change the DamageContainerPrototype in the component at the same time.
    ///     Only use this method if you know what you are doing.
    /// </remarks>
    public void SetDamageSpecifier(Entity<DamageableComponent?> ent, DamageSpecifier damage)
    {
        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
            return;

        ent.Comp.Damage = damage;

        OnEntityDamageChanged((ent, ent.Comp));
    }

    /// <summary>
    ///     Applies damage specified via a <see cref="DamageSpecifier"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="DamageSpecifier"/> is effectively just a dictionary of damage types and damage values. This
    ///     function just applies the container's resistances (unless otherwise specified) and then changes the
    ///     stored damage data. Division of group damage into types is managed by <see cref="DamageSpecifier"/>.
    /// </remarks>
    /// <returns>
    ///     If the attempt was successful or not.
    /// </returns>
    public bool TryChangeDamage(
        Entity<DamageableComponent?> ent,
        DamageSpecifier damage,
        bool ignoreResistances = false,
        bool interruptsDoAfters = true,
        EntityUid? origin = null,
        bool ignoreGlobalModifiers = false,
        TargetBodyPart targetPart = TargetBodyPart.All,
        SplitDamageBehavior splitDamage = SplitDamageBehavior.Split,
        bool canMiss = false
    )
    {
        //! Empty just checks if the DamageSpecifier is _literally_ empty, as in, is internal dictionary of damage types is empty.
        // If you deal 0.0 of some damage type, Empty will be false!
        return TryChangeDamage(ent, damage, out _, ignoreResistances, interruptsDoAfters, origin, ignoreGlobalModifiers, targetPart, splitDamage, canMiss);
    }

    /// <summary>
    ///     Applies damage specified via a <see cref="DamageSpecifier"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="DamageSpecifier"/> is effectively just a dictionary of damage types and damage values. This
    ///     function just applies the container's resistances (unless otherwise specified) and then changes the
    ///     stored damage data. Division of group damage into types is managed by <see cref="DamageSpecifier"/>.
    /// </remarks>
    /// <returns>
    ///     If the attempt was successful or not.
    /// </returns>
    public bool TryChangeDamage(
        Entity<DamageableComponent?> ent,
        DamageSpecifier damage,
        out DamageSpecifier newDamage,
        bool ignoreResistances = false,
        bool interruptsDoAfters = true,
        EntityUid? origin = null,
        bool ignoreGlobalModifiers = false,
        TargetBodyPart targetPart = TargetBodyPart.All,
        SplitDamageBehavior splitDamage = SplitDamageBehavior.Split,
        bool canMiss = false
    )
    {
        //! Empty just checks if the DamageSpecifier is _literally_ empty, as in, is internal dictionary of damage types is empty.
        // If you deal 0.0 of some damage type, Empty will be false!
        newDamage = ChangeDamage(ent, damage, ignoreResistances, interruptsDoAfters, origin, ignoreGlobalModifiers, targetPart, splitDamage, canMiss);
        return !newDamage.Empty;
    }

    /// <summary>
    ///     Applies damage specified via a <see cref="DamageSpecifier"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="DamageSpecifier"/> is effectively just a dictionary of damage types and damage values. This
    ///     function just applies the container's resistances (unless otherwise specified) and then changes the
    ///     stored damage data. Division of group damage into types is managed by <see cref="DamageSpecifier"/>.
    /// </remarks>
    /// <returns>
    ///     The actual amount of damage taken, as a DamageSpecifier.
    /// </returns>
    public DamageSpecifier ChangeDamage(
        Entity<DamageableComponent?> ent,
        DamageSpecifier damage,
        bool ignoreResistances = false,
        bool interruptsDoAfters = true,
        EntityUid? origin = null,
        bool ignoreGlobalModifiers = false,
        TargetBodyPart targetPart = TargetBodyPart.All,
        SplitDamageBehavior splitDamage = SplitDamageBehavior.Split,
        bool canMiss = false
    )
    {
        var damageDone = new DamageSpecifier();

        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
            return damageDone;

        if (damage.Empty)
            return damageDone;

        var before = new BeforeDamageChangedEvent(damage, origin);
        RaiseLocalEvent(ent, ref before);

        if (before.Cancelled)
            return damageDone;

        //SpacePrototype Changes start

        // For entities with a body, route damage through body parts and then sum it up
            if (_bodyQuery.TryGetComponent(ent, out var body) && targetPart != TargetBodyPart.None)
            {
                var appliedDamage = ApplyDamageToBodyParts(ent, damage, origin, ignoreResistances,
                    interruptsDoAfters, targetPart, splitDamage, canMiss);

                if (appliedDamage == null) return damageDone;

                return appliedDamage;
            }
        //SpacePrototype Changes end

        // Apply resistances
        if (!ignoreResistances)
        {
            if (
                ent.Comp.DamageModifierSetId != null &&
                _prototypeManager.Resolve(ent.Comp.DamageModifierSetId, out var modifierSet)
            )
                damage = DamageSpecifier.ApplyModifierSet(damage, modifierSet);

            // TODO DAMAGE
            // byref struct event.
            var ev = new DamageModifyEvent(damage, origin);
            RaiseLocalEvent(ent, ev);
            damage = ev.Damage;

            if (damage.Empty)
                return damageDone;
        }

        if (!ignoreGlobalModifiers)
            damage = ApplyUniversalAllModifiers(damage);


        damageDone.DamageDict.EnsureCapacity(damage.DamageDict.Count);

        var dict = ent.Comp.Damage.DamageDict;
        foreach (var (type, value) in damage.DamageDict)
        {
            // CollectionsMarshal my beloved.
            if (!dict.TryGetValue(type, out var oldValue))
                continue;

            var newValue = FixedPoint2.Max(FixedPoint2.Zero, oldValue + value);
            if (newValue == oldValue)
                continue;

            dict[type] = newValue;
            damageDone.DamageDict[type] = newValue - oldValue;
        }

        if (!damageDone.Empty)
            OnEntityDamageChanged((ent, ent.Comp), damageDone, interruptsDoAfters, origin);

        return damageDone;
    }

    /// <summary>
    /// Will reduce the damage on the entity exactly by <see cref="amount"/> as close as equally distributed among all damage types the entity has.
    /// If one of the damage types of the entity is too low. it will heal that completly and distribute the excess healing among the other damage types.
    /// If the <see cref="amount"/> is larger than the total damage of the entity then it just clears all damage.
    /// </summary>
    /// <param name="ent">entity to be healed</param>
    /// <param name="amount">how much to heal. value has to be negative to heal</param>
    /// <param name="group">from which group to heal. if null, heal from all groups</param>
    /// <param name="origin">who did the healing</param>
    public DamageSpecifier HealEvenly(
        Entity<DamageableComponent?> ent,
        FixedPoint2 amount,
        ProtoId<DamageGroupPrototype>? group = null,
        EntityUid? origin = null)
    {
        var damageChange = new DamageSpecifier();

        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false) || amount >= 0)
            return damageChange;

        // Get our total damage, or heal if we're below a certain amount.
        if (!TryGetDamageGreaterThan((ent, ent.Comp), -amount, out var damage, group))
            return ChangeDamage(ent, -damage, true, false, origin);

        // make sure damageChange has the same damage types as damage
        damageChange.DamageDict.EnsureCapacity(damage.DamageDict.Count);
        foreach (var type in damage.DamageDict.Keys)
        {
            damageChange.DamageDict.Add(type, FixedPoint2.Zero);
        }

        var remaining = -amount;
        var keys = damage.DamageDict.Keys.ToList();

        while (remaining > 0)
        {
            var count = keys.Count;
            // We do this to ensure that we always round up when dividing to avoid excess loops.
            // We already have logic to prevent healing more than we have.
            var maxHeal = count == 1 ? remaining : (remaining + FixedPoint2.Epsilon * (count - 1)) / count;

            // Iterate backwards since we're removing items.
            for (var i = count - 1; i >= 0; i--)
            {
                var type = keys[i];
                // This is the amount we're trying to heal, capped by maxHeal
                var heal = damage.DamageDict[type] + damageChange.DamageDict[type];

                // Don't go above max, if we don't go above max
                if (heal > maxHeal)
                    heal = maxHeal;
                // If we're not above max, we will heal it fully and don't need to enumerate anymore!
                else
                    keys.RemoveAt(i);

                if (heal >= remaining)
                {
                    // Don't remove more than we can remove. Prevents us from healing more than we'd expect...
                    damageChange.DamageDict[type] -= remaining;
                    remaining = FixedPoint2.Zero;
                    break;
                }

                remaining -= heal;
                damageChange.DamageDict[type] -= heal;
            }
        }

        return ChangeDamage(ent, damageChange, true, false, origin);
    }

    /// <summary>
    /// Will reduce the damage on the entity exactly by <see cref="amount"/> distributed by weight among all damage types the entity has.
    /// (the weight is how much damage of the type there is)
    /// If the <see cref="amount"/> is larger than the total damage of the entity then it just clears all damage.
    /// </summary>
    /// <param name="ent">entity to be healed</param>
    /// <param name="amount">how much to heal. value has to be negative to heal</param>
    /// <param name="group">from which group to heal. if null, heal from all groups</param>
    /// <param name="origin">who did the healing</param>
    public DamageSpecifier HealDistributed(
        Entity<DamageableComponent?> ent,
        FixedPoint2 amount,
        ProtoId<DamageGroupPrototype>? group = null,
        EntityUid? origin = null)
    {
        var damageChange = new DamageSpecifier();

        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false) || amount >= 0)
            return damageChange;

        // Get our total damage, or heal if we're below a certain amount.
        if (!TryGetDamageGreaterThan((ent, ent.Comp), -amount, out var damage, group))
            return ChangeDamage(ent, -damage, true, false, origin);

        // make sure damageChange has the same damage types as damageEntity
        damageChange.DamageDict.EnsureCapacity(damage.DamageDict.Count);
        var total = damage.GetTotal();

        // heal weighted by the damage of that type
        foreach (var (type, value) in damage.DamageDict)
        {
            damageChange.DamageDict.Add(type, value / total * amount);
        }

        return ChangeDamage(ent, damageChange, true, false, origin);
    }

    /// <summary>
    /// Tries to get damage from an entity with an optional group specifier.
    /// </summary>
    /// <param name="ent">Entity we're checking the damage on</param>
    /// <param name="amount">Amount we want the damage to be greater than ideally</param>
    /// <param name="damage">Damage specifier we're returning with</param>
    /// <param name="group">An optional group, note that if it fails to index it will just use all damage.</param>
    /// <returns>True if the total damage is greater than the specified amount</returns>
    public bool TryGetDamageGreaterThan(Entity<DamageableComponent> ent,
        FixedPoint2 amount,
        out DamageSpecifier damage,
        ProtoId<DamageGroupPrototype>? group = null)
    {
        // get the damage should be healed (either all or only from one group)
        damage = group == null ? GetDamage(ent) : GetDamage(ent, group.Value);

        // If trying to heal more than the total damage of damageEntity just heal everything
        return damage.GetTotal() > amount;
    }

    /// <summary>
    /// Returns a <see cref="DamageSpecifier"/> with all positive damage of the entity from the group specified
    /// </summary>
    /// <param name="ent">entity with damage</param>
    /// <param name="group">group of damage to get values from</param>
    /// <returns></returns>
    public DamageSpecifier GetDamage(Entity<DamageableComponent> ent, ProtoId<DamageGroupPrototype> group)
    {
        // No damage if no group exists...
        if (!_prototypeManager.Resolve(group, out var groupProto))
            return new DamageSpecifier();

        var damage = new DamageSpecifier();
        damage.DamageDict.EnsureCapacity(groupProto.DamageTypes.Count);

        foreach (var damageId in groupProto.DamageTypes)
        {
            if (!ent.Comp.Damage.DamageDict.TryGetValue(damageId, out var value))
                continue;
            if (value > FixedPoint2.Zero)
                damage.DamageDict.Add(damageId, value);
        }

        return damage;
    }

    /// <summary>
    /// Returns a <see cref="DamageSpecifier"/> with all positive damage of the entity
    /// </summary>
    /// <param name="ent">entity with damage</param>
    /// <returns></returns>
    public DamageSpecifier GetDamage(Entity<DamageableComponent> ent)
    {
        var damage = new DamageSpecifier();
        damage.DamageDict.EnsureCapacity(ent.Comp.Damage.DamageDict.Count);

        foreach (var (damageId, value) in ent.Comp.Damage.DamageDict)
        {
            if (value > FixedPoint2.Zero)
                damage.DamageDict.Add(damageId, value);
        }

        return damage;
    }

    /// <summary>
    /// Applies the two universal "All" modifiers, if set.
    /// Individual damage source modifiers are set in their respective code.
    /// </summary>
    /// <param name="damage">The damage to be changed.</param>
    public DamageSpecifier ApplyUniversalAllModifiers(DamageSpecifier damage)
    {
        // Checks for changes first since they're unlikely in normal play.
        if (
            MathHelper.CloseToPercent(UniversalAllDamageModifier, 1f) &&
            MathHelper.CloseToPercent(UniversalAllHealModifier, 1f)
        )
            return damage;

        foreach (var (key, value) in damage.DamageDict)
        {
            if (value == 0)
                continue;

            if (value > 0)
            {
                damage.DamageDict[key] *= UniversalAllDamageModifier;

                continue;
            }

            if (value < 0)
                damage.DamageDict[key] *= UniversalAllHealModifier;
        }

        return damage;
    }

    public void ClearAllDamage(Entity<DamageableComponent?> ent)
    {
        SetAllDamage(ent, FixedPoint2.Zero);
    }

    /// <summary>
    ///     Sets all damage types supported by a <see cref="Components.DamageableComponent"/> to the specified value.
    /// </summary>
    /// <remarks>
    ///     Does nothing If the given damage value is negative.
    /// </remarks>
    public void SetAllDamage(Entity<DamageableComponent?> ent, FixedPoint2 newValue)
    {
        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
            return;

        if (newValue < 0)
            return;

        foreach (var type in ent.Comp.Damage.DamageDict.Keys)
        {
            ent.Comp.Damage.DamageDict[type] = newValue;
        }

        // Setting damage does not count as 'dealing' damage, even if it is set to a larger value, so we pass an
        // empty damage delta.
        OnEntityDamageChanged((ent, ent.Comp), new DamageSpecifier());
    }

    /// <summary>
    /// Set's the damage modifier set prototype for this entity.
    /// </summary>
    /// <param name="ent">The entity we're setting the modifier set of.</param>
    /// <param name="damageModifierSetId">The prototype we're setting.</param>
    public void SetDamageModifierSetId(Entity<DamageableComponent?> ent, ProtoId<DamageModifierSetPrototype>? damageModifierSetId)
    {
        if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
            return;

        ent.Comp.DamageModifierSetId = damageModifierSetId;

        Dirty(ent);
    }

    //SpacePrototype Changes

    private DamageSpecifier? ApplyDamageToBodyParts(
            EntityUid uid,
            DamageSpecifier damage,
            EntityUid? origin,
            bool ignoreResistances,
            bool interruptsDoAfters,
            TargetBodyPart? targetPart,
            SplitDamageBehavior splitDamageBehavior = SplitDamageBehavior.Split,
            bool canMiss = false,
            float partMultiplier = 1.00f)
        {
            DamageSpecifier? totalAppliedDamage = null;
            var adjustedDamage = damage * partMultiplier;
            // This cursed shitcode lets us know if the target part is a power of 2
            // therefore having multiple parts targeted.
            if (targetPart != null
                && targetPart != 0 && (targetPart & (targetPart - 1)) != 0)
            {
                // Extract only the body parts that are targeted in the bitmask
                var targetedBodyParts = new List<(EntityUid Id,
                    BodyPartComponent Component,
                    DamageableComponent Damageable)>();

                // Get only the primitive flags (powers of 2) - these are the actual individual body parts
                var primitiveFlags = Enum.GetValues<TargetBodyPart>()
                    .Where(flag => flag != 0 && (flag & (flag - 1)) == 0) // Power of 2 check
                    .ToList();

                foreach (var flag in primitiveFlags)
                {
                    // Check if this specific flag is set in our targetPart bitmask
                    if (targetPart.Value.HasFlag(flag))
                    {
                        var query = _body.ConvertTargetBodyPart(flag);
                        var parts = _body.GetBodyChildrenOfTypeWithComponent<DamageableComponent>(uid, query.Type,
                            symmetry: query.Symmetry).ToList();

                        if (parts.Count > 0)
                            targetedBodyParts.AddRange(parts);
                    }
                }

                // If we couldn't find any of the targeted parts, fall back to all body parts
                if (targetedBodyParts.Count == 0)
                {
                    var query = _body.GetBodyChildrenWithComponent<DamageableComponent>(uid).ToList();
                    if (query.Count > 0)
                        targetedBodyParts = query;
                    else
                        return null;
                }

                var damagePerPart = ApplySplitDamageBehaviors(splitDamageBehavior, adjustedDamage, targetedBodyParts);
                var appliedDamage = new DamageSpecifier();
                var surplusHealing = new DamageSpecifier();
                foreach (var (partId, partComp, partDamageable) in targetedBodyParts)
                {
                    var modifiedDamage = damagePerPart + surplusHealing;

                    // Apply damage to this part
                    var partDamageResult = ChangeDamageOnBodyPart((partId, partDamageable), modifiedDamage, origin, ignoreResistances, interruptsDoAfters, partComp, uid);

                    if (partDamageResult != null && !partDamageResult.Empty)
                    {
                        appliedDamage += partDamageResult;

                        /*
                            Why this ugly shitcode? Its so that we can track chems and other sorts of healing surpluses.
                            Assume you're fighting in a spaced area. Your chest has 30 damage, and every other part
                            is getting 0.5 per tick. Your chems will only be 1/11th as effective, so we take the surplus
                            healing and pass it along parts. That way a chem that would heal you for 75 brute would truly
                            heal the 75 brute per tick, and not some weird shit like 6.8 per tick.
                        */
                        foreach (var (type, damageFromDict) in modifiedDamage.DamageDict)
                        {
                            if (damageFromDict >= 0
                                || !partDamageResult.DamageDict.TryGetValue(type, out var damageFromResult)
                                || damageFromResult > 0)
                                continue;

                            // If the damage from the dict plus the surplus healing is equal to the damage from the result,
                            // we can safely set the surplus healing to 0, as that means we consumed all of it.
                            if (damageFromDict >= damageFromResult)
                            {
                                surplusHealing.DamageDict[type] = FixedPoint2.Zero;
                            }
                            else
                            {
                                if (surplusHealing.DamageDict.TryGetValue(type, out var _))
                                    surplusHealing.DamageDict[type] = damageFromDict - damageFromResult;
                                else
                                    surplusHealing.DamageDict.TryAdd(type, damageFromDict - damageFromResult);
                            }
                        }
                    }
                }

                totalAppliedDamage = appliedDamage;
            }
            else
            {
                // Target a specific body part
                TargetBodyPart? target;
                var totalDamage = damage.GetTotal();

                if (totalDamage <= 0 || !canMiss) // Whoops i think i fucked up damage here.
                    target = _body.GetTargetBodyPart(uid, origin, targetPart);
                else
                    target = _body.GetRandomBodyPart(uid, origin, targetPart);

                var (partType, symmetry) = _body.ConvertTargetBodyPart(target);
                var possibleTargets = _body.GetBodyChildrenOfType(uid, partType, symmetry: symmetry).ToList();

                if (possibleTargets.Count == 0)
                {
                    if (totalDamage <= 0)
                        return null;

                    possibleTargets = _body.GetBodyChildren(uid).ToList();
                }

                // No body parts at all?
                if (possibleTargets.Count == 0)
                    return null;

                var chosenTarget = _random.PickAndTake(possibleTargets);

                if (!_damageableQuery.TryComp(chosenTarget.Id, out var partDamageable))
                    return null;

                totalAppliedDamage = ChangeDamageOnBodyPart((chosenTarget.Id, partDamageable), adjustedDamage, origin, ignoreResistances, interruptsDoAfters, Comp<BodyPartComponent>(chosenTarget.Id), uid);
            }

            return totalAppliedDamage;
        }

    public DamageSpecifier ApplySplitDamageBehaviors(SplitDamageBehavior splitDamageBehavior,
            DamageSpecifier damage,
            List<(EntityUid Id, BodyPartComponent Component, DamageableComponent Damageable)> parts)
        {
            var newDamage = new DamageSpecifier(damage);
            switch (splitDamageBehavior)
            {
                case SplitDamageBehavior.None:
                    return newDamage;
                case SplitDamageBehavior.Split:
                    return newDamage / parts.Count;
                case SplitDamageBehavior.SplitEnsureAllDamaged:
                    var damagedParts = parts.Where(part =>
                        part.Damageable.TotalDamage > FixedPoint2.Zero).ToList();

                    parts.Clear();
                    parts.AddRange(damagedParts);

                    goto case SplitDamageBehavior.SplitEnsureAll;
                case SplitDamageBehavior.SplitEnsureAllOrganic:
                    var organicParts = parts.Where(part =>
                        part.Component.PartComposition == BodyPartComposition.Organic).ToList();

                    parts.Clear();
                    parts.AddRange(organicParts);

                    goto case SplitDamageBehavior.SplitEnsureAll;
                case SplitDamageBehavior.SplitEnsureAllDamagedAndOrganic:
                    var compatableParts = parts.Where(part =>
                        part.Damageable.TotalDamage > FixedPoint2.Zero &&
                        part.Component.PartComposition == BodyPartComposition.Organic).ToList();

                    parts.Clear();
                    parts.AddRange(compatableParts);
                    goto case SplitDamageBehavior.SplitEnsureAll;
                case SplitDamageBehavior.SplitEnsureAll:
                    foreach (var (type, val) in newDamage.DamageDict)
                    {
                        if (val > 0)
                        {
                            if (parts.Count > 0)
                                newDamage.DamageDict[type] = val / parts.Count;
                            else
                                newDamage.DamageDict[type] = FixedPoint2.Zero;
                        }
                        else if (val < 0)
                        {
                            var count = 0;

                            foreach (var (id, _, damageable) in parts)
                                if (damageable.Damage.DamageDict.TryGetValue(type, out var currentDamage)
                                    && currentDamage > 0)
                                    count++;

                            if (count > 0)
                                newDamage.DamageDict[type] = val / count;
                            else
                                newDamage.DamageDict[type] = FixedPoint2.Zero;
                        }
                    }
                    // We sort the parts to ensure that surplus damage gets passed from least to most damaged.
                    parts.Sort((a, b) => a.Damageable.TotalDamage.CompareTo(b.Damageable.TotalDamage));
                    return newDamage;
                default:
                    return damage;
            }
        }

    private DamageSpecifier? ChangeDamageOnBodyPart(
            Entity<DamageableComponent?> ent,
            DamageSpecifier damage,
            EntityUid? origin,
            bool ignoreResistances,
            bool interruptsDoAfters,
            BodyPartComponent partComp,
            EntityUid bodyUid)
        {
            var appliedDamage = new DamageSpecifier();

            if (!_damageableQuery.Resolve(ent, ref ent.Comp, false))
                return appliedDamage;

            var partMaxDamage = _woundable.GetMaxDamage(ent.Owner);

            if(ent.Comp.Damage.GetTotal() >= partMaxDamage)
                damage /= 20; //Потом изменить

            appliedDamage = ChangeDamage(ent, damage, ignoreResistances, interruptsDoAfters, origin);

            ChangeDamage(bodyUid, appliedDamage, true, interruptsDoAfters, origin, targetPart: TargetBodyPart.None);

            return appliedDamage;
        }

}
