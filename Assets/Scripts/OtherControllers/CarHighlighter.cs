using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarParts { Doors, Windows, Wheels, Headlight, Taillight }

public class CarHighlighter : MonoBehaviour
{
    public FloatingAlpha doors;
    public FloatingAlpha windows;
    public FloatingAlpha wheel1;
    public FloatingAlpha wheel2;
    public FloatingAlpha headLight;
    public FloatingAlpha taillight;

    public void SetColor(Color color)
    {
        doors.image.color = color;
        windows.image.color = color;
        wheel1.image.color = color;
        wheel2.image.color = color;
        headLight.image.color = color;
        taillight.image.color = color;
    }

    public void HighlightPart(CarParts part)
    {
        switch (part)
        {
            case CarParts.Doors:
                doors.StartFloating(0f);
                break;
            case CarParts.Windows:
                windows.StartFloating(0f);
                break;
            case CarParts.Wheels:
                wheel1.StartFloating(0f);
                wheel2.StartFloating(0f);
                break;
            case CarParts.Headlight:
                headLight.StartFloating(0f);
                break;
            case CarParts.Taillight:
                taillight.StartFloating(0f);
                break;
            default:
                break;
        }
    }

    public void StopAll()
    {
        doors.StopFloating();
        windows.StopFloating();
        wheel1.StopFloating();
        wheel2.StopFloating();
        headLight.StopFloating();
        taillight.StopFloating();
    }
}
