using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField]public  Image dialogueFrame;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public Image dialogueSpeakerIcon;
    [SerializeField] public ParticleSystem dialogueFrameParticles;
    [SerializeField] public ParticleSystem dialogueSpeakerIconParticles;
    [SerializeField] public TextMeshProUGUI dialoguePromptText;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject winMenu;
    [SerializeField] public GameObject loseMenu;
    [SerializeField] public AmmoDisplay ammoDisplay;
    [SerializeField] public Crosshair crosshair;
    private void Awake()
    {
        instance = this;
    }
}
