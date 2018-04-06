using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wandelt die Positionen von drei Slidern in eine RGB Farbe um
/// </summary>
public class ColorSelector : MonoBehaviour
{
	/// <summary>
	/// Die Farbe die momentan ausgewählt ist
	/// </summary>
	public Color SelectedColor;

	/// <summary>
	/// Das Element in dem die Farbe dargestellt wird
	/// </summary>
	public Image Display;

	/// <summary>
	/// Der Slider mit dem der Rotkanal geändert wird
	/// </summary>
	public Slider Red;

	/// <summary>
	/// Der Slider mit dem der Grünkanal geändert wird
	/// </summary>
	public Slider Green;

	/// <summary>
	/// Der Slider mit dem der Blaukanal geändert wird
	/// </summary>
	public Slider Blue;
	
	// Update is called once per frame
	void Update ()
	{
		Display.color = SelectedColor = new Color(Red.value, Green.value, Blue.value, 1f);
	}
}
