using System;

public partial class SROptions
{
    public event Action OnLockAllContent;
    
    public void OnlockAllContent()
    {
        OnLockAllContent?.Invoke();
    }
}