using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class NotificationManager : AbstractService<INotificationManager>, INotificationManager
    {
        [Inject] NotificationMessage.Factory messageFactory = default;

        public void DisplayMessage(string message)
        {
            messageFactory.Create(message);
        }
    }
}
