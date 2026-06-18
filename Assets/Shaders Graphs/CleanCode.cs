using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;

public class CleanCode : MonoBehaviour {

    [SerializeField] private Texture2D dirtMaskTextureBase;
    [SerializeField] private Texture2D noMask;
    [SerializeField] private Texture2D dirtBrush;
    [SerializeField] private Material material;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float amountToClean = 0.87f;

    private Texture2D dirtMaskTexture;
    private float dirtAmountTotal;
    private float dirtAmount;

    private bool isOnCamera;
    private bool isDirt;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public bool _isOnCamaera{ get { return isOnCamera; } set { isOnCamera = value; } }
    public bool _isDirt => isDirt;
    public CinemachineVirtualCamera _virtualCamera => virtualCamera;

    private void Awake() {
        material = GetComponent<MeshRenderer>().material;
        dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        dirtMaskTexture.Apply();
        material.SetTexture("_DirtMask", dirtMaskTexture);

        dirtAmountTotal = 0f;
        for (int x = 0; x < dirtMaskTextureBase.width; x++)
        {
            for (int y = 0; y < dirtMaskTextureBase.height; y++)
            {
                dirtAmountTotal += dirtMaskTextureBase.GetPixel(x, y).g;
            }
        }
        dirtAmount = dirtAmountTotal;

        isOnCamera = false;
        CleanTable();
    }

    private void Update() {
        if (Mouse.current.leftButton.isPressed && isOnCamera == true) {

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.value), out RaycastHit raycastHit)) {
                Vector2 textureCoord = raycastHit.textureCoord;

                int pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
                int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);

                Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);

                int pixelXOffset = pixelX - (dirtBrush.width / 2);
                int pixelYOffset = pixelY - (dirtBrush.height / 2);

                for (int x = 0; x < dirtBrush.width; x++) {
                    for (int y = 0; y < dirtBrush.height; y++) {
                        Color pixelDirt = dirtBrush.GetPixel(x, y);
                        Color pixelDirtMask = dirtMaskTexture.GetPixel(pixelXOffset + x, pixelYOffset + y);

                        float removedAmount = pixelDirtMask.g - (pixelDirtMask.g * pixelDirt.g);
                        dirtAmount -= removedAmount;

                        dirtMaskTexture.SetPixel(
                            pixelXOffset + x, 
                            pixelYOffset + y, 
                            new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                        );
                    }
                }

                dirtMaskTexture.Apply();

                GetDirtAmount();
            }
        }
    }

    public void GetDirtAmount() {
        if (this.dirtAmount / dirtAmountTotal < 0.9f)
        {
            CleanTable();
            virtualCamera.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResetPaint()
    {
        dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        dirtMaskTexture.Apply();
        material.SetTexture("_DirtMask", dirtMaskTexture);
        isDirt = true;
    }

    public void CleanTable()
    {
        dirtMaskTexture = new Texture2D(noMask.width, noMask.height);
        dirtMaskTexture.SetPixels(noMask.GetPixels());
        dirtMaskTexture.Apply();
        material.SetTexture("_DirtMask", dirtMaskTexture);
        isDirt = false;
        isOnCamera = false;
    }

    public void OnCamera()
    {
        isOnCamera = true;
        virtualCamera.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
