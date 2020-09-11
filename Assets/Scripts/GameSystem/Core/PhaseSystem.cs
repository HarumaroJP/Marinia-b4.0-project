using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class ChildList
{
    public List<PhaseSystem.MonsterSpawnType> list;
    public AllFlagList.FlagType phaseFlagType;
    public int eventID;

    public ChildList(List<PhaseSystem.MonsterSpawnType> list)
    {
        this.list = list;
    }
}

[Serializable]
public class QuickSpawnList
{
    public List<PhaseSystem.MonsterSpawnType> list;

    public QuickSpawnList(List<PhaseSystem.MonsterSpawnType> list)
    {
        this.list = list;
    }
}

public class PhaseSystem : SingletonMonoBehaviour<PhaseSystem>
{
    [SerializeField] public bool isFighting;
    [SerializeField] private List<ChildList> monsterSpawnTypes;
    [SerializeField] private List<QuickSpawnList> quickSpawnTypes;

    [SerializeField] private Transform entityParent;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private CinemachineVirtualCamera bossCamera;
    [SerializeField] private float spawnRadius;
    [SerializeField] private PostProcessManager postProcessManager;
    [SerializeField] private EnvironmentManager environmentManager;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private Vector3[] fieldSizeLevel;
    [SerializeField] private Color[] fieldColorLevel;

    [SerializeField] [ColorUsage(false, true)]
    private Color[] fieldEmissionLevel;

    [SerializeField] private Transform magicField_main;
    [SerializeField] private Transform magicField;
    [SerializeField] private Transform magicRockField;
    [SerializeField] private Transform magicRock;
    [SerializeField] private Transform magicBox;
    [SerializeField] private PuzzleBox puzzleBox;
    [SerializeField] private Transform upper;
    [SerializeField] private Transform buttons;
    [SerializeField] private Transform decoy;
    [SerializeField] private Transform magicField_box;
    [SerializeField] private Transform box_parent;
    [SerializeField] private ParticleSystem particle_box;
    [SerializeField] private ParticleSystem particleSystem_appear;
    [SerializeField] private Renderer puzzleBox_magicField;
    [SerializeField] private Transform puzzleBox_line;
    [SerializeField] private Material yellow;
    [SerializeField] private ParticleSystem particle_main;

    [SerializeField] private Transform coreBlood;

    // [SerializeField] private Transform shockWave;
    [SerializeField] private Animator coreAnim;
    public int nowAttackPhaseID;
    public int nowQuickPhaseID;
    private bool isQuick;
    private Renderer magicFieldMat;
    private float magicFieldSpeed = 0f;
    private static readonly int Color = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int CanRotate = Animator.StringToHash("canRotate");

    [Serializable]
    public struct MonsterSpawnType
    {
        public MonsterData data;
        public int count;
    }

    private void Start()
    {
        magicFieldMat = magicField.GetComponent<Renderer>();
        environmentManager.SetFlowerUsable(false);

        buttons.localPosition = new Vector3(buttons.localPosition.x, -0.05f, buttons.localPosition.z);
        box_parent.localPosition = new Vector3(0f, -2f, 0f);

        upper.transform.localScale = Vector3.zero;
        magicField_box.localScale = Vector3.zero;
        upper.gameObject.SetActive(false);
        magicRock.gameObject.SetActive(false);


        // ExtendPuzzleBox1();
        // SummonMagicRock();
        // BossSpawned();
        // CountdownPhase();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && InGameMenu.Instance.inAnyMode)
        {
            RandomSetMonster();
        }

        // magicField.RotateAround(axis.position, Vector3.up, 2f * speed);
        magicField.Rotate(new Vector3(0f, 2f * magicFieldSpeed, 0f));
    }

