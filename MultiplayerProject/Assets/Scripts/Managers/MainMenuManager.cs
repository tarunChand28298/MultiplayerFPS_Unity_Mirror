using UnityEngine;

enum MenuPage
{
    empty = -1,
    play,
    settings,
    help,
    about,
    quit
}

public class MainMenuManager : MonoBehaviour
{
    public AudioSource clickSound;

    private float tweeningTime = 0.1f;

    public GameObject container;
    public GameObject[] menus;

    MenuPage currentPage = MenuPage.empty;
    GameObject currentMenu = null;

    public void PlayButtonPressed()
    {
        HandleMenubuttonClicked(MenuPage.play);
    }
    public void SettingsButtonPressed()
    {
        HandleMenubuttonClicked(MenuPage.settings);
    }
    public void HelpButtonPressed()
    {
        HandleMenubuttonClicked(MenuPage.help);
    }
    public void AboutButtonPressed()
    {
        HandleMenubuttonClicked(MenuPage.about);
    }
    public void QuitButtonPressed()
    {
        HandleMenubuttonClicked(MenuPage.quit);
    }

    private void HandleMenubuttonClicked(MenuPage buttonClicked)
    {
        clickSound.Play();
        if(currentMenu == null)
        {
            TweenInNewPage(buttonClicked);
        }
        else
        {
            if(currentPage == buttonClicked)
            {
                TweenOutCurrentPage();
            }
            else
            {
                TweenOutCurrentPage();
                TweenInNewPage(buttonClicked);
            }
        }
    }
    private LTDescr TweenOutCurrentPage()
    {
        currentPage = MenuPage.empty;
        return LeanTween.scaleX(currentMenu, 0.0f, tweeningTime).setOnComplete(() => { Destroy(currentMenu); });
    }
    private void TweenInNewPage(MenuPage page)
    {
        currentPage = page;
        GameObject newMenu = Instantiate(menus[(int)page], container.transform);
        newMenu.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

        LeanTween.scaleY(newMenu, 1.0f, tweeningTime).setDelay(tweeningTime).setOnComplete(() => { currentMenu = newMenu; });
    }
}
