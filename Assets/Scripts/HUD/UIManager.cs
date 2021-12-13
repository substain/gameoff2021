using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MenuController;

/// <summary>
/// Super class for HUD Manager
/// </summary>
public class UIManager : MonoBehaviour
{
    protected const int N_UI_AUDIO_SOURCES = 2;

    public enum MenuSoundType
    {
        focusItem, useFocused, back, openMenu, closeMenu
    }

    public static UIManager Instance = null;

    private AudioSource menuSoundAudioSource;
    private AudioSource menuMusicAudioSource;

    [SerializeField]
    private AudioClip focusItemClip;

    [SerializeField]
    private AudioClip useFocusedClip;

    [SerializeField]
    private AudioClip backNavigationClip;

    [SerializeField]
    private AudioClip openMenuClip;

    [SerializeField]
    private AudioClip closeMenuClip;

    [SerializeField]
    private AudioClip uiBackgroundMusic;

    //private MenuController activeMenu = null;
    private Dictionary<MenuType, MenuController> controlledMenus;
    private MenuType? activeMenu;

    protected virtual void Awake()
    { 
        SetInstance();
        controlledMenus = GetComponentsInChildren<MenuController>(includeInactive: true).ToDictionary(mc => mc.GetMenuType());
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogWarning("Warning: less than 2 audio sources were found on " + this.GetType().ToString() + "'s GameObject.");
        }
        menuMusicAudioSource = audioSources[0];
        menuMusicAudioSource.clip = uiBackgroundMusic;
        menuSoundAudioSource = audioSources[1];
    }

    private void SetInstance()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
        }
        Instance = this;
    }

    void Start()
    {
        KeyValuePair<MenuType, MenuController> menuPair = controlledMenus.Where(mc => mc.Value.IsEnabledOnStart()).FirstOrDefault();
        if(menuPair.Equals(default(KeyValuePair<MenuType, MenuController>))){
            SetMenuActive(null, isStart: true);
        }
        else
        {
            SetMenuActive(menuPair.Key, isStart:true);
        }
    }

    public void SetMenuActive(MenuType? newActiveMenu, MenuType? parentMenu = null, GameManager.GameOverReason? gameOverReason = null, bool isStart = false)
    {
        bool oldMenuHasValue = activeMenu.HasValue;
        foreach (KeyValuePair<MenuType, MenuController> menuPair in controlledMenus)
        {
            if(newActiveMenu.HasValue && menuPair.Key == newActiveMenu.Value)
            {
                PlayerMenuInput.Instance.SetEnabled(true);
                MenuController mc = menuPair.Value;
                //mc.SetEnabled(true);
                mc.gameObject.SetActive(true);
                mc.SetTitle(GetRandomMenuTitleByType(menuPair.Key, gameOverReason));
                
                if (parentMenu.HasValue)
                {
                    mc.SetParent(parentMenu.Value);
                }
            }
            else
            {
                menuPair.Value.gameObject.SetActive(false);
                //menuPair.Value.SetEnabled(false);
            }
        }
        if (!newActiveMenu.HasValue && oldMenuHasValue)
        {
            PlayerMenuInput.Instance.SetEnabled(false);
            menuMusicAudioSource.Pause();
            if (!isStart)
            {
                PlayMenuSound(MenuSoundType.closeMenu);
            }
        }
        if(newActiveMenu.HasValue && !oldMenuHasValue)
        {
            menuMusicAudioSource.Play();
            if (!isStart)
            {
                PlayMenuSound(MenuSoundType.openMenu);
            }
        }
        activeMenu = newActiveMenu;
    }

    public void UseBackInMenu()
    {
        if (activeMenu == null)
        {
            return;
        }
        PlayMenuSound(MenuSoundType.back);

        controlledMenus[activeMenu.Value].UseBack();
    }

    public void UseFocusedInMenu()
    {
        if (activeMenu == null)
        {
            return;
        }
        PlayMenuSound(controlledMenus[activeMenu.Value].GetFocusedSoundType());
        controlledMenus[activeMenu.Value].UseFocusedItem();
    }

    public void NavigateDirectional(Util.Dir4 dir)
    {
        if (activeMenu == null)
        {
            return;
        }
        controlledMenus[activeMenu.Value].NavigateDirectional(dir);
    }

    protected void PlayMenuSound(MenuSoundType menuSoundType)
    {
        switch (menuSoundType)
        {
            case MenuSoundType.focusItem:
                {
                    menuSoundAudioSource.PlayOneShot(focusItemClip);
                    break;
                }
            case MenuSoundType.useFocused:
                {
                    menuSoundAudioSource.PlayOneShot(useFocusedClip);

                    break;
                }
            case MenuSoundType.back:
                {
                    menuSoundAudioSource.PlayOneShot(backNavigationClip);

                    break;
                }
            case MenuSoundType.openMenu:
                {
                    menuSoundAudioSource.PlayOneShot(openMenuClip);

                    break;
                }
            case MenuSoundType.closeMenu:
                {
                    menuSoundAudioSource.PlayOneShot(closeMenuClip);

                    break;
                }
        }
    }

    private static readonly List<string> pauseMenuTitles = new List<string>() {
        "game paused.",
        "you needed a break.",
        "time to sleep, agent.",
        "you are a great listener.",
        "time to get some cheese sticks.",
        "keep it up, agent.",
        "we'll wait here."
    };

    private static readonly List<string> creditTitles = new List<string>() {
        "credits",
        "agency staff",
        "retired agents",
        "attributions",
        "a random list of great people"
    };


    public static string GetRandomMenuTitleByType(MenuType type, GameManager.GameOverReason? gor = null)
    {
        switch (type)
        {
            case MenuType.pause:
                {
                    return Util.ChooseRandomFromList(pauseMenuTitles);
                }
            case MenuType.mainMenu:
                {
                    return MainManager.GAME_NAME;
                }
            case MenuType.options:
                {
                    return "Options";
                }
            case MenuType.gameover:
                {
                    return gor.HasValue ? GameOverReasonToString(gor.Value) : "Game Over";
                }
            case MenuType.credits:
                {
                    return Util.ChooseRandomFromList(creditTitles);
                }
        }
        return "Menu";
    }

    public static string GameOverReasonToString(GameManager.GameOverReason gor)
    {
        switch (gor)
        {
            case GameManager.GameOverReason.CoverBlown:
                {
                    return Util.ChooseRandomFromList(coverBlownTitles);
                }
            case GameManager.GameOverReason.WentSwimming:
                {
                    return Util.ChooseRandomFromList(wentSwimmingTitles);
                }
            case GameManager.GameOverReason.TooMuchCheese:
                {
                    return Util.ChooseRandomFromList(tooMuchCheeseTitles);
                }
        }
        return "you failed.";
    }

    private static readonly List<string> coverBlownTitles = new List<string>() {
        "your cover was blown!",
        "you were uncovered!",
        "you failed!",
        "they found you!",
        "you have nowhere to hide!",
        "you were detected!"
    };

    private static readonly List<string> wentSwimmingTitles = new List<string>() {
        "swimming is not recommended!",
        "bad weather for a dive!",
        "you failed!",
        "the sharks were waiting!",
        "don't dare to spy on the ocean!",
        "you forgot your swimsuit!",
        "the water is polluted, you fool!",
        "there is no hope for humanity..."
    };

    private static readonly List<string> tooMuchCheeseTitles = new List<string>() {
        "you were trapped like a mouse!",
        "you left too many cheese trails!",
        "cheese is bad for your health!",
        "you found the cheesy end!",
        "you don't know when to stop!"
    };

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}