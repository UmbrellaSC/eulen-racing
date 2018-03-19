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
    public Rigidbody CarRigidbody;
    
    private Renderer _renderer;

    /// <summary>
    /// Auflistung der einzelnen Achsen des Autos
    /// </summary>
    public List<AxisInfo> CarAxes = new List<AxisInfo>();

    /// <summary>
    /// Die maximale Beschleunigung des Autos
    /// </summary>
    public Single MaxMotorTorque;

    /// <summary>
    /// Die maximale Auslenkung der Räder
    /// </summary>
    public Single MaxSteerAngle;

    /// <summary>
    /// Wie stark die Räder abbremsen können
    /// </summary>
    public Single BrakeTorque;

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
    /// Die Komponente die das Auto auf dem Bildschirm anzeigt
    /// </summary>
    public Renderer CarRenderer;

    public Transform CenterOfMass;

	/// <summary>
    /// Initialisierung des Autos
    /// </summary>
	void Start ()
    {
        // Komponenten aus der Struktur des GameObjects finden
        CarRenderer.materials[0] = Instantiate(CarMaterial);

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
        // Durch alle Achsen iterieren
        for (Int32 i = 0; i < CarAxes.Count; i++)
        {
            // Jede Eingabeachse kann Werte von -1 bis 1 annehmen. Durch das Multiplizieren mit dem
            // Maximalwert erhält man eine Abstufung von Geschwindigkeit und Auslenkung
            Single steerAngle = MaxSteerAngle * Input.GetAxis("Horizontal_" + Type);
            Single motorTorque = MaxMotorTorque * Input.GetAxis("Vertical_" + Type);
            Single brakeTorque = BrakeTorque;

            // Ist die Achse mit dem Motor verbunden?
            if (!CarAxes[i].Powered)
            {
                // Beschleunigung aktualisieren
                motorTorque *= 0;
                brakeTorque *= 0;
            }

            // Ist die Achse lenkbar?
            if (!CarAxes[i].Steerable)
            {
                steerAngle *= 0;
            }

            ApplyInput(CarAxes[i].Left, motorTorque, steerAngle, brakeTorque);
            ApplyInput(CarAxes[i].Right, motorTorque, steerAngle, brakeTorque);
        }

        // Farbe des Autos einstellen
        CarRenderer.materials[0].color = Color;

        // Gewicht des Autos einstellen
        CarRigidbody.mass = Mass;
        CarRigidbody.centerOfMass = CenterOfMass.localPosition;
    }

    public Single GetSpeed(WheelCollider collider)
    {
        return Mathf.Round((Mathf.PI * 2 * collider.radius) * collider.rpm * 60 / 1000);
    }

    public void ApplyInput(WheelCollider collider, Single motorTorque, Single steerAngle, Single brakeTorque)
    {
        Single speed = GetSpeed(collider);
        Boolean braking = false;
        if ((speed > 0 && motorTorque <= 0) || (speed < 0 && motorTorque >= 0))
        {
            braking = true;
        }
        else
        {
            braking = false;
            collider.brakeTorque = 0f;
        }
        if (Mathf.Abs(speed) < 0.2f)
        {
            braking = false;
            collider.motorTorque = 0;
            collider.brakeTorque = 0;
        }

        if (!braking)
        {
            collider.motorTorque = motorTorque;            
        }
        else
        {
            collider.brakeTorque = brakeTorque;
            collider.motorTorque = 0f;
        }

        collider.steerAngle = steerAngle;
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
