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

    /// <summary>
    /// Das Massezentrum des Autos
    /// </summary>
    public Transform CenterOfMass;

    /// <summary>
    /// Gibt die durchschnittliche Geschwindigkeit aller angetriebenen Räder zurück
    /// </summary>
    public Single Speed
    {
        get
        {
            Single _speed = 0;
            Int32 count = 0;
            for (Int32 i = 0; i < CarAxes.Count; i++)
            {
                if (CarAxes[i].Powered)
                {
                    _speed += GetSpeed(CarAxes[i].Left);
                    _speed += GetSpeed(CarAxes[i].Right);
                    count += 2;
                }
            }
            return _speed / count;
        }
    }

	/// <summary>
    /// Initialisierung des Autos
    /// </summary>
	void Start ()
    {
        // Komponenten aus der Struktur des GameObjects finden
        CarRenderer.materials[0] = Instantiate(CarMaterial);
        
        // Farbe aktualisieren
        if (Type == InputType.Primary)
        {
            Color = GlobalState.PlayerAColor;
        }
        else
        {
            Color = GlobalState.PlayerBColor;
        }

        // Alle Werte auf 0 setzen
        for (Int32 i = 0; i < CarAxes.Count; i++)
        {
            CarAxes[i].Left.steerAngle = 0f;
            CarAxes[i].Left.motorTorque = 0f;
            CarAxes[i].Right.steerAngle = 0f;
            CarAxes[i].Right.motorTorque = 0f;
        }
        
        // Cache erstellen
        pausedBrakeTorque = new List<Single>();
        pausedSteerAngle = new List<Single>();
        pausedMotorTorque = new List<Single>();
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

            if (!GlobalState.Paused && wasPaused)
            {
                steerAngle = pausedSteerAngle[i];
                motorTorque = pausedMotorTorque[i];
                brakeTorque = pausedBrakeTorque[i];
            }

            if (GlobalState.Paused && !wasPaused)
            {
                pausedSteerAngle.Add(steerAngle);
                pausedMotorTorque.Add(motorTorque);
                pausedBrakeTorque.Add(brakeTorque);
            }

            ApplyInput(CarAxes[i].Left, motorTorque, steerAngle, brakeTorque);
            ApplyInput(CarAxes[i].Right, motorTorque, steerAngle, brakeTorque);
        }

        // Farbe des Autos einstellen
        CarRenderer.materials[0].color = Color;

        // Gewicht des Autos einstellen
        CarRigidbody.mass = Mass;
        CarRigidbody.centerOfMass = CenterOfMass.localPosition;
        
        // Pause?
        if (!GlobalState.Paused && wasPaused)
        {
            CarRigidbody.isKinematic = false;
            wasPaused = false;
            pausedSteerAngle.Clear();
            pausedBrakeTorque.Clear();
            pausedMotorTorque.Clear();
        }

        if (GlobalState.Paused && !wasPaused)
        {
            wasPaused = true;
            CarRigidbody.isKinematic = true;
        }
    }

    // Zwischengespeicherte Werte für die pausierung des Spiels
    private Boolean wasPaused;
    private List<Single> pausedMotorTorque;
    private List<Single> pausedBrakeTorque;
    private List<Single> pausedSteerAngle;

    /// <summary>
    /// Berechnet die Geschwindigkeit mit der sich das spezifizierte Rad des Autos dreht
    /// </summary>
    public Single GetSpeed(WheelCollider collider)
    {
        return Mathf.Round((Mathf.PI * 2 * collider.radius) * collider.rpm * 60 / 1000);
    }

    /// <summary>
    /// Wendet die Eingaben des Benutzers auf die Räder des Autos an
    /// </summary>
    public void ApplyInput(WheelCollider collider, Single motorTorque, Single steerAngle, Single brakeTorque)
    {
        Single speed = GetSpeed(collider);
        Boolean braking;
        
        // Ist das Auto dabei zu bremsen?
        if (speed > 0 && motorTorque <= 0 || speed < 0 && motorTorque >= 0)
        {
            braking = true;
        }
        else
        {
            braking = false;
            collider.brakeTorque = 0f;
        }
        
        // Wenn die Geschwindigkeit des Autos zu gering ist, nicht mehr bremsen
        if (Mathf.Abs(speed) < 0.2f)
        {
            braking = false;
            collider.motorTorque = 0;
            collider.brakeTorque = 0;
        }
        
        // Wenn das Auto gegen einen Baum gefahren ist, bleibt das Momentum erhalten, es kann aber nicht weiterfahren
        // Das führt dazu, dass man erst einmal eine Zeitlang in die Gegenrichtung steuern muss bis sich das 
        // Auto wieder bewegt. Um das zu kompensieren setzen wird die Bremskraft auf den höchstmöglichen Wert, wenn 
        // die Geschwindigkeit 0 ist und der Benutzer keine Eingabe macht
        if (CarRigidbody.velocity.magnitude < 0.1f && Mathf.Abs(motorTorque / MaxMotorTorque) < 0.01f)
        {
            braking = false;
            collider.brakeTorque = Single.MaxValue;
        }
        else 
        {
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
