// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Systems.Chat.Controls;
using Content.Goobstation.Client.VoiceChat;
using Content.Goobstation.Client.VoiceChat.UI;
using Robust.Shared.IoC;

namespace Content.Goobstation.Client.IoC;

internal static class ContentGoobClientIoC
{
    internal static void Register()
    {
        var collection = IoCManager.Instance!;

        collection.Register<VoiceChatManager>();
        collection.Register<IChatVoiceControls, ChatVoiceControls>();
    }
}
