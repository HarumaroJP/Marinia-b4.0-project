using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    void LoadSaveData();
    void OnAfterSave();
    void OnBeforeLoad();
    string GetObjectData();
}