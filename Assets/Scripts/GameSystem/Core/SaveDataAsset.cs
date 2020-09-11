using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class SaveDataAsset : MonoBehaviour
{
    [Serializable]
    public struct SavePlayerObjectType
    {
        public int itemID;

        public float[] _position;
        public float[] _rotation;
        public float[] _scale;

        public string param;

        public SavePlayerObjectType(int itemId, Transform trans, string paramData)
        {
            this.itemID = itemId;

            Vector3 position = trans.localPosition;
            this._position = new float[] {position.x, position.y, position.z};

            Quaternion rotation = trans.localRotation;
            this._rotation = new float[] {rotation.x, rotation.y, rotation.z, rotation.w};

            Vector3 scale = trans.localScale;
            this._scale = new float[] {scale.x, scale.y, scale.z};

            this.param = paramData;
        }
    }
}