namespace EmuDev
{
    public class Cpu
    {
        public Memory Memory;
        public Display Display;
        public Keyboard Keyboard;
        public Stack<ushort> Stack;
        public ushort I;
        public ushort Pc;
        public byte DelayTimer;
        public byte SoundTimer;
        private Random _random;

        public Cpu(Random random)
        {
            Memory = new Memory();
            Display = new Display();
            Keyboard = new Keyboard();
            Stack = new Stack<ushort>();
            Pc = 0x200;
            I = 0;
            _random = random;
            DelayTimer = 0;
            SoundTimer = 0;
        }

        private void Done()
        {
            Pc += 2;
        }

        private void SkipIf(bool condition)
        {
            Pc += (ushort)(condition ? 4 : 2);
        }

        public void Handle0(ushort opcode)
        {
            switch (Bits.GetNNN(opcode))
            {
                case 0x0E0:
                    for (int i = 0; i < Display.Length; i++)
                        Display[i] = false;
                    Done();
                    break;
                case 0x0EE:
                    if (Stack.Count == 0)
                        throw new IndexOutOfRangeException();
                    Pc = Stack.Pop();
                    Done();
                    break;
                default:
                    throw new Exception("unhandled 0NNN");
            }
        }

        public void Handle1(ushort opcode)
        {
            Pc = Bits.GetNNN(opcode);
        }

        public void Handle2(ushort opcode)
        {
            Stack.Push(Pc);
            Pc = Bits.GetNNN(opcode);
        }

        public void Handle3(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);
            SkipIf(Memory.Registers[x] == nn);
        }

        public void Handle4(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);
            SkipIf(Memory.Registers[x] != nn);
        }

        public void Handle5(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var y = Bits.GetY(opcode);
            SkipIf(Memory.Registers[x] == Memory.Registers[y]);
        }

        public void Handle6(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);
            Memory.Registers[x] = nn;
            Done();
        }

        public void Handle7(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);
            Memory.Registers[x] += nn;
            Done();
        }

        public void Handle8(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var y = Bits.GetY(opcode);
            var n = Bits.GetN(opcode);
            switch (n)
            {
                case 0x0:
                    Memory.Registers[x] = Memory.Registers[y];
                    break;
                case 0x1:
                    Memory.Registers[x] |= Memory.Registers[y];
                    break;
                case 0x2:
                    Memory.Registers[x] &= Memory.Registers[y];
                    break;
                case 0x3:
                    Memory.Registers[x] ^= Memory.Registers[y];
                    break;
                case 0x4:
                    var sum = Memory.Registers[x] + Memory.Registers[y];
                    Memory.Registers[0xF] = (byte)(sum > 0xFF ? 1 : 0);
                    Memory.Registers[x] = (byte)sum;
                    break;
                case 0x5:
                    Memory.Registers[0xF] = (byte)(Memory.Registers[x] >= Memory.Registers[y] ? 1 : 0);
                    Memory.Registers[x] = (byte)(Memory.Registers[x] - Memory.Registers[y]);
                    break;
                case 0x6:
                    Memory.Registers[0xF] = (byte)(Memory.Registers[x] & 0x1);
                    Memory.Registers[x] >>= 1;
                    break;
                case 0x7:
                    Memory.Registers[0xF] = (byte)(Memory.Registers[x] > Memory.Registers[y] ? 0 : 1);
                    Memory.Registers[x] = (byte)(Memory.Registers[y] - Memory.Registers[x]);
                    break;
                case 0xE:
                    Memory.Registers[0xF] = (byte)((Memory.Registers[x] & 0x80) >> 7);
                    Memory.Registers[x] <<= 1;
                    break;
                default:
                    throw new Exception("Invalid operation");
            }
            Done();
        }

        public void Handle9(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var y = Bits.GetY(opcode);
            SkipIf(Memory.Registers[x] != Memory.Registers[y]);
        }

        public void HandleA(ushort opcode)
        {
            I = Bits.GetNNN(opcode);
            Done();
        }

        public void HandleB(ushort opcode)
        {
            Pc = (ushort)(Bits.GetNNN(opcode) + Memory.Registers[0]);
        }
        
        public void HandleC(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);
            Memory.Registers[x] = (byte)(_random.Next(0, 256) & nn);
            Done();
        }

        public void HandleD(ushort opcode)
        {
            var x = Memory.Registers[Bits.GetX(opcode)];
            var y = Memory.Registers[Bits.GetY(opcode)];
            var n = Bits.GetN(opcode);
            Memory.Registers[0xF] = 0;

            for (int lineIndex = 0; lineIndex < n; lineIndex++)
            {
                var spriteLine = Memory.Ram[I + lineIndex];
                for (int i = 0; i < 8; i++)
                {
                    var pixelIndex = (x + i + (y + lineIndex) * 64) % 2048;
                    var spritePixel = (spriteLine >> (7 - i)) & 1;
                    if (spritePixel == 1 && Display[pixelIndex])
                        Memory.Registers[0xF] = 1;
                    Display[pixelIndex] ^= spritePixel == 1;
                }
            }
            Done();
        }

        public void HandleE(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var key = Memory.Registers[x];
            switch (Bits.GetNN(opcode))
            {
                case 0x9E:
                    SkipIf(Keyboard[key]);
                    break;
                case 0xA1:
                    SkipIf(!Keyboard[key]);
                    break;
                default:
                    throw new Exception("Invalid instruction");
            }
        }
        public void HandleF(ushort opcode)
        {
            var x = Bits.GetX(opcode);
            var nn = Bits.GetNN(opcode);

            switch (nn)
            {
                case 0x07:
                    Memory.Registers[x] = DelayTimer;
                    break;
                case 0x0A:
                    for (byte i = 0; i < 16; i++)
                    {
                        if (Keyboard[i])
                        {
                            Memory.Registers[x] = i;
                            break;
                        }
                    }
                    break;
                case 0x15:
                    DelayTimer = Memory.Registers[x];
                    break;
                case 0x18:
                    SoundTimer = Memory.Registers[x];
                    break;
                case 0x1E:
                    I += Memory.Registers[x];
                    break;
                case 0x29:
                    I = (ushort)(Memory.Registers[x] * 5);
                    break;
                case 0x33:
                    Memory.Ram[I] = (byte)(Memory.Registers[x] / 100);
                    Memory.Ram[I + 1] = (byte)((Memory.Registers[x] / 10) % 10);
                    Memory.Ram[I + 2] = (byte)(Memory.Registers[x] % 10);
                    break;
                case 0x55:
                    for (int i = 0; i <= x; i++)
                        Memory.Ram[I + i] = Memory.Registers[i];
                    break;
                case 0x65:
                    for (int i = 0; i <= x; i++)
                        Memory.Registers[i] = Memory.Ram[I + i];
                    break;
                default:
                    throw new Exception("Invalid instruction");
            }
            Done();
        }
    }
}