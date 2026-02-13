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
        SubscribeLocalEvent<WoundablePartComponent, DamageChangedEvent>(OnDamageChanged);
        //SubscribeLocalEvent<WoundableComponent, BodyPartsInitializedEvent>(OnBodyPartsInit);
    }

    /*public virtual void OnBodyPartsInit(Entity<WoundableComponent> ent, ref BodyPartsInitializedEvent args)
    {
        if (!TryComp<BodyComponent>(ent.Owner, out var body) || body.RootContainer == null)
            return;

        var partsOnInit = new List<TargetBodyPart>();

        foreach (var (partUid, partComp) in _body.GetBodyChildren(ent.Owner, body))
        {
            var targetPart = _body.GetTargetBodyPart(partComp.PartType, partComp.Symmetry);

            partsOnInit.Add(targetPart);
        }

        ent.Comp.PartsOnInit = partsOnInit;
        Dirty(ent.Owner, ent.Comp);
    }*/

    public void OnDamageChanged(Entity<WoundablePartComponent> ent, ref DamageChangedEvent args)
    {
        if (args.DamageDelta == null)
            return;

        var integrity =  Math.Clamp(ent.Comp.Integrity - (float)(args.DamageDelta.GetTotal() / ent.Comp.MaxDamage), 0f, 1f);
        ent.Comp.Integrity = integrity;

        if (!TryComp<BodyPartComponent>(ent.Owner, out var bodyPart)
            || bodyPart.Body is not { } bodyUid)
            return;

        UpdateIntegrity(bodyUid, _body.GetTargetBodyPart(bodyPart.PartType, bodyPart.Symmetry), integrity);
    }


    public float GetMaxDamage(Entity<WoundablePartComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return 0;

        return ent.Comp.MaxDamage;
    }

    public virtual void UpdateIntegrity(EntityUid uid, TargetBodyPart bodyPart, float integrity)
    {
    }
}
