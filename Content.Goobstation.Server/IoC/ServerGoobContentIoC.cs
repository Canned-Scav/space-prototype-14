// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.VoiceChat;

namespace Content.Goobstation.Server.IoC;

internal static class ServerGoobContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;

        instance.Register<VoiceChatManager>();
        instance.Register<VoiceLogManager>();
    }
}