    public void CountdownPhase()
    {
        Debug.Log(nameof(CountdownPhase));

        isFighting = true;
        isQuick = false;
        Sequence seq = DOTween.Sequence();
        magicField_main.localScale = Vector3.zero;
        magicField_main.gameObject.SetActive(true);

        coreAnim.SetBool(CanRotate, false);
        magicFieldMat.sharedMaterial.SetColor(Color, fieldColorLevel[0]);
        magicFieldMat.sharedMaterial.SetColor(EmissionColor, fieldEmissionLevel[0]);

        seq.Append(coreBlood.DOScale(0.782405f, 2f));
        seq.Append(magicField_main.DOScale(fieldSizeLevel[0], 2f));
        seq.AppendCallback(() => magicFieldSpeed = 0.2f);
        seq.AppendInterval(2f);
        seq.Append(magicField_main.DOScale(fieldSizeLevel[1], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldColorLevel[1], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldEmissionLevel[1], EmissionColor, 2f));
        seq.AppendCallback(() => magicFieldSpeed = 0.4f);
        seq.AppendInterval(2f);
        seq.Append(magicField_main.DOScale(fieldSizeLevel[2], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldColorLevel[2], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldEmissionLevel[2], EmissionColor, 2f));
        seq.AppendCallback(() => magicFieldSpeed = 0.6f);
        seq.AppendInterval(2f);
        seq.Append(magicField_main.DOScale(fieldSizeLevel[3], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldColorLevel[3], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldEmissionLevel[3], EmissionColor, 2f));
        seq.AppendCallback(() => magicFieldSpeed = 1f);
        seq.AppendInterval(2f);
        seq.Append(magicField_main.DOScale(fieldSizeLevel[4], 2f));
        // seq.Join(shockWave.DOScale(8f, 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldColorLevel[4], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldEmissionLevel[4], EmissionColor, 2f));
        seq.AppendInterval(2f);
        Vector3 localScale = magicField.localScale;
        seq.Append(magicField.DOScale(
            new Vector3(localScale.x * 20f, localScale.y * 20f, localScale.z * 20f), 3f));
        // seq.Join(shockWave.DOScale(150f, 3f));
        seq.AppendCallback(RandomSetMonster);
        seq.AppendInterval(6f);
        seq.Append(magicField.DOScale(0f, 3f));
        // seq.Join(shockWave.DOScale(0f, 3f));
        seq.Join(coreBlood.DOScale(0f, 3f));
        seq.AppendCallback(() =>
        {
            magicField.localScale = localScale;
            // shockWave.localScale = Vector3.zero;
            magicField_main.gameObject.SetActive(false);
            magicFieldSpeed = 0f;
            // eventManager.MoveNextEvent();
        });
        seq.Play().SetDelay(5f);
    }

    public void QuickPhase(int index)
    {
        isFighting = true;
        isQuick = true;
        Sequence seq = DOTween.Sequence();
        magicField_main.localScale = Vector3.zero;
        magicField_main.gameObject.SetActive(true);

        coreAnim.SetBool(CanRotate, false);
        magicFieldMat.sharedMaterial.SetColor(Color, fieldColorLevel[0]);
        magicFieldMat.sharedMaterial.SetColor(EmissionColor, fieldEmissionLevel[0]);

        seq.AppendCallback(() => magicFieldSpeed = 1f);
        seq.Append(magicField_main.DOScale(fieldSizeLevel[4], 2f));
        // seq.Join(shockWave.DOScale(8f, 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldColorLevel[4], 2f));
        seq.Join(magicFieldMat.sharedMaterial.DOColor(fieldEmissionLevel[4], EmissionColor, 2f));
        seq.AppendInterval(2f);
        Vector3 localScale = magicField.localScale;
        seq.Append(magicField.DOScale(
            new Vector3(localScale.x * 20f, localScale.y * 20f, localScale.z * 20f), 3f));
        // seq.Join(shockWave.DOScale(150f, 3f));
        seq.AppendCallback(() => RandomQuickSetMonster(index));
        seq.AppendInterval(6f);
        seq.Append(magicField.DOScale(0f, 3f));
        // seq.Join(shockWave.DOScale(0f, 3f));
        seq.Join(coreBlood.DOScale(0f, 3f));
        seq.AppendCallback(() =>
        {
            magicField.localScale = localScale;
            // shockWave.localScale = Vector3.zero;
            magicField_main.gameObject.SetActive(false);
            magicFieldSpeed = 0f;
        });
        seq.Play().SetDelay(5f);
    }

    public void SummonMagicRock()
    {
        eventManager.SetCamera(true);
        eventManager.SetPriorityDefaultCamera(0);
        Sequence seq = DOTween.Sequence();

        puzzleBox.gameObject.GetComponent<Collider>().enabled = true;
        magicRock.gameObject.SetActive(false);
        float circleSize = 1f;
        magicRockField.localScale = Vector3.zero;
        magicRock.localPosition = new Vector3(0.26f, 7.79f, -6.03f);

        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        seq.AppendCallback(() =>
        {
            InGameMenu.Instance.SetOverlay(false, false);
            eventManager.SetCamera(false);
            eventManager.SetPriorityVirtualCamera(1);
            InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal);
            coreAnim.SetBool(CanRotate, false);
        });
        seq.AppendInterval(4f);
        seq.AppendCallback(() =>
        {
            magicRock.gameObject.SetActive(true);
            particle_main.Play();
        });
        seq.Append(magicRockField.DOScale(circleSize, 1f));

