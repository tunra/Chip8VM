using System.Drawing;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Chip8VM
{
    class Program
    {
        static void Main(string[] args)
        {
            CPU cpu = new CPU();

            using (BinaryReader reader = new BinaryReader(new FileStream("IBM Logo.ch8", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var opcode = (ushort)((reader.ReadByte() << 8) | reader.ReadByte());

                    try
                    {
                        cpu.executeOpcode(opcode);
                    } catch (Exception e) { 
                        Console.WriteLine(e.Message);
                    }
                }
            }

        }
    }

    /*  
     *  https://tobiasvl.github.io/blog/write-a-chip-8-emulator/
     *  
     *  Memory: CHIP-8 has direct access to up to 4 kilobytes of RAM
     *  Display: 64 x 32 pixels (or 128 x 64 for SUPER-CHIP) monochrome, ie. black or white
     *  A program counter, often called just “PC”, which points at the current instruction in memory
     *  One 16-bit index register called “I” which is used to point at locations in memory
     *  A stack for 16-bit addresses, which is used to call subroutines/functions and return from them
     *  An 8-bit delay timer which is decremented at a rate of 60 Hz (60 times per second) until it reaches 0
     *  An 8-bit sound timer which functions like the delay timer, but which also gives off a beeping sound as long as it’s not 0
     *  16 8-bit (one byte) general-purpose variable registers numbered 0 through F hexadecimal, ie. 0 through 15 in decimal, called V0 through VF
     *  VF is also used as a flag register; many instructions will set it to either 1 or 0 based on some rule, for example using it as a carry flag
    */
    class CPU
    {
        public byte[] memory = new byte[4096];
        public byte[] v = new byte[16];
        public ushort i = 0;
        public ushort pc = 0x200;
        public ushort[] stack = new ushort[24];
        //public Stack<ushort> stack = new Stack<ushort>();
        public byte[] keys = new byte[16];
        public byte DelayTimer;
        public byte SoundTimer;

        public byte[] display = new byte[64 * 32];

        public void Run()
        {
            // test memory
            memory[pc] = 0xEF;
            memory[pc + 1] = 224;
            ushort opcode = (ushort)(memory[pc] << 8 | memory[pc + 1]);

            byte[] bytes = new byte[1] { memory[pc] };

            Console.WriteLine(Convert.ToHexString(bytes));
            Console.WriteLine(Convert.ToString(memory[pc], 2));
        }

        public void executeOpcode(ushort opcode)
        {
            byte nibble = (byte)(opcode & 0b1111_0000_0000_0000);

            switch (nibble)
            {
                case 0x00:
                    if (opcode == 0x00E0)
                    {
                        display = new byte[64 * 32];
                    }
                    else if (opcode == 0x00EE)
                    {
                        Console.WriteLine($"executed opcode:{opcode.ToString("X")}");

                        i = stack.Pop();
                    }
                    else
                    {
                        throw new Exception($"Unsupported opcode: {opcode.ToString("X4")}");
                    }
                    break;
                default:
                    throw new Exception($"Unsupported opcode: {opcode.ToString("X4")}");
            }
        }
    }
}