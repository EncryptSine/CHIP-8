namespace EmuDev;

public class Display
{
    private bool[] _memory;
    public char[] Str { get; }
    private const int DisplayW = 2 * 64 + 2 + 1; // 2 per pixel, 2 for borders, one for \n
    private const int DisplayH = 34;
    private const int Cpf = 10; // cycles per frame
    private int _cycle = 0;
    
    public int Length => _memory.Length;

    public Display()
    {
        _memory = new bool[32 * 64];
        for (int i = 0; i < _memory.Length; i++)
            _memory[i] = false;

        Str = new char[DisplayW * DisplayH];
        for (int i = 0; i < Str.Length; i++)
            Str[i] = ' ';

        // horizontal lines
        for (int i = 1; i < DisplayW - 2; i++)
        {
            Str[i] = '-';
            Str[(DisplayH - 1) * DisplayW + i] = '-';
        }

        // vertical lines
        for (int i = 0; i < DisplayH; i++)
        {
            Str[i * DisplayW] = '|';
            Str[(i + 1) * DisplayW - 2] = '|';
            Str[(i + 1) * DisplayW - 1] = '\n';
        }
    }

    public bool this[int i]
    {
        get => _memory[i];
        set
        {
            if (_memory[i] == value)
                return;
            _memory[i] = value;
            var c = value ? '█' : ' ';
            var x = i % 64;
            var y = i / 64;
            var idx = (y + 1) * DisplayW + 2 * x + 1;
            Str[idx] = c;
            Str[idx + 1] = c;
        }
    }

    public void Draw()
    {
        _cycle++;
        if (_cycle < Cpf)
            return;
        _cycle = 0;
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.Write(Str);
        Thread.Sleep(60);
    }
}