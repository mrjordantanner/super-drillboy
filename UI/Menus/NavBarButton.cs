using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class NavBarButton : MonoBehaviour
{
    public MenuPanel PanelToOpen;
    public Button button;
    public Image icon;
    public TextMeshProUGUI text;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        NavBar.Instance.ActiveMenuPanel.Hide();
        NavBar.Instance.ActiveMenuPanel = PanelToOpen;
        PanelToOpen.Show();

        // TODO also close other associated panels, e.g. Leaderboard, or Settings menus
    }
}
