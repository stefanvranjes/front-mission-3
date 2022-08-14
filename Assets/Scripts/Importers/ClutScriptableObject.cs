using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutScriptableObject : ScriptableObject
{
    public string prefabName;

    public int VRAM_X;
    public int VRAM_Y;
    public int WIDTH;
    public int HEIGHT;
    public ushort[] PALETTE;
    public Texture2D TEX_2D;

    public ushort GetIndex(int id, int y)
    {
        int index = id + y * 0x10;

        if (index < PALETTE.Length)
            return PALETTE[index];
        else
            return 0;
    }
}
