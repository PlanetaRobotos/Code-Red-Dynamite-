using Core.Logging;
using UnityEngine;

namespace Core.Logging
{
    public class GameLogMessage : ScriptableObject, IGameLogger
    {
        public void AddMessage(LogMessage message, GameObject console)
        {
        }
    }
}