using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
	/// <summary>
	/// Eine Liste mit Buttons die sich gegenseitig umschalten
	/// </summary>
	public List<Button> Buttons;

	/// <summary>
	/// Der Hintergrund des Buttons wenn er ausgewählt ist
	/// </summary>
	public Sprite Selected;

	/// <summary>
	/// Der Hintergrund des Buttons wenn er nicht ausgewählt ist
	/// </summary>
	public Sprite NotSelected;
	
	void Start()
	{
		for (Int32 i = 0; i < Buttons.Count; i++)
		{
			Int32 current = i;
			Buttons[i].onClick.AddListener(() =>
			{
				for (Int32 j = 0; j < Buttons.Count; j++)
				{
					if (j == current)
					{
						Buttons[j].GetComponent<Image>().sprite = Selected;
					}
					else
					{
						Buttons[j].GetComponent<Image>().sprite = NotSelected;
					}
				}
			});
		}
	}
}
