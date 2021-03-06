using UnityEngine;
using System.Collections;

public class GuiButtons : MonoBehaviour {

    private bool ShowPlayerStats=false;

    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 120, 100), LocalizationText.GetText("lblLanguage"));
        if (ShowPlayerStats)
        {
            GUI.Box(new Rect(10, 300, 300, 600), LocalizationText.GetText("lblPlayerStats"));

            //Text Label of Attributes
            GUI.Label(new Rect(20, 320, 130, 20), LocalizationText.GetText("lblStrength"));
            GUI.Label(new Rect(20, 340, 130, 20), LocalizationText.GetText("lblLife"));
            GUI.Label(new Rect(20, 360, 130, 20), LocalizationText.GetText("lblEndurance"));
            GUI.Label(new Rect(20, 380, 130, 20), LocalizationText.GetText("lblWisdom"));
            GUI.Label(new Rect(20, 400, 130, 20), LocalizationText.GetText("lblIntelligence"));
            GUI.Label(new Rect(20, 420, 130, 20), LocalizationText.GetText("lblWeight"));
            GUI.Label(new Rect(20, 440, 130, 20), LocalizationText.GetText("lblHeight"));
            GUI.Label(new Rect(20, 460, 130, 20), LocalizationText.GetText("lblOld"));
            GUI.Label(new Rect(20, 480, 130, 20), LocalizationText.GetText("lblWilderness"));
            GUI.Label(new Rect(20, 500, 130, 20), LocalizationText.GetText("lblStreet"));
            GUI.Label(new Rect(20, 520, 130, 20), LocalizationText.GetText("lblFood"));
            GUI.Label(new Rect(20, 540, 130, 20), LocalizationText.GetText("lblThirst"));
            GUI.Label(new Rect(20, 560, 130, 20), LocalizationText.GetText("lblLvl"));
            GUI.Label(new Rect(20, 580, 130, 20), LocalizationText.GetText("lblSpellpower"));
            GUI.Label(new Rect(20, 600, 130, 20), LocalizationText.GetText("lblRunspeed"));
            GUI.Label(new Rect(20, 620, 130, 20), LocalizationText.GetText("lblCountry"));
            GUI.Label(new Rect(20, 640, 130, 20), LocalizationText.GetText("lblFriends"));
            GUI.Label(new Rect(20, 660, 130, 20), LocalizationText.GetText("lblEnemies"));
            GUI.Label(new Rect(20, 680, 130, 20), LocalizationText.GetText("lblMoney"));
            GUI.Label(new Rect(20, 700, 130, 20), LocalizationText.GetText("lblEarnings"));
            GUI.Label(new Rect(20, 720, 130, 20), LocalizationText.GetText("lblName"));
            GUI.Label(new Rect(20, 740, 130, 20), LocalizationText.GetText("lblSurName"));
            GUI.Label(new Rect(20, 760, 130, 20), LocalizationText.GetText("lblBorn"));


            //Attributes
            GUI.Label(new Rect(200, 320, 120, 20), "110");
            GUI.Label(new Rect(200, 340, 120, 20), "52");
            GUI.Label(new Rect(200, 360, 120, 20), "40");
            GUI.Label(new Rect(200, 380, 120, 20), "60");
            GUI.Label(new Rect(200, 400, 120, 20), "80");
            GUI.Label(new Rect(200, 420, 120, 20), "100");
            GUI.Label(new Rect(200, 440, 120, 20), "200");
            GUI.Label(new Rect(200, 460, 120, 20), "500");
            GUI.Label(new Rect(200, 480, 120, 20), "800");
            GUI.Label(new Rect(200, 500, 120, 20), "20");
            GUI.Label(new Rect(200, 520, 120, 20), "12");
            GUI.Label(new Rect(200, 540, 120, 20), "12");
            GUI.Label(new Rect(200, 560, 120, 20), "12");
            GUI.Label(new Rect(200, 580, 120, 20), "12");
            GUI.Label(new Rect(200, 600, 120, 20), "12");
            GUI.Label(new Rect(200, 620, 120, 20), LocalizationText.GetText("Country"));
            GUI.Label(new Rect(200, 640, 120, 20), "12");
            GUI.Label(new Rect(200, 660, 120, 20), "12");
            GUI.Label(new Rect(200, 680, 120, 20), "12");
            GUI.Label(new Rect(200, 700, 120, 20), "12");
            GUI.Label(new Rect(200, 720, 120, 20), LocalizationText.GetText("Name"));
            GUI.Label(new Rect(200, 740, 120, 20), LocalizationText.GetText("SurName"));
            GUI.Label(new Rect(200, 760, 120, 20), LocalizationText.GetText("BornCity"));

            //text of the Character		
            GUI.TextArea(new Rect(20, 780, 280, 110), LocalizationText.GetText("PlayerText"));
        }
        //Show PlayerStats
        if (GUI.Button(new Rect(10, 280, 100, 20), LocalizationText.GetText("lblPlayerStats")))
            ShowPlayerStats = !ShowPlayerStats;

       
        if (GUI.Button(new Rect(30, 40, 80, 20), LocalizationText.GetText("btnEnglish")))
        {
            LocalizationText.SetLanguage("EN");
        }        
        if (GUI.Button(new Rect(30, 70, 80, 20), LocalizationText.GetText("btnGerman")))
        {
            LocalizationText.SetLanguage("DE");
        }
    }
}