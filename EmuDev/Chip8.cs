namespace EmuDev
{
    public class Chip8 : Cpu
    {
        public Chip8(Random random) : base(random) { }

        public bool ExecuteOp()
        {
            ushort opcode = Memory.ReadShort(Pc);
            byte firstNibble = (byte)(opcode >> 12);

            switch (firstNibble)
            {
                case 0x0:
                    Handle0(opcode);
                    break;
                case 0x1:
                    if (Bits.GetNNN(opcode) == Pc)
                        return false;
                    Handle1(opcode);
                    break;
                case 0x2:
                    Handle2(opcode);
                    break;
                case 0x3:
                    Handle3(opcode);
                    break;
                case 0x4:
                    Handle4(opcode);
                    break;
                case 0x5:
                    Handle5(opcode);
                    break;
                case 0x6:
                    Handle6(opcode);
                    break;
                case 0x7:
                    Handle7(opcode);
                    break;
                case 0x8:
                    Handle8(opcode);
                    break;
                case 0x9:
                    Handle9(opcode);
                    break;
                case 0xA:
                    HandleA(opcode);
                    break;
                case 0xB:
                    HandleB(opcode);
                    break;
                case 0xC:
                    HandleC(opcode);
                    break;
                case 0xD:
                    HandleD(opcode);
                    break;
                case 0xE:
                    HandleE(opcode);
                    break;
                case 0xF:
                    HandleF(opcode);
                    break;
                default:
                    throw new ArgumentException("Operation doesn't exist");
            }

            return true;
        }

        public void RunProgram()
        {
            while (ExecuteOp())
            {
                if (DelayTimer > 0)
                    DelayTimer--;
                if (SoundTimer > 0)
                    SoundTimer--;

                Keyboard.ReadInput();
                Display.Draw();
            }
        }
    }
}