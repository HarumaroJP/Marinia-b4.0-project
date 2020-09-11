using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSaveClassHolder
{
}

[Serializable]
public class PlayerSaveType
{
    
}

[Serializable]
public class FurnaceSaveType
{
    public List<SaveManager.ListSaveType> holdData_jsonData;
    public List<SaveManager.ListSaveType> holdData_end_jsonData;
    public List<SaveManager.ListSaveType> fuelData_jsonData;
}

[Serializable]
public class BonfireSaveType
{
    public SaveManager.ListSaveType holdData_jsonData;
    public SaveManager.ListSaveType holdData_end_jsonData;
    public SaveManager.ListSaveType fuelData_jsonData;
}

[Serializable]
public class WorkbenchSaveType
{
    public SaveManager.ListSaveType holdData_jsonData;
}

[Serializable]
public class Chest_SSaveType
{
    public List<SaveManager.ListSaveType> holdData_jsonData;
}

[Serializable]
public class MemoSaveType
{
    public int memoID;
}