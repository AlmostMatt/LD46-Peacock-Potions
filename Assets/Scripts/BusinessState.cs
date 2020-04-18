using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessState
{
    public static float money = 0; // TODO: maybe we make a "currency" or "resources" array, if needed
    public static bool[] animals = new bool[(int)AnimalType.AT_MAX]; // TODO: this could be ints for health, or maybe structs if need to track more per-animal
    public static int[] inventory = new int[(int)ProductType.PT_MAX];
    public static float[] prices = new float[(int)ProductType.PT_MAX];
}
