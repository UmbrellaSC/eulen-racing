using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Steuert das Auto durch Benutzereingaben.
/// </summary>
public class CarController : MonoBehaviour
{
    public enum InputType
    {
        /// <summary>
        /// Das Auto wird mit WASD gesteuert
        /// </summary>
        Primary,

        /// <summary>
        /// Das Auto wird mit den Pfeiltasten gesteuert
        /// </summary>
        Secondary
    }

    /// <summary>
    /// Die Komponente die dem Auto-Objekt physikalische 
    /// Eigenschaften verleiht.
    /// </summary>
    [HideInInspector]
    private Rigidbody _rigidbody;
    
    private Renderer _renderer;

    /// <summary>
    /// Auflistung der einzelnen Achsen des Autos
    /// </summary>
    public List<AxisInfo> CarAxes = new List<AxisInfo>();

    /// <summary>
    /// Die maximale Beschleunigung des Motors
    /// </summary>
    public Single MaxMotorTorque;

    /// <summary>
    /// Die maximale Auslenkung der Räder
    /// </summary>
    public Single MaxSteerAngle;

    /// <summary>
    /// Das Gewicht des Autos
    /// </summary>
    public Single Mass;

    /// <summary>
    /// Gibt an, wie das Auto gesteuert wird
    /// </summary>
    public InputType Type;

    /// <summary>
    /// Die Farbe des Autos
    /// </summary>
    public Color Color = Color.red;

    /// <summary>
    /// Das Material zur Darstelung des Autos
    /// </summary>
    public Material CarMaterial;

    /// <summary>
    /// Die momentane Geschwindigkeit des Autos
    /// </summary>
    public Single Speed
    {
        get { return _rigidbody.velocity.magnitude; }
    }

	/// <summary>
    /// Initialisierung des Autos
    /// </summary>
	void Start ()
    {
        // Komponenten aus der Struktur des GameObjects finden
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = gameObject.GetComponentsInChildren<Renderer>().FirstOrDefault(r => r.name == "Body");
        _renderer.materials[0] = Instantiate(CarMaterial);

        // Alle Werte auf 0 setzen
        for (Int32 i = 0; i < CarAxes.Count; i++)
        {
            CarAxes[i].Left.steerAngle = 0f;
            CarAxes[i].Left.motorTorque = 0f;
            CarAxes[i].Right.steerAngle = 0f;
            CarAxes[i].Right.motorTorque = 0f;
        }
	}
	
	/// <summary>
    /// Beschleunigung und Lenkung jeden Frame aktualisieren
    /// </summary>
	void Update ()
    {
        // Jede Eingabeachse kann Werte von -1 bis 1 annehmen. Durch das Multiplizieren mit dem
        // Maximalwert erhält man eine Abstufung von Geschwindigkeit und Auslenkung
        Single steerAngle = MaxSteerAngle * Input.GetAxis("Horizontal_" + Type);
        Single motorTorque = MaxMotorTorque * Input.GetAxis("Vertical_" + Type);

        // Durch alle Achsen iterieren
        for (Int32 i = 0; i < CarAxes.Count; i++)
        {
            // Ist die Achse mit dem Motor verbunden?
            if (CarAxes[i].Powered)
            {
                // Beschleunigung aktualisieren
                CarAxes[i].Left.motorTorque = motorTorque;
                CarAxes[i].Right.motorTorque = motorTorque;
            }

            // Ist die Achse lenkbar?
            if (CarAxes[i].Steerable)
            {
                CarAxes[i].Left.steerAngle = steerAngle;
                CarAxes[i].Right.steerAngle = steerAngle;
            }
        }

        // Farbe des Autos einstellen
        _renderer.materials[0].color = Color;

        // Gewicht des Autos einstellen
        _rigidbody.mass = Mass;
    }

    /// <summary>
    /// Beschreibt eine Achse eines Fahrzeugs
    /// </summary>
    [Serializable]
    public class AxisInfo
    {
        /// <summary>
        /// Das linke Rad der Achse
        /// </summary>
        public WheelCollider Left;

        /// <summary>
        /// Das rechte Rad der Achse
        /// </summary>
        public WheelCollider Right;

        /// <summary>
        /// Ist die Achse mit dem Motor verbunden?
        /// </summary>
        public Boolean Powered;

        /// <summary>
        /// Ist die Achse lenkbar?
        /// </summary>
        public Boolean Steerable;
    }
}
