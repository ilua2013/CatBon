using System;

namespace Tools
{
    public partial class SROptions
    {
        public event Action OnSubscribeActivated;
        public event Action OnSubscribeDeactivated;
        
        public void OnSubscribeActivate()
        {
            OnSubscribeActivated?.Invoke();
        }
        
        public void OnSubscribeDeactivate()
        {
            OnSubscribeDeactivated?.Invoke();
        }
    }
}