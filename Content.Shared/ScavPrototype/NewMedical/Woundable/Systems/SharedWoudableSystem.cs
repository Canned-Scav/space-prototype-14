using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;
using Content.Shared.ScavPrototype.NewMedical.Targeting;
using Content.Shared.Damage.Systems;
using Content.Shared.Body.Part;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;

namespace Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;
public abstract class SharedWoundableSystem : EntitySystem
{
    [Dependency] private readonly SharedBodySystem _body = default!;

    public override void Initialize()
    {
        //SubscribeLocalEvent<WoundableComponent, ComponentInit>(WoundableInit);
        SubscribeLocalEvent<WoundablePartComponent, DamageChangedEvent>(ChangeIntegrity);
        SubscribeLocalEvent<WoundableComponent, BodyPartsInitializedEvent>(OnBodyPartsInit);
    }

    private void OnBodyPartsInit(Entity<WoundableComponent> ent, ref BodyPartsInitializedEvent args)
    {
        if (!TryComp<BodyComponent>(ent.Owner, out var body) || body.RootContainer == null)
            return;

        var _partsWoundable = new Dictionary<TargetBodyPart, EntityUid>();

        foreach (var (partUid, partComp) in _body.GetBodyChildren(ent.Owner, body))
        {
            var targetPart = _body.GetTargetBodyPart(partComp.PartType, partComp.Symmetry);

            _partsWoundable.Add(targetPart, partUid);
        }

        ent.Comp.PartsWoundable = _partsWoundable;
    }

    public void ChangeIntegrity(Entity<WoundablePartComponent> ent, ref DamageChangedEvent args)
    {
        if (args.DamageDelta == null)
            return;

        var integrityChanged =  Math.Clamp(ent.Comp.Integrity - (float)(args.DamageDelta.GetTotal() / ent.Comp.MaxDamage), 0f, 1f);
        ent.Comp.Integrity = integrityChanged;

        if (!TryComp<BodyPartComponent>(ent.Owner, out var bodyPart)
            || bodyPart.Body is not { } bodyUid)
            return;

        UpdateIntegrity(bodyUid, _body.GetTargetBodyPart(bodyPart.PartType, bodyPart.Symmetry), integrityChanged);
    }

    public virtual void UpdateIntegrity(EntityUid uid, TargetBodyPart bodyPart, float integrityChanged)
    {

    }

    public float GetMaxDamage(Entity<WoundablePartComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return 0;

        return ent.Comp.MaxDamage;
    }

    public bool HasTargetPartUid(Entity<WoundableComponent?> ent, TargetBodyPart targetPart, out EntityUid? partUid)
    {
        if (!Resolve(ent, ref ent.Comp, false) || !ent.Comp.PartsWoundable.ContainsKey(targetPart)) {
            partUid = null;
            return false;
        }

        partUid = ent.Comp.PartsWoundable[targetPart];
        if (partUid == null)
            return false;

        return true;
    }
}
