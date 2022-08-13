using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXP_IDATA
{
    public static void ExtractIDATA(string inFile, string outDir)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open)))
        {
            int elementCount = reader.ReadInt32();
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            long begin = reader.BaseStream.Position;

            for (int i = 0; i < elementCount; i++)
            {
                reader.BaseStream.Seek(begin + i * 8, SeekOrigin.Begin);
                int sectorPosition = reader.ReadInt32() * 0x800;
                int sectorSize = reader.ReadInt16() * 0x800;
                reader.BaseStream.Seek(sectorPosition, SeekOrigin.Begin);
                byte[] buffer = reader.ReadBytes(sectorSize);
                string timFile = outDir + Path.DirectorySeparatorChar;
                timFile += "IDATA_" + i.ToString("D2") + ".tim";
                File.WriteAllBytes(timFile, buffer);
            }
        }
    }
}
