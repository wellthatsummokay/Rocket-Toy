using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketToy
{
    class Entity
    {
        private static readonly int positionOffset = -132;
        private static readonly int velocityOffset = 140;
        private int index;
        private byte[] signature;
        private int signatureLocation = 0;
        private MemoryEditor memEditor;

        public byte[] FullSignature
        {
            get
            {
                return memEditor.ReadMemory(signatureLocation, 10);
            }
        }
        public float PositionX
        {
            get
            {
                track();
                byte[] posX = memEditor.ReadMemory(signatureLocation + positionOffset, 4);
                return BitConverter.ToSingle(posX, 0);
            }
            set
            {
                track();
                byte[] posX = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + positionOffset, posX);
            }
        }
        public float PositionZ
        {
            get
            {
                track();
                byte[] posZ = memEditor.ReadMemory(signatureLocation + positionOffset + 4, 4);
                return BitConverter.ToSingle(posZ, 0);
            }
            set
            {
                track();
                byte[] posZ = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + positionOffset + 4, posZ);
            }
        }
        public float PositionY
        {
            get
            {
                track();
                byte[] posY = memEditor.ReadMemory(signatureLocation + positionOffset + 8, 4);
                return BitConverter.ToSingle(posY, 0);
            }
            set
            {
                track();
                byte[] posY = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + positionOffset + 8, posY);
            }
        }
        public float VelocityX
        {
            get
            {
                track();
                byte[] velX = memEditor.ReadMemory(signatureLocation + velocityOffset, 4);
                return BitConverter.ToSingle(velX, 0);
            }
            set
            {
                track();
                byte[] velX = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + velocityOffset, velX);
            }
        }
        public float VelocityZ
        {
            get
            {
                track();
                byte[] velZ = memEditor.ReadMemory(signatureLocation + velocityOffset + 4, 4);
                return BitConverter.ToSingle(velZ, 0);
            }
            set
            {
                track();
                byte[] velZ = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + velocityOffset + 4, velZ);
            }
        }
        public float VelocityY
        {
            get
            {
                track();
                byte[] velY = memEditor.ReadMemory(signatureLocation + velocityOffset + 8, 4);
                return BitConverter.ToSingle(velY, 0);
            }
            set
            {
                track();
                byte[] velY = BitConverter.GetBytes(value);
                memEditor.WriteMemory(signatureLocation + velocityOffset + 8, velY);
            }
        }

        public Entity(byte[] Signature, Process RL, int Index = 0)
        {
            signature = Signature;
            memEditor = new MemoryEditor(RL);
            index = Index;
            track();
        }
        public Entity(byte[] Signature, int KnownAddress, Process RL)
        {
            signature = Signature;
            signatureLocation = KnownAddress;
            memEditor = new MemoryEditor(RL);
            index = 0;
            track();
        }
        public void PrintCoordinates()
        {
            Console.WriteLine("X: " + PositionX + " Y: " + PositionY + " Z: " + PositionZ);
        }
        private void track()
        {
            if (signatureLocation == 0)
            {
                signatureLocation = memEditor.GetAddressesOf(signature)[index];
            }
            else if (!memEditor.ReadMemory(signatureLocation, signature.Length).SequenceEqual(signature))
            {
                signatureLocation = memEditor.GetAddressesOf(signature)[index];
            }
        }
    }
}
