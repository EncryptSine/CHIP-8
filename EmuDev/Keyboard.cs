namespace EmuDev;

public class Keyboard
{
    public bool[] Keys = new bool[16];

    public bool this[int i]
    {
        get
        {
            var res = Keys[i];
            Keys[i] = false;
            return res;
        }
    }

    public void ReadInput()
    {
        while (Console.KeyAvailable)
        {
            var inputCode = Console.ReadKey(true).Key switch
            {
                ConsoleKey.D1 => 0x1,
                ConsoleKey.D2 => 0x2,
                ConsoleKey.D3 => 0x3,
                ConsoleKey.D4 => 0xc,
                ConsoleKey.Q => 0x4,
                ConsoleKey.W => 0x5,
                ConsoleKey.E => 0x6,
                ConsoleKey.R => 0xd,
                ConsoleKey.A => 0x7,
                ConsoleKey.S => 0x8,
                ConsoleKey.D => 0x9,
                ConsoleKey.F => 0xe,
                ConsoleKey.Z => 0xa,
                ConsoleKey.X => 0x0,
                ConsoleKey.C => 0xb,
                ConsoleKey.V => 0xf,
                _ => -1
            };
            if (inputCode != -1)
                Keys[inputCode] = true;
        }
    }
}