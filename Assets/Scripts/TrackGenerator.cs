using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackGenerator : MonoBehaviour
{
	/// <summary>
	/// Das Zentrum des Bereichs in dem die Strecke generiert werden soll
	/// </summary>
	public Transform Center;

	/// <summary>
	/// Der minimale Radius auf dem die Strecke ungefähr liegen soll
	/// </summary>
	public Single MinRadius;

	/// <summary>
	/// Der maximale Radius auf dem die Strecke ungefähr liegen soll
	/// </summary>
	public Single MaxRadius;

	/// <summary>
	/// Die RaceManager Komponenten.
	/// </summary>
	public List<RaceManager> Managers;

	/// <summary>
	/// Vorgefertigte Instanzen der Checkpointobjektes
	/// </summary>
	public List<GameObject> CheckpointPrefab;

	/// <summary>
	/// Eine vorgefertigte Instanz des Straßenobjektes
	/// </summary>
	public GameObject StreetPrefab;

	/// <summary>
	/// Eine vorgefertigte Instanz des Kurvenobjektes
	/// </summary>
	public GameObject CurvePrefab;

	/// <summary>
	/// Eine vorgefertigte Instanz des Kreuzungsobjektes
	/// </summary>
	public GameObject CrossingPrefab;

	/// <summary>
	/// Der Punkt an dem mit dem Erstellen der Strecke begonnen wird
	/// </summary>
	public Transform StartPoint;

	/// <summary>
	/// Das Terrain in dem die Strecke generiert wird
	/// </summary>
	public Terrain Terrain;

	/// <summary>
	/// Alle Elemente der generierten Straße
	/// </summary>
	private List<GameObject> _street;

	/// <summary>
	/// Die Instanz der Minimap
	/// </summary>
	public Minimap Minimap;

	/// <summary>
	/// Generiert die Checkpoints der Strecke 
	/// </summary>
	void Start()
	{
		Random.InitState(GlobalState.Seed.GetHashCode());
		_street = new List<GameObject>();
		Int32 rounds = Random.Range(1, 5);

		// Anzahl an Checkpoints
		Int32 checkpoints = Random.Range(4, 5);
		Vector3[] points = new Vector3[checkpoints];

		for (Int32 i = 0; i < checkpoints; i++)
		{
			Vector3 point = Random.onUnitSphere * Random.Range(MinRadius, MaxRadius);
			Vector3 position = new Vector3(point.x, 0, point.y);

			// Keine Punkte zulassen die aufeinander folgen und eine Ebene teilen
			if (points.Any(p =>
				Math.Abs(Center.InverseTransformPoint(p).x - position.x) < 50 ||
				Math.Abs(Center.InverseTransformPoint(p).z - position.z) < 50))
			{
				i--;
				continue;
			}

			// Checkpoint erstellen
			for (Int32 j = 0; j < Managers.Count; j++)
			{
				GameObject checkpoint = Instantiate(CheckpointPrefab[j]);
				checkpoint.transform.position = Center.TransformPoint(position);
				points[i] = checkpoint.transform.position;
				Managers[j].Checkpoints.Add(checkpoint.GetComponent<TriggerDetector>());
				Managers[j].Rounds = rounds;
			}
		}

		// Start und ersten Punkt verbinden
		ConnectWithStreet(StartPoint.transform.position, points[0], points[1]);

		// Streckenelemente zwischen den Punkten generieren
		for (Int32 i = 1; i < checkpoints; i++)
		{
			Vector3 next = StartPoint.transform.position;
			if (i < checkpoints - 1)
			{
				next = points[i + 1];
			}

			ConnectWithStreet(points[i - 1], points[i], next);
		}

		// Start und letzten Punkt verbinden
		ConnectWithStreet(points[checkpoints - 1], StartPoint.transform.position, points[0]);

		// Holzfäller
		List<TreeInstance> trees = new List<TreeInstance>(Terrain.terrainData.treeInstances);
		for (Int32 i = 0; i < Terrain.terrainData.treeInstanceCount; i++)
		{
			TreeInstance instance = Terrain.terrainData.treeInstances[i];
			Vector3 treePosition = Vector3.Scale(instance.position, Terrain.terrainData.size) + Terrain.transform.position;
			for (Int32 j = 0; j < _street.Count; j++)
			{
				if ((_street[j].transform.position - treePosition).magnitude < 15)
				{
					trees.Remove(instance);
				}
			}
		}

		Terrain.terrainData = Instantiate(Terrain.terrainData);
		Terrain.terrainData.treeInstances = trees.ToArray();

		// Damit die Collider auch entfernt werden muss das Terrain einmal geupdated werden
		Terrain.GetComponent<TerrainCollider>().terrainData = Terrain.terrainData;

		// Bestimmte Straßenobjekte aus der Liste auswählen
		GameObject[][] crossings = _street.Select(s =>

			// Alle Straßenobjekte auswählen, die mit einem anderen Objekt kollidieren
				_street.Where(s_ =>
					s_.GetComponent<Collider>().bounds.Intersects(s.GetComponent<Collider>().bounds) &&
					
					// Überprüfen ob die Objekte näher als 10 Meter sind
					(s.transform.position - s_.transform.position).magnitude < 10 &&
					
					// Überprüfen ob die Objekte unterschiedliche Ausrichtunge haben
					(Math.Abs(s.transform.forward.x) < 0.1 && Math.Abs(s_.transform.forward.x) > 0.1 || Math.Abs(s.transform.forward.z) < 0.1 && Math.Abs(s_.transform.forward.z) > 0.1 || s == s_)
				).ToArray()
		)
			
		// Nur Auflistungen mit mehr als einem Objekt zulassen, da sonst Objekte mit sich selbst kollidieren
		.Where(t => t.Length > 1).ToArray();
		
		// Alle Kreuzungen erstellen
		List<Vector3> crossingPoints = new List<Vector3>();
		foreach (GameObject[] cross in crossings)
		{
			Vector3 intersection;
			if (LineLineIntersection(out intersection, cross[0].transform.position, cross[0].transform.forward,
				cross[1].transform.position, cross[1].transform.forward) && !crossingPoints.Contains(intersection))
			{
				crossingPoints.Add(intersection);
			}
		}

		for (Int32 i = 0; i < crossingPoints.Count; i++)
		{
			GameObject crossing = Instantiate(CrossingPrefab);
			crossing.transform.position = crossingPoints[i];
			
			// Die Kreuzung etwas anheben, sodass sie über den anderen Streckenelementen rendert
			crossing.transform.Translate(0, 0.001f, 0);
		}
		
		// Minimap korrekt positionieren
		List<Vector3> _points = new List<Vector3>(points);
		_points.Add(StartPoint.position);
		Minimap.Center = CenterOfVectors(_points.ToArray());
		Minimap.Radius = Math.Min(250f, _points.Max(p => (p - Minimap.Center).magnitude));
	}

	/// <summary>
	/// Verbindet zwei Punkte mit einer Straße
	/// </summary>
	private void ConnectWithStreet(Vector3 start, Vector3 end, Vector3 next)
	{
		Vector3 dist = end - start;
		
		// Punkte in der X Ebene verbinden
		Vector3 curvePosition = GenerateStreet(start, new Vector3(Mathf.Sign(dist.x), 0, 0), Mathf.Abs(dist.x), 12f, 12f);
		
		// Kurve generieren
		GenerateCurve(curvePosition, new Vector3(Mathf.Sign(dist.x), 0, 0), new Vector3(0, 0, Mathf.Sign(dist.z)));
		
		// Punkte in der Z-Ebene verbinden
		curvePosition = GenerateStreet(curvePosition, new Vector3(0, 0, Mathf.Sign(dist.z)), Mathf.Abs(dist.z), 12f, 12f);
		
		// Kurve generieren
		Vector3 newDist = next - end;
		GenerateCurve(curvePosition, new Vector3(0, 0, Mathf.Sign(dist.z)), new Vector3(Mathf.Sign(newDist.x), 0, 0));

	}

	/// <summary>
	/// Generiert eine Kurve die zwei Streckenabschnitte verbindet
	/// </summary>
	private void GenerateCurve(Vector3 position, Vector3 inDir, Vector3 outDir)
	{
		GameObject curve = Instantiate(CurvePrefab);
		curve.transform.position = position;
		curve.transform.forward = inDir;
		
		if (inDir.x < 0)
		{
			if (outDir.z > 0)
			{
				curve.transform.Rotate(Vector3.up, -90, Space.Self);
			}
		}
		else if (inDir.x > 0)
		{
			if (outDir.z < 0)
			{
				curve.transform.Rotate(Vector3.up, -90, Space.Self);
			}
		} 
		else if (inDir.z > 0)
		{
			if (outDir.x > 0)
			{
				curve.transform.Rotate(Vector3.up, -90, Space.Self);
			}
		}
		else if (inDir.z < 0)
		{
			if (outDir.x < 0)
			{
				curve.transform.Rotate(Vector3.up, -90, Space.Self);
			}
		}
		_street.Add(curve);
	}

	/// <summary>
	/// Generiert einen geraden Streckenabschnitt in eine bestimmte Richtung
	/// </summary>
	private Vector3 GenerateStreet(Vector3 start, Vector3 direction, Single length, Single emptySpaceBefore, Single emptySpaceAfter)
	{
		// Jedes Streckenelement is standardmäßig 10 Meter lang
		// Entweder passt alles, oder das letzte Element muss kürzer sein
		length -= emptySpaceAfter;
		Single distance = emptySpaceBefore;
		while (distance < length - 10)
		{
			GameObject street = Instantiate(StreetPrefab);
			street.transform.position = start + (direction * distance);
			street.transform.forward = direction;
			_street.Add(street);
			distance += 10f;
		}
		
		// Muss noch ein Streckenelement nachgeschoben werden?
		if (length - distance > 0)
		{
			GameObject street = Instantiate(StreetPrefab);
			street.transform.position = start + (direction * distance);
			street.transform.forward = direction;
			street.transform.localScale = new Vector3(street.transform.localScale.x,
				street.transform.localScale.y, street.transform.localScale.z * (length - distance) / 10f);
			_street.Add(street);
		}

		return start + direction * (length + emptySpaceAfter);
	}

	/// <summary>
	/// Berechnet das Zentrum meherer Vektoren
	/// </summary>
	public Vector3 CenterOfVectors(Vector3[] vectors)
	{
		Vector3 sum = Vector3.zero;
		if (vectors == null || vectors.Length == 0)
		{
			return sum;
		}

		foreach (Vector3 vec in vectors)
		{
			sum += vec;
		}

		return sum / vectors.Length;
	}

	/// <summary>
	/// Berechnet den Schnittpunkt von zwei Linien
	/// </summary>
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1,
		Vector3 linePoint2, Vector3 lineVec2)
	{

		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		//is coplanar, and not parrallel
		if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
			return true;
		}

		intersection = Vector3.zero;
		return false;
	}
}
