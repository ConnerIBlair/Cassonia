using System.Collections.Generic;
using UnityEngine;
public class ColorSwatches : MonoBehaviour
{
    [SerializeField]
    private List<Color> _colourSwatches = new List<Color>();

    public List<Color> ColourSwatches => _colourSwatches;
}