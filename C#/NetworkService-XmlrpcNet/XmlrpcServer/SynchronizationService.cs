using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LamportClock
{
    private int currentTime;

    public LamportClock()
    {
        currentTime = 0;
    }

    public void UpdateLamportClock(int inputTime = 0)
    {
        int maxLamportClock = Math.Max(inputTime, currentTime);
        currentTime = maxLamportClock + 1;
    }

    public int getCurrentTime()
    {
        return currentTime;
    }
}

public class RicartAgrawala
{
    
}