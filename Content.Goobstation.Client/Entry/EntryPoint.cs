// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aidenkrz <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 Sara Aldrete's Top Guy <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Client.IoC;
using Content.Goobstation.Client.VoiceChat;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Client.Entry;

public sealed partial class EntryPoint : GameClient
{
    [Dependency] private VoiceChatManager _voiceManager = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;

    public override void Init()
    {
        ContentGoobClientIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);

        _prototypeManager.RegisterIgnore("achievement");
    }

    public override void PostInit()
    {
        base.PostInit();

        _voiceManager.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _voiceManager?.Shutdown();
    }
}
