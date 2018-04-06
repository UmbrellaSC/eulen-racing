using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Enhält globale Statusvariablen
/// </summary>
public class GlobalState : MonoBehaviour
{
    /// <summary>
    /// Die momentan aktive Instanz der Klasse
    /// </summary>
    public static GlobalState Instance;
    
    /// <summary>
    /// Ist das Spiel pausiert?
    /// </summary>
    public static Boolean Paused;

    /// <summary>
    /// Ob ein oder zwei Spieler fahren
    /// </summary>
    public static Boolean Singleplayer = true;

    /// <summary>
    /// Die Autofarbe des ersten Spielers
    /// </summary>
    public static Color PlayerAColor;

    /// <summary>
    /// Die Autofarbe des zweiten Spielers
    /// </summary>
    public static Color PlayerBColor;

    /// <summary>
    /// Der Seed mit dem das Level generiert wird
    /// </summary> 
    public static String Seed;

    /// <summary>
    /// Der Text der auf dem Pause-Button angezeigt wird
    /// </summary>
    public Text pauseText;

    /// <summary>
    /// Die Komponenten der UI die angezeigt werden, wenn die "1 Spieler" Option ausgewählt ist
    /// </summary>
    public RectTransform singleplayerUI;

    /// <summary>
    /// Die Komponenten der UI die angezeigt werden, wenn die "2 Spieler" Option ausgewählt ist
    /// </summary>
    public RectTransform multiplayerUI;

    /// <summary>
    /// Das Auswahlfeld für die Farbe des Spielers im Singleplayer
    /// </summary>
    public ColorSelector SingleplayerColor;

    /// <summary>
    /// Das Auswahlfeld für die Farbe des ersten Spielers im Multiplayer
    /// </summary>
    public ColorSelector MultiplayerAColor;

    /// <summary>
    /// Das Auswahlfeld für die Farbe des zweiten Spielers im Multiplayer
    /// </summary>
    public ColorSelector MultiplayerBColor;

    /// <summary>
    /// Das Eingabefeld für den Seed;
    /// </summary>
    public InputField SeedInput;

    /// <summary>
    /// Verhindert dass mehr als eine Instanz der Klasse aktiv ist
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        
        // Spieleranzahl auf 1 setzen
        Singleplayer = true;
    }

    /// <summary>
    /// Die momentan ausgewählte Farbe wird jeden Frame ausgelesen und zwischengespeichert
    /// </summary>
    void Update()
    {
        if (Singleplayer)
        {
            if (SingleplayerColor != null)
            {
                PlayerAColor = SingleplayerColor.SelectedColor;
            }
        }
        else
        {
            if (MultiplayerAColor != null)
            {
                PlayerAColor = MultiplayerAColor.SelectedColor;
            }

            if (MultiplayerBColor != null)
            {
                PlayerBColor = MultiplayerBColor.SelectedColor;
            }
        }

        if (SeedInput != null)
        {
            Seed = SeedInput.text;
        }
    }
    
    /// <summary>
    /// Pausiert das Spiel
    /// </summary>
    public void Pause()
    {
        Paused = !Paused;
        pauseText.text = Paused ? "Weiter" : "Pause";
    }

    /// <summary>
    /// Startet das Rennen durch neu laden der momentanen Szene neu
    /// </summary>
    public void Restart()
    {
        // Pausierung aufheben
        Paused = false;
        
        if (Singleplayer)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Lädt das Hauptmenü
    /// </summary>
    public void Menu()
    {
        // Pausierung aufheben
        Paused = false;
        
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    /// <summary>
    /// Wird aufgerufen wenn man im Hauptmenü die Option "1 Spieler" auswählt
    /// </summary>
    public void SingleplayerClicked()
    {
        Singleplayer = true;
        multiplayerUI.gameObject.SetActive(false);
        singleplayerUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// Wird aufgerufen wenn man im Hauptmenü die Option "2 Spieler" auswählt
    /// </summary>
    public void MultiplayerClicked()
    {
        Singleplayer = false;
        singleplayerUI.gameObject.SetActive(false);
        multiplayerUI.gameObject.SetActive(true);
    }
}