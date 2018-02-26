using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// An RNG is a Random Number Generator. It uses 
// a static value, called seed, to generate hardly
// predictable numbers. Both, .NET and Unity already
// implement a RNG, but both aren't completely what
// we want, so this class serves as a wrapper.
public class RNG
{
    /// <summary>
    /// The .NET RNG reference.
    /// </summary>
    private System.Random _random;
    
    /// <summary>
    /// Create a new RNG with a random seed calculated from system time
    /// </summary>
    public RNG()
    {
        _random = new System.Random();
    }

    /// <summary>
    /// Create a new RNG with a custom seed
    /// </summary>
    /// <param name="seed">
    /// The seed that is used for generating numbers. If the string is parseable as an integer, 
    /// the interger form will get used, otherwise the code falls back to using the hash of the string.
    /// </param>
    public RNG(String seed)
    {
        _random = new System.Random(ParseSeed(seed));
    }

    /// <summary>
    /// Create a new RNG with a custom seed
    /// </summary>
    /// <param name="seed">The object whose hash code will be used as the seed.</param>
    public RNG(System.Object seed)
    {
        _random = new System.Random(seed.GetHashCode());
    }

    /// <summary>
    /// Transform a string into an integer representation
    /// </summary>
    private Int32 ParseSeed(String seed)
    {
		Int32 intSeed;
        if (!Int32.TryParse(seed, out intSeed))
        {
            intSeed = seed.GetHashCode();
        }
        return intSeed;
    }

    private Int16 Next(Int16 minValue, Int16 maxValue)
    {
        return Next(minValue, maxValue);
    }

    private Double Next(Double minValue, Double maxValue)
    {
        return (_random.NextDouble() * (maxValue - minValue)) + minValue;
    }

    private Single Next(Single minValue, Single maxValue)
    {
        return (Single)Next((Double)minValue, maxValue);
    }

    /// <summary>
    /// Generates a random positive number
    /// </summary>
    public T Next<T>()
    {
        if (typeof(T) == typeof(Int32))
        {
            return (T)(System.Object)Next(0, Int32.MaxValue);
        }
        else if (typeof(T) == typeof(Double))
        {
            return (T)(System.Object)Next(0, Double.MaxValue);
        }
        else if (typeof(T) == typeof(Single))
        {
            return (T)(System.Object)Next(0, Single.MaxValue);
        }
        else
        {
            throw new ArgumentException("Only Int32, Double and Single are supported!", nameof(T));
        }
    }

    /// <summary>
    /// Generates a random positive number that is smaller than maxValue
    /// </summary>
    public T Next<T>(T maxValue)
    {
        if (typeof(T) == typeof(Int32))
        {
            return (T)(System.Object)Next(0, (Int32)(System.Object)maxValue);
        }
        else if (typeof(T) == typeof(Double))
        {
            return (T)(System.Object)Next(0, (Double)(System.Object)maxValue);
        }
        else if (typeof(T) == typeof(Single))
        {
            return (T)(System.Object)Next(0, (Single)(System.Object)maxValue);
        }
        else
        {
            throw new ArgumentException("Only Int32, Double and Single are supported!", nameof(T));
        }
    }

    /// <summary>
    /// Generates a random positive number that is smaller than maxValue and higher than minValue
    /// </summary>
    public T Next<T>(T minValue, T maxValue)
    {
        if (typeof(T) == typeof(Int32))
        {
            return (T)(System.Object)Next((Int32)(System.Object)minValue, (Int32)(System.Object)maxValue);
        }
        else if (typeof(T) == typeof(Double))
        {
            return (T)(System.Object)Next((Double)(System.Object)minValue, (Double)(System.Object)maxValue);
        }
        else if (typeof(T) == typeof(Single))
        {
            return (T)(System.Object)Next((Single)(System.Object)minValue, (Single)(System.Object)maxValue);
        }
        else
        {
            throw new ArgumentException("Only Int32, Double and Single are supported!", nameof(T));
        }
    }
}
