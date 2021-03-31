using UnityEngine;

namespace Core.Logging
{
    public interface IGameLogger
    {
        void AddMessage(LogMessage message, GameObject console);
    }
}