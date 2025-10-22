using UnityEngine;

public struct TimeOfDayHoursMinutes
{
    public static TimeOfDayHoursMinutes MinutesToHMTime(float aTimeOfDayInMinutes)
    {
        TimeOfDayHoursMinutes myTime = new TimeOfDayHoursMinutes();
        myTime.hours = (int)(aTimeOfDayInMinutes / 60f);
        myTime.minutes = (int)(aTimeOfDayInMinutes - (myTime.hours * 60));
        return myTime;
    }

    int hours;
    int minutes;

    public void SetTime(float aTimeOfDayInMinutes)
    {
        this = MinutesToHMTime(aTimeOfDayInMinutes);
    }

    public override string ToString()
    {
        string h = "0";
        if (hours > 9) h = hours.ToString();
        else h += hours.ToString();

        string m = "0";
        if (minutes > 9) m = minutes.ToString();
        else m += minutes.ToString();
        return h + ":" + m;
    }
}