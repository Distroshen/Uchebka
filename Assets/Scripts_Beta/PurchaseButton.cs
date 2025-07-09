using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int price = 100;
    [SerializeField] private string itemID = "music_track_1";
    [SerializeField] private AudioClip newTrack;

    [Header("References")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private AudioSource musicPlayer;

    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color purchasedColor = Color.green;
    [SerializeField] private Color activeColor = Color.blue;

    [Header("Texts")]
    [SerializeField] private string purchasedText = "PLAY";
    [SerializeField] private string activeText = "PLAYING";

    [SerializeField] private AudioSource audioSource;
    private Button button;
    private CoinManager coins;
    private bool isPurchased = false;
    private bool isActiveTrack = false;
    private static PurchaseButton currentlyPlaying;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        button = GetComponent<Button>();
        coins = FindObjectOfType<CoinManager>();

        LoadPurchaseState();
        InitializeButton();
        UpdateButtonState();
    }

    private void InitializeButton()
    {
        button.onClick.RemoveAllListeners();

        if (isPurchased)
        {
            button.onClick.AddListener(PlayMusic);
        }
        else
        {
            button.onClick.AddListener(OnPurchase);
        }
    }

    private void LoadPurchaseState()
    {
        isPurchased = PlayerPrefs.GetInt(itemID, 0) == 1;
    }

    private void SavePurchaseState()
    {
        PlayerPrefs.SetInt(itemID, 1);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if (!isPurchased && coins != null)
        {
            bool canAfford = coins.CurrentCoins >= price;
            buttonImage.color = canAfford ? availableColor : lockedColor;
            button.interactable = canAfford;
        }
    }

    public void UpdateButtonState()
    {
        if (isActiveTrack)
        {
            buttonImage.color = activeColor;
            priceText.text = activeText;
            button.interactable = true;
            return;
        }

        if (isPurchased)
        {
            buttonImage.color = purchasedColor;
            priceText.text = purchasedText;
            button.interactable = true;
            return;
        }

        if (coins != null)
        {
            bool canAfford = coins.CurrentCoins >= price;
            buttonImage.color = canAfford ? availableColor : lockedColor;
            button.interactable = canAfford;
            priceText.text = price.ToString();
        }
    }

    public void OnPurchase()
    {
        if (isPurchased || coins == null) return;

        if (coins.CurrentCoins >= price)
        {
            coins.SpendCoins(price);
            isPurchased = true;
            SavePurchaseState();
            InitializeButton();
            PlayMusic();
            Debug.Log($"Purchased: {itemID}");
        }
    }

    private void PlayMusic()
    {
       // if (!isPurchased || musicPlayer == null) return;

        // Останавливаем предыдущий трек
        if (currentlyPlaying != null && currentlyPlaying != this)
        {
            currentlyPlaying.StopMusic();
        }

        switch (gameObject.tag)
        {
            case "Song":
                // Для тега Song - воспроизводим музыку
                MusicController.Instance.ChangeMusic();
                break;

            case "Song1":
                // Для тега Song - воспроизводим музыку
                MusicController.Instance.ChangeMusic1();
                break;
            case "Song2":
                // Для тега Song - воспроизводим музыку
                MusicController.Instance.ChangeMusic2();
                break;
        }
        UpdateButtonState();
    }

    private void StopMusic()
    {
        isActiveTrack = false;
        UpdateButtonState();
    }
}