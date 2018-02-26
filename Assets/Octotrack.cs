using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octotrack : MonoBehaviour
{
    /// <summary>
    /// The seed that is used to generate the track.
    /// </summary>
    public String Seed;

    /// <summary>
    /// The maximum radius of the track
    /// </summary>
    public Double MaxRadius;

    /// <summary>
    /// The minimum radius of the track
    /// </summary>
    public Double MinRadius;

    /// <summary>
    /// The random number generator
    /// </summary>
    private RNG _rng;

    /// <summary>
    /// The Start() method gets executed when the
    /// component is enabled. Here we can generate 
    /// the values for the various parameters
    /// from the seed.
    /// </summary>
    void Start()
    {
        _rng = new RNG(Seed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
