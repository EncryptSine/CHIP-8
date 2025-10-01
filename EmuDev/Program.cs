using EmuDev;




Chip8 chip8 = new Chip8(new Random());
chip8.Memory.LoadRom("tetris.ch8");
chip8.RunProgram();