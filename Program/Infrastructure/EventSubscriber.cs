using System;

namespace MEATaste.Infrastructure
{
    internal record EventSubscriber(EventType EventType, Action Action);
}