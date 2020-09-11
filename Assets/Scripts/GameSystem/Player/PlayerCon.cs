using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    static PlayerCon _instance = null;

    // オブジェクト実体の参照（初期参照時、実体の登録も行う）
    static PlayerCon instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<PlayerCon>()); }
    }

    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }

        // 以降破棄しない
        DontDestroyOnLoad(gameObject);

    }

    void OnDestroy()
    {
        if (this == instance) _instance = null;
    }

}
