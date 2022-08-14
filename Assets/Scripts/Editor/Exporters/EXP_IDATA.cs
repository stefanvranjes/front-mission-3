using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

                if (reader.ReadUInt32() == 0x10)
                {
                    if (reader.ReadUInt32() != 8)
                    {
                        Debug.Log("No CLUT!");
                        return;
                    }

                    int timOffset = reader.ReadInt32();
                    reader.BaseStream.Seek(4, SeekOrigin.Current);
                    int clutWidth = reader.ReadInt16();
                    int clutHeight = reader.ReadInt16();
                    reader.BaseStream.Seek(sectorPosition, SeekOrigin.Begin);
                    byte[] clutBuffer = reader.ReadBytes(clutWidth * clutHeight * 2 + 0x14);
                    string clutFile = outDir + Path.DirectorySeparatorChar;
                    clutFile += "IDATA_" + i.ToString("D2") + ".clut";
                    File.WriteAllBytes(clutFile, clutBuffer);
                    AssetDatabase.Refresh();
                    reader.BaseStream.Seek(sectorPosition + timOffset + 0x10, SeekOrigin.Begin);
                    int timWidth = reader.ReadInt16();
                    int timHeight = reader.ReadInt16();
                    reader.BaseStream.Seek(-12, SeekOrigin.Current);
                    byte[] timBuffer = reader.ReadBytes(timWidth * timHeight * 2 + 0xc);
                    string timFile = outDir + Path.DirectorySeparatorChar;
                    timFile += "IDATA_" + i.ToString("D2") + ".tim";
                    File.WriteAllBytes(timFile, timBuffer);

                    if (timFile.StartsWith(Application.dataPath))
                        timFile = "Assets" + timFile.Substring(Application.dataPath.Length);

                    if (clutFile.StartsWith(Application.dataPath))
                        clutFile = "Assets" + clutFile.Substring(Application.dataPath.Length);

                    TimPostprocessor.script = true;
                    TimPostprocessor.clut = AssetDatabase.LoadAssetAtPath<ClutScriptableObject>(clutFile);
                    AssetDatabase.Refresh();
                    TimPostprocessor.script = false;
                }
            }
        }
    }
}
