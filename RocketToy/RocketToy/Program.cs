using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RocketToy
{
    class Program
    {
        static readonly byte[] CarSignature = new byte[] { 0x6B, 0x0B, 0x5E, 0x5D, 0xD0, 0x03 };
        //static readonly byte[] BallSignature = new byte[] { 0x6B, 0x0B, 0x5E, 0x5D, 0x90, 0x04 };//0xFE, 0x42, 0x10, 0xD2 };
        //static readonly byte[] BallSignature = new byte[] { 0x6B, 0x0B, 0x5E, 0x5D, 0x10, 0x07 };
        static readonly byte[] EntitySignature = new byte[] { 0x6B, 0x0B, 0x5E, 0x5D };

        static void Main(string[] args)
        {
            Console.WriteLine("Rocket Toy unstable version 0.1");
            Console.Write("Press enter to attatch to RL...");
            Console.ReadLine();
            Process rl = null;
            try
            {
                rl = Process.GetProcessesByName("RocketLeague")[0];
            } catch
            {
                Console.WriteLine("Couldn't find RL");
                while (true) { }
            }
            Console.Write("RL found! Please ensure that Rocket League is currently in free play mode, then press enter...");
            Console.ReadLine();
            Console.WriteLine("Scanning memory...");
            //MemoryEditor memEditor = new MemoryEditor(rl);
            Entity car = new Entity(CarSignature, rl);
            /*List<Entity> entities = new List<Entity>();
            foreach (int address in memEditor.GetAddressesOf(EntitySignature))
            {
                entities.Add(new Entity(EntitySignature, address, rl));
            }
            foreach (Entity e in entities)
            {
                e.PrintCoordinates();
            }
            Console.ReadKey();*/
            float oldX = -10000;
            float oldY = 0;
            float oldZ = 0;
            while (true)
            {
                if (car.VelocityY < 0)
                {
                    car.VelocityY = 0;
                }
                if (car.PositionX != oldX || car.PositionY != oldY || car.PositionZ != oldZ)
                {
                    oldX = car.PositionX;
                    oldY = car.PositionY;
                    oldZ = car.PositionZ;
                    Console.Clear();
                    Console.WriteLine("Controlled flying enabled.");
                    Console.WriteLine("X: " + oldX.ToString() + " Y: " + oldY.ToString() + " Z: " + oldZ.ToString());
                    Console.WriteLine("VelX: " + car.VelocityX.ToString() + " VelY: " + car.VelocityY.ToString() + " VelZ: " + car.VelocityZ.ToString());
                    Console.WriteLine("This screen refreshes automatically. Press enter to teleport or re-teleport outside map at any time.");
                }
                if (Console.KeyAvailable)
                {
                    Console.ReadKey();
                    car.VelocityX = 0;
                    car.VelocityY = 0;
                    car.VelocityZ = 0;
                    car.PositionX = 0;
                    car.PositionY = 40;
                    car.PositionZ = -170;
                }
            }
        }
    }
}
