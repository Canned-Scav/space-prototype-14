using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Trigger.Systems;
using Content.Shared.Lock;

namespace Content.Shared.ScavPrototype.Trigger;

public sealed partial class TriggerOnStrappedOrBuckledSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnLockComponent, LockToggledEvent>(OnLockToggled);
    }

    private void OnLockToggled(Entity<TriggerOnLockComponent> ent, ref LockToggledEvent args)
    {
        IoCManager.Resolve<IEntitySystemManager>()
            .GetEntitySystem<TriggerSystem>()
            .Trigger(ent.Owner, ent.Owner, ent.Comp.KeyOut);
    }
}
