using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public struct PageData
    {
        public int pageIndex;
        public string recipeTitle;
        public VideoPlayer videoPlayer;
        public GameObject rightPageContent;
    }

    [Header("UI Components")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private TMP_Text recipeTitleText;
    [SerializeField] private RawImage videoRawImageDisplay;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    [Header("Player Components")]
    [SerializeField] private Interaction playerInteraction;
    [SerializeField] private DealingController playerDealing;

    [Header("Book Content")]
    [SerializeField] private int totalPages = 5;
    [SerializeField] private List<PageData> recipePages;

    private int currentPageIndex = 0;
    private bool isBookOpen = false;

    [Header("Sounds book")]
    [SerializeField] private AudioClip openBookSound;
    [SerializeField] private AudioClip closeBookSound;
    [SerializeField] private AudioClip passPage;
    private SoundManager soundManager;

    private void Start()
    {
        bookPanel.SetActive(false);
        isBookOpen = false;

        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (backButton != null) backButton.onClick.AddListener(PreviousPage);

        soundManager = GameManager.instance._soundManager;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isBookOpen)
            {
                CloseBook();
            }
            else
            {
                if (Cursor.visible) return;
                OpenBook();
            }
        }
    }

    private void OpenBook()
    {
        soundManager.ReproduceSound(openBookSound);

        isBookOpen = true;
        bookPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerInteraction != null) playerInteraction.enabled = false;
        if (playerDealing != null) playerDealing.enabled = false;

        UpdatePageVisuals();
    }

    private void CloseBook()
    {
        soundManager.ReproduceSound(closeBookSound);

        isBookOpen = false;
        bookPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerInteraction != null) playerInteraction.enabled = true;
        if (playerDealing != null) playerDealing.enabled = true;

        StopAllVideos();
    }

    public void NextPage()
    {
        if (currentPageIndex < totalPages - 1)
        {
            soundManager.ReproduceSound(passPage);
            currentPageIndex++;
            UpdatePageVisuals();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            soundManager.ReproduceSound(passPage);
            currentPageIndex--;
            UpdatePageVisuals();
        }
    }

    private void UpdatePageVisuals()
    {
        if (backButton != null) backButton.gameObject.SetActive(currentPageIndex > 0);
        if (nextButton != null) nextButton.gameObject.SetActive(currentPageIndex < totalPages - 1);

        StopAllVideos();
        DisableAllContent();

        if (videoRawImageDisplay == null) return;

        bool pageFound = false;

        foreach (var page in recipePages)
        {
            if (page.pageIndex == currentPageIndex)
            {
                if (recipeTitleText != null) recipeTitleText.text = page.recipeTitle;

                if (page.videoPlayer != null)
                {
                    videoRawImageDisplay.gameObject.SetActive(true);
                    videoRawImageDisplay.texture = page.videoPlayer.targetTexture;
                    page.videoPlayer.Play();
                }
                else
                {
                    videoRawImageDisplay.gameObject.SetActive(false);
                }

                if (page.rightPageContent != null)
                {
                    page.rightPageContent.SetActive(true);
                }

                pageFound = true;
                break;
            }
        }

        if (!pageFound)
        {
            videoRawImageDisplay.gameObject.SetActive(false);
            if (recipeTitleText != null) recipeTitleText.text = "no recipe";
        }
    }

    private void DisableAllContent()
    {
        foreach (var page in recipePages)
        {
            if (page.rightPageContent != null)
            {
                page.rightPageContent.SetActive(false);
            }
        }
    }

    private void StopAllVideos()
    {
        foreach (var page in recipePages)
        {
            if (page.videoPlayer != null)
            {
                page.videoPlayer.Stop();
            }
        }
    }
}