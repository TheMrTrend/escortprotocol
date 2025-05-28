using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Menus")]
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject winMenu;
    [SerializeField] public GameObject loseMenu;

    [Header("Ammo Displays")]
    [SerializeField] public AmmoDisplay ammoDisplay_Pistol;
    [SerializeField] public AmmoDisplay ammoDisplay_Rifle;
    [SerializeField] public GameObject knifeDisplay;

    [Header("Crosshairs")]
    [SerializeField] public Image crosshairPistol;
    [SerializeField] public Image crosshairRifle;
    [SerializeField] public Image crosshairKnife;


    [Header("Screen Effects")]
    [SerializeField] public Image damageFlash;
    [SerializeField] public RawImage toxicFlash;

    [Header("Interaction Prompts")]
    public TMP_Text textPopupDescription;
    public GameObject textPopup;

    [Header("Dialogue Settings")]
    [SerializeField]public  Image dialogueFrame;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public Image dialogueSpeakerIcon;
    [SerializeField] public ParticleSystem dialogueFrameParticles;
    [SerializeField] public ParticleSystem dialogueSpeakerIconParticles;
    [SerializeField] public TextMeshProUGUI dialoguePromptText;


    public void ShowKnifeDisplay()
    {
        if (knifeDisplay != null) knifeDisplay.SetActive(true);
        if (ammoDisplay_Pistol != null) ammoDisplay_Pistol.gameObject.SetActive(false);
        if (ammoDisplay_Rifle != null) ammoDisplay_Rifle.gameObject.SetActive(false);
    }

    public void ShowPistolDisplay()
    {
        if (knifeDisplay != null) knifeDisplay.SetActive(false);
        if (ammoDisplay_Pistol != null) ammoDisplay_Pistol.gameObject.SetActive(true);
        if (ammoDisplay_Rifle != null) ammoDisplay_Rifle.gameObject.SetActive(false);
    }

    public void ShowRifleDisplay()
    {
        if (knifeDisplay != null) knifeDisplay.SetActive(false);
        if (ammoDisplay_Pistol != null) ammoDisplay_Pistol.gameObject.SetActive(false);
        if (ammoDisplay_Rifle != null) ammoDisplay_Rifle.gameObject.SetActive(true);
    }



    private void Awake()
    {
        instance = this;
    }
}
