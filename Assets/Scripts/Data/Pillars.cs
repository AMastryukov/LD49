public class Pillars
{
    public static Pillars operator +(Pillars a, Pillars b)
    {
        a.Military += b.Military;
        a.Economy += b.Economy;
        a.Culture += b.Culture;

        return a;
    }     

    public int Military { get; set; } = 0;
    public int Economy { get; set; } = 0;
    public int Culture { get; set; } = 0;
}
