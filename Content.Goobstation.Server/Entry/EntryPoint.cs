// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.IoC;
using Content.Goobstation.Server.VoiceChat;
using Robust.Shared.ContentPack;

namespace Content.Goobstation.Server.Entry;

public sealed class EntryPoint : GameServer
{
    private VoiceChatManager _voiceManager = default!;
    private VoiceLogManager _voiceLogs = default!;

    public override void Init()
    {
        base.Init();

        ServerGoobContentIoC.Register();

        IoCManager.BuildGraph();

        _voiceManager = IoCManager.Resolve<VoiceChatManager>();
        _voiceManager.Initialize();

        _voiceLogs = IoCManager.Resolve<VoiceLogManager>();
        _voiceLogs.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _voiceManager?.Shutdown();
        _voiceLogs?.Shutdown();
    }
}
