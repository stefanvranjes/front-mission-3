using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AssetImporters;
#endif

#if UNITY_EDITOR
[ScriptedImporter(1, "tim")]
public class IMP_TIM : ScriptedImporter
{
    public ClutScriptableObject clut;
    public int scale;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        byte[] buffer = File.ReadAllBytes(ctx.assetPath);
        
        using (BufferedBinaryReader reader = new BufferedBinaryReader(buffer))
        {
            if (clut != null)
            {
                GridScriptableObject grid = ScriptableObject.CreateInstance("GridScriptableObject") as GridScriptableObject;

                reader.Seek(4, SeekOrigin.Current);
                grid.VRAM_X = reader.ReadInt16();
                grid.VRAM_Y = reader.ReadInt16();
                int width = reader.ReadInt16();
                int height = reader.ReadInt16();
                Texture2D texture4 = new Texture2D(width * 4, height, TextureFormat.RGBA32, false, false);
                Texture2D texture8 = new Texture2D(width * 2, height, TextureFormat.RGBA32, false, false);
                Texture2D preview4 = new Texture2D(width * 4, height, TextureFormat.RGBA32, false, false);
                Texture2D preview8 = new Texture2D(width * 2, height, TextureFormat.RGBA32, false, false);
                Color32[] pixels4 = new Color32[texture4.width * height];
                Color32[] pixels8 = new Color32[texture8.width * height];
                Color32[] _pixels4 = new Color32[preview4.width * height];
                Color32[] _pixels8 = new Color32[preview8.width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < texture4.width; x++)
                    {
                        int index = x + y * texture4.width;

                        if (index % 2 == 0)
                        {
                            int index2 = reader.ReadByte(index / 2) & 0xf;
                            pixels4[index] = new Color32((byte)index2, 0, 0, 0);
                            _pixels4[index] = GetColor32(clut.GetIndex(index2, scale));
                        }
                        else
                        {
                            int index2 = reader.ReadByte(index / 2) >> 4;
                            pixels4[index] = new Color32((byte)index2, 0, 0, 0);
                            _pixels4[index] = GetColor32(clut.GetIndex(index2, scale));
                        }
                    }
                }

                Color32[] flipped4 = new Color32[texture4.width * height];
                Color32[] _flipped4 = new Color32[preview4.width * height];
                int m = 0;

                for (int i = 0, j = pixels4.Length - texture4.width; i < pixels4.Length; i += texture4.width, j -= texture4.width)
                {
                    for (int k = 0; k < texture4.width; ++k)
                    {
                        flipped4[m] = new Color32
                            (pixels4[j + k].r, 
                             pixels4[j + k].g, 
                             pixels4[j + k].b, 
                             pixels4[j + k].a);
                        _flipped4[m++] = new Color32
                            (_pixels4[j + k].r,
                             _pixels4[j + k].g,
                             _pixels4[j + k].b,
                             _pixels4[j + k].a);
                    }
                }

                texture4.name = "4-bit texture";
                texture4.SetPixels32(flipped4);
                texture4.wrapMode = TextureWrapMode.Clamp;
                texture4.filterMode = FilterMode.Point;
                texture4.Apply();
                ctx.AddObjectToAsset("tim4", texture4);
                preview4.name = "4-bit preview";
                preview4.SetPixels32(_flipped4);
                preview4.wrapMode = TextureWrapMode.Clamp;
                preview4.filterMode = FilterMode.Point;
                preview4.Apply();
                ctx.AddObjectToAsset("prev4", preview4);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < texture8.width; x++)
                    {
                        int index = x + y * texture8.width;
                        int index2 = reader.ReadByte(index);
                        pixels8[index] = new Color32((byte)index2, 0, 0, 0);
                        _pixels8[index] = GetColor32(clut.GetIndex(index2, scale));
                    }
                }

                Color32[] flipped8 = new Color32[texture8.width * height];
                Color32[] _flipped8 = new Color32[preview8.width * height];
                m = 0;

                for (int i = 0, j = pixels8.Length - texture8.width; i < pixels8.Length; i += texture8.width, j -= texture8.width)
                {
                    for (int k = 0; k < texture8.width; ++k)
                    {
                        flipped8[m] = new Color32
                            (pixels8[j + k].r,
                             pixels8[j + k].g,
                             pixels8[j + k].b,
                             pixels8[j + k].a);
                        _flipped8[m++] = new Color32
                            (_pixels8[j + k].r,
                             _pixels8[j + k].g,
                             _pixels8[j + k].b,
                             _pixels8[j + k].a);
                    }
                }

                texture8.name = "8-bit texture";
                texture8.SetPixels32(flipped8);
                texture8.wrapMode = TextureWrapMode.Clamp;
                texture8.filterMode = FilterMode.Point;
                texture8.Apply();
                ctx.AddObjectToAsset("tim8", texture8);
                preview8.name = "8-bit preview";
                preview8.SetPixels32(_flipped8);
                preview8.wrapMode = TextureWrapMode.Clamp;
                preview8.filterMode = FilterMode.Point;
                preview8.Apply();
                ctx.AddObjectToAsset("prev8", preview8);
                ctx.SetMainObject(texture8);

                grid.tex4 = texture4;
                grid.tex8 = texture8;
                ctx.AddObjectToAsset("grid", grid);
            }
        }
    }

    private Color32 GetColor32(ushort color16)
    {
        ushort redMask = 0x7C00;
        ushort greenMask = 0x3E0;
        ushort blueMask = 0x1F;

        byte R5 = (byte)((color16 & redMask) >> 10);
        byte G5 = (byte)((color16 & greenMask) >> 5);
        byte B5 = (byte)(color16 & blueMask);

        byte R8 = (byte)(R5 << 3);
        byte G8 = (byte)(G5 << 3);
        byte B8 = (byte)(B5 << 3);

        byte A = 255;

        if (color16 >> 15 == 0)
        {
            if (R8 == 0 && G8 == 0 && B8 == 0)
                A = 0;
            else
                A = 255;
        }
        else
        {
            if (R8 == 0 && G8 == 0 && B8 == 0)
                A = 127;
            else
                A = 127;
        }

        return new Color32(B8, G8, R8, A);
    }
}
#endif