using System;
using UnityEngine;

/// <summary>
/// Komponente die ein Objekt hinter einem anderen Objekt herführt.
/// </summary>
public class Follower : MonoBehaviour
{
    /// <summary>
    /// Die Geschwindigkeit mit der eine Änderung der Rotation umgesetzt wird.
    /// </summary>
    public Single RotationDamping;

    /// <summary>
    /// Der Höhenunterschied zum Ziel
    /// </summary>
    public Single Height;

    /// <summary>
    /// Der Abstand zum Ziel
    /// </summary>
    public Single Distance;

    /// <summary>
    /// Geschwindigkeit mit der die Änderung der Position umgesetzt wird.
    /// </summary>
    public Single SpeedDamping;

    /// <summary>
    /// Das Ziel dem das Objekt folgen soll
    /// </summary>
    public Transform Target;

	void Update ()
    {
        Vector3 wantedPosition = Target.TransformPoint(0, Height, -Distance);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * SpeedDamping);

        Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping);
    }
}
