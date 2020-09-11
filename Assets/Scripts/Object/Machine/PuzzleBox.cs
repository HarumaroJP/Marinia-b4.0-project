using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleBox : MonoBehaviour, IUsable
{
    [SerializeField] private int randomizeCount = 3;
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    [SerializeField] private Material metal;
    [SerializeField] private List<Renderer> materials;
    [SerializeField] private List<Renderer> lastCountRenderers;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private PhaseSystem phaseSystem;
    [SerializeField] private ToolSetter toolSetter;
    [SerializeField] private int counter;
    [SerializeField] private int maxCount;
    [SerializeField] public bool isLoaded;
    [SerializeField] private ItemData allowData;

    private int[][] checkPattern = new int[][]
    {
        new int[] {1, 0},
        new int[] {-1, 0},
        new int[] {0, 1},
        new int[] {0, -1}
    };

    private bool[,] map =
    {
        {false, false, false, false},
        {false, false, false, false},
        {false, false, false, false},
        {false, false, false, false}
    };

    private int x, y;
    int length;

    void Start()
    {
        length = map.GetLength(1);
    }

    public void PuzzleReset()
    {
        foreach (Renderer renderer in materials) renderer.material = red;
        foreach (Renderer renderer in lastCountRenderers) renderer.material = blue;

        map = new bool[4, 4];
    }

    public void Initialize()
    {
        // Debug.Log(nameof(Initialize) + " isFighting");
        if (phaseSystem.isFighting) return;

        if (!isLoaded)
        {
            // Debug.Log(nameof(Initialize) + " !isLoaded");
            if (!FlagCore.Instance.IsFlagComplete(15)) return;
            if (ItemLibrary.Instance.itemSlotIDList[toolSetter.activeIndex].key != allowData) return;
            ItemLibrary.Instance.RemoveItemsForMenu(allowData, 1);
            // Debug.Log(nameof(Initialize) + " Extended!!");
            phaseSystem.ExtendPuzzleBox1();
            return;
        }

        eventManager.SetCamera(true);
        eventManager.SetPriorityDefaultCamera(1);
        InGameMenu.Instance.SetOverlay(true, false);
        InGameMenu.Instance.OnUIWithoutTimeScale();
        mainCollider.enabled = false;
        maxCount = lastCountRenderers.Count;

        PuzzleReset();

        for (int i = 0; i < randomizeCount; i++)
        {
            bool result = true;
            Vector2Int coordinate = new Vector2Int(0, 0);
            int index = 0;
            while (result)
            {
                index = Random.Range(0, map.Length);
                coordinate = ConvertIndexToXY(index);
                result = map[coordinate.x, coordinate.y];
            }

            map[coordinate.x, coordinate.y] = true;
            SetMaterial(index, true);
        }

        counter = 0;
    }

    public void SetBlock(int index)
    {
        Vector2Int coordinate = ConvertIndexToXY(index);
        // Debug.Log($"{coordinate.x} + {coordinate.y}");
        counter++;

        foreach (int[] pattern in checkPattern)
        {
            x = coordinate.x + pattern[0];
            y = coordinate.y + pattern[1];

            if (0 <= x && x < length && 0 <= y && y < length)
            {
                map[x, y] = !map[x, y];
                SetMaterial(ConvertXYToIndex(new Vector2Int(x, y)), map[x, y]);
            }
        }

        map[coordinate.x, coordinate.y] = true;
        SetMaterial(index, true);

        lastCountRenderers[counter - 1].material = metal;

        if (map.Cast<bool>().All(x => x))
        {
            eventManager.RunEvent();
        }

        if (counter == lastCountRenderers.Count)
        {
            eventManager.SetCamera(true);
            eventManager.SetPriorityDefaultCamera(0);
            InGameMenu.Instance.SetOverlay(true, true);
            InGameMenu.Instance.OffUIWithoutTimeScale();
            mainCollider.enabled = true;
            phaseSystem.QuickPhase(0);
        }
    }

    private Vector2Int ConvertIndexToXY(int index)
    {
        int tmpIndex = index;
        int x = 0, y = 0;
        while (tmpIndex - length >= 0)
        {
            tmpIndex -= length;
            y++;
        }

        x = tmpIndex;

        return new Vector2Int(y, x);
    }

    private int ConvertXYToIndex(Vector2Int coordinate)
    {
        return coordinate.x * length + coordinate.y;
    }


    private void SetMaterial(int index, bool result)
    {
        materials[index].material = result ? blue : red;
    }
}