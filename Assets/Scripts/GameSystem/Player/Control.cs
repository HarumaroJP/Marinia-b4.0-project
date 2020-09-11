using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class Control : MonoBehaviour
{
    [SerializeField] public Vector3 localGravity; //プレイヤー単体にかける重力（落下速度調整用）
    [SerializeField] public Vector2 MinAndMaxPolerAngle;
    [SerializeField] public Vector2 keyInputs; //キー入力時の単位ベクトルの取得
    [SerializeField] public Vector2 mouseInputs; //マウス入力時の単位ベクトルの取得
    [SerializeField] public Vector2 RotXY;
    [SerializeField] public int JumpPower; //ジャンプ力
    [SerializeField] public float moveSpeed; //移動速度
    [SerializeField] public float TimerLimit; //ジャンプタイマーリミット
    [SerializeField] public bool isJumping; //ジャンプ判定
    [SerializeField] public bool isPlaying = true;
    [SerializeField] private bool isSprinting;
    [SerializeField] public float rayDistance; //Rayの長さ
    [SerializeField] public float sensi;
    [SerializeField] private float walkSpeedLimit = 7f;
    [SerializeField] private float sprintSpeedLimit = 10f;

    Rigidbody rb;
    RaycastHit hitObj;
    Vector3 cameraForward;
    Vector3 cameraRight;
    Vector3 moveVector;
    private Camera cam;
    private float defaultFOV;

    void Start()
    {
        cam = Camera.main;
        defaultFOV = cam.fieldOfView;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isPlaying) return;
        keyInputs.x = Input.GetAxisRaw("Horizontal");
        keyInputs.y = Input.GetAxisRaw("Vertical");
        mouseInputs.x = Input.GetAxisRaw("Mouse X");
        mouseInputs.y = Input.GetAxisRaw("Mouse Y");


        JumpChecker(); //RayCast
    }

    private void LateUpdate()
    {
        if (!isPlaying) return;
        UpdateAngle(mouseInputs.x, mouseInputs.y);
    }

    void FixedUpdate()
    {
        VarCheck();
        MoveChecker(); //ジャンプ時の移動メソッド
        rb.AddForce(localGravity, ForceMode.Acceleration); //人工重力
    }

    void VarCheck()
    {
        cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized; //カメラの単位ベクトル
        cameraRight = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1)).normalized;


        if (!isJumping)
        {
            moveVector = cameraForward * keyInputs.y + cameraRight * keyInputs.x; //プレイヤーの移動方向
            // Debug.Log(moveVector);

            if (keyInputs == Vector2.zero) //移動速度の初期化
                moveSpeed = 1f;
            else
            {
                moveSpeed += Time.deltaTime * 20f;
                moveSpeed = Mathf.Clamp(moveSpeed, 0f, Input.GetKey(KeyCode.LeftShift) ? 10f : 7f);
            }

            if (keyInputs.y <= 0f)
            {
                cam.DOFieldOfView(defaultFOV, 0.5f).Play().SetEase(Ease.OutBack);
                isSprinting = false;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && keyInputs.y > 0f)
            {
                cam.DOFieldOfView(defaultFOV + 10f, 0.5f).Play().SetEase(Ease.OutBack);
                isSprinting = true;
            }
        }
    }
    
    void MoveChecker()
    {
        if (moveVector != Vector3.zero)
        {
            rb.velocity = moveVector * moveSpeed + new Vector3(0, rb.velocity.y, 0);
        }
    }

    void UpdateAngle(float x, float y)
    {
        RotXY.x += x * sensi * Time.deltaTime;
        RotXY.y += y * sensi * Time.deltaTime;
        RotXY.y = Mathf.Clamp(RotXY.y, MinAndMaxPolerAngle.x, MinAndMaxPolerAngle.y);
        cam.transform.localEulerAngles = new Vector3(-RotXY.y, RotXY.x, 0);

        // x = azimuthalAngle - x * mouseXSensitivity;
        // azimuthalAngle = Mathf.Repeat(x, 360);
        //
        // y = polarAngle + y * mouseYSensitivity;
        // polarAngle = Mathf.Clamp(y, minPolarAngle, maxPolarAngle);
    }

    float jumpingTimer = 0f; //ジャンプタイマー（計測用）

    void JumpChecker()
    {
        Ray JumpRay = new Ray(transform.position, Vector3.down);
        Debug.DrawLine(JumpRay.origin, JumpRay.origin + JumpRay.direction * rayDistance, Color.red);

        if (jumpingTimer > TimerLimit)
        {
            jumpingTimer = 0f;
            moveSpeed /= 1.5f;
            isJumping = false;
        }
        else if (isJumping)
            jumpingTimer += Time.deltaTime;

        if (Physics.Raycast(JumpRay, out hitObj, rayDistance))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = moveVector * moveSpeed + new Vector3(0, JumpPower, 0);
                isJumping = true;
            }
        }
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None; //カーソルをアンロック 
        Cursor.visible = true; //カーソルを表示 
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked; //カーソルをロック 
        Cursor.visible = false; //カーソルを非表示 
    }
}