namespace EmuDev;

public static class Bits
{
    public static byte GetNibble(ushort instruction, int n)
    {
        return (byte)((instruction >> (4 * (3 - n))) & 0xF);
    }

    public static byte GetN(ushort instruction)
    {
        return (byte)(instruction & 0xF);
    }

    public static byte GetNN(ushort instruction)
    {
        return (byte)(instruction & 0xFF);
    }

    public static ushort GetNNN(ushort instruction)
    {
        return (ushort)(instruction & 0xFFF);
    }

    public static byte GetX(ushort instruction)
    {
        return (byte)((instruction >> 8) & 0xF);
    }

    public static byte GetY(ushort instruction)
    {
        return (byte)((instruction >> 4) & 0xF);
    }
}