        Quaternion localRotation = magicRockField.localRotation;
        seq.Join(magicRockField.DOLocalRotate(
            new Vector3(localRotation.eulerAngles.x, 90f, localRotation.eulerAngles.z),
            2f).SetEase(Ease.OutBack));

        seq.Append(magicRock.DOLocalMoveZ(magicRock.localPosition.z + 8.7f, 3f).SetEase(Ease.Linear));
        seq.Append(magicRock.DOLocalMoveY(magicRock.localPosition.y - 7f, 2f).SetEase(Ease.OutBack));
        seq.AppendInterval(5f);
        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        seq.AppendCallback(() =>
        {
            magicRockField.localScale = Vector3.zero;
            magicField.localRotation =
                Quaternion.Euler(new Vector3(localRotation.eulerAngles.x, 0f, localRotation.eulerAngles.z));
            particle_main.Stop();

            eventManager.SetCamera(true);
            InGameMenu.Instance.SetOverlay(true, true);
        });

        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        // seq.AppendCallback(StorageMagicRock);
        seq.Play();
    }

    public void StorageMagicRock()
    {
        eventManager.SetCamera(true);
        eventManager.SetPriorityDefaultCamera(0);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(4f);
        sequence.AppendCallback(() =>
        {
            InGameMenu.Instance.SetOverlay(false, false);
            eventManager.SetCamera(false);
            eventManager.SetPriorityVirtualCamera(1);
            InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal);
        });
        sequence.AppendCallback(() => particleSystem_appear.Play());
        sequence.AppendInterval(2f);
        sequence.Append(box_parent.DOLocalMoveY(0f, 2f));
        sequence.AppendCallback(() => particleSystem_appear.Stop());
        sequence.AppendInterval(2f);
        sequence.Append(magicBox.DOLocalMoveZ(2.5f, 2f).SetEase(Ease.OutBack));
        sequence.Append(magicRock.DOLocalMoveY(-0.1f, 1f).SetEase(Ease.Linear));
        sequence.AppendCallback(() => magicRock.SetParent(magicBox));
        sequence.AppendInterval(1f);
        sequence.Append(magicBox.DOLocalMoveZ(-0.08f, 2f).SetEase(Ease.Linear));
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() => puzzleBox_magicField.material = yellow);
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() => { eventManager.RunEvent(FadeDownStory); });
        sequence.Play();
    }

    public void FadeDownStory()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(4f);
        sequence.AppendCallback(() =>
        {
            eventManager.SetCamera(true);
            InGameMenu.Instance.SetOverlay(true, true);
        });
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(4f);
        sequence.Play();
    }

    public void ExtendPuzzleBox1()
    {
        upper.gameObject.SetActive(true);
        decoy.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        Quaternion localRotation = magicField_box.localRotation;
        sequence.AppendInterval(1f);
        sequence.Append(decoy.DOScale(0f, 1f));
        sequence.AppendCallback(() => particle_box.Play());
        sequence.Append(magicField_box.DOScale(1f, 2f));
        sequence.Join(magicField_box.DOLocalRotate(
            new Vector3(localRotation.eulerAngles.x, 90f, localRotation.eulerAngles.z),
            2f));
        sequence.AppendCallback(() => puzzleBox_line.gameObject.SetActive(false));
        sequence.Append(upper.DOScale(1f, 1f));
        sequence.Append(buttons.DOLocalMove(new Vector3(buttons.localPosition.x, 0f, buttons.localPosition.z), 2f));
        sequence.AppendCallback(() => particle_box.Stop());
        sequence.Append(magicField_box.DOScale(0f, 2f));
        sequence.Append(
            magicField.DOLocalRotate(new Vector3(localRotation.eulerAngles.x, 0f, localRotation.eulerAngles.z), 2f));
        sequence.AppendCallback(() => puzzleBox.PuzzleReset());
        sequence.Play().OnComplete(() => puzzleBox.isLoaded = true);
    }

    private List<MonsterSpawnType> tmpMonsterDataList = new List<MonsterSpawnType>();
    Vector3 halfExtents = new Vector3(5f, 5f, 5f);

    private void RandomSetMonster()
    {
        RaycastHit hitInfo;
        float littleOffset = 2f;
        ChildList monsters = monsterSpawnTypes[nowAttackPhaseID];
        tmpMonsterDataList = new List<MonsterSpawnType>(monsters.list);
//各オブジェクトを円状に配置
        foreach (MonsterSpawnType spawnData in monsters.list)
        {
            GameObject monsterPrefab = Marinia.GetMonster(spawnData.data.GetMonsterID());
            for (int j = 0; j < spawnData.count; j++)
            {
                Vector3 childPosition = transform.position;
                bool result = true;
                int counter = 0;

                while (result)
                {
                    float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
                    childPosition.x += spawnRadius * Mathf.Cos(angle);
                    childPosition.z += spawnRadius * Mathf.Sin(angle);

                    Physics.BoxCast(childPosition, halfExtents, Vector3.down, out hitInfo);
                    if (hitInfo.collider != null && hitInfo.collider.CompareTag("Floor"))
                    {
                        childPosition.y = hitInfo.point.y + littleOffset;
                        Instantiate(monsterPrefab, childPosition, Quaternion.identity, entityParent);
                        result = false;
                    }
                    
                    if (counter >= 100)
                    {
                        Debug.LogError("試行回数が多すぎます！");
                        result = false;
                    }
                }
            }
        }
    }

    private void RandomQuickSetMonster(int index)
    {
        RaycastHit hitInfo;
        float littleOffset = 2f;
        QuickSpawnList monsters = quickSpawnTypes[index];
        tmpMonsterDataList = new List<MonsterSpawnType>(monsters.list);
        Debug.Log(tmpMonsterDataList.Count);
//各オブジェクトを円状に配置
        foreach (MonsterSpawnType spawnData in monsters.list)
        {
            GameObject monsterPrefab = Marinia.GetMonster(spawnData.data.GetMonsterID());
            for (int j = 0; j < spawnData.count; j++)
            {
                Vector3 childPosition = transform.position;
                bool result = true;
                int counter = 0;

                while (result)
                {
                    counter++;
                    float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
                    childPosition.x += spawnRadius * Mathf.Cos(angle);
                    childPosition.z += spawnRadius * Mathf.Sin(angle);

                    Physics.BoxCast(childPosition, halfExtents, Vector3.down, out hitInfo);
                    if (hitInfo.collider != null && hitInfo.collider.CompareTag("Floor"))
                    {
                        childPosition.y = hitInfo.point.y + littleOffset;
                        Instantiate(monsterPrefab, childPosition, Quaternion.identity, entityParent);
                        result = false;
                    }

                    if (counter >= 100)
                    {
                        Debug.LogError("試行回数が多すぎます！");
                        result = false;
                    }
                }
            }
        }
    }

    public void BossSpawned()
    {
        CinemachineTrackedDolly cinemachineTrackedDolly =
            bossCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        Vector3 childPosition = bossSpawnPoint.position;
        GameObject monsterPrefab = Marinia.GetMonster(4);
        GameObject instance = Instantiate(monsterPrefab, childPosition, Quaternion.identity, entityParent);


        eventManager.SetCamera(false);
        eventManager.SetPriorityVirtualCamera(2);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(7f);
        sequence.Append(DOTween.To(() => cinemachineTrackedDolly.m_PathPosition,
            x => cinemachineTrackedDolly.m_PathPosition = x, 0f,
            3f));
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Quick));
        sequence.AppendCallback(Ending);
        sequence.Play();
    }

    public void Ending()
    {
    }

    public void MonsterDeathChecker(MonsterData data, int count)
    {
        MonsterSpawnType monsterSpawnType = tmpMonsterDataList.Find(x => x.data == data);
        monsterSpawnType.count -= 1;

        int index = tmpMonsterDataList.FindIndex(x => x.data == data);
        if (monsterSpawnType.count == 0)
        {
            tmpMonsterDataList.RemoveAt(index);
        }
        else
        {
            tmpMonsterDataList[index] = monsterSpawnType;
        }

        if (tmpMonsterDataList.Count == 0)
        {
            isFighting = false;
            if (nowAttackPhaseID == 0) environmentManager.SetFlowerUsable(true);

            if (!isQuick)
            {
                FlagCore.Instance.SetEpisodeFlag(monsterSpawnTypes[nowAttackPhaseID].eventID,
                    monsterSpawnTypes[nowAttackPhaseID].phaseFlagType, true);

                nowAttackPhaseID++;
            }

            if (FlagCore.Instance.CheckCanExecute())
                InGameMenu.Instance.SetOverlay(true, true);
            // FlagCore.Instance.CheckForceExecute();
        }
    }
}