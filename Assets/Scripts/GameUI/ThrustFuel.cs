using UnityEngine;
using UnityEngine.UI;

public class ThrustFuel
{
    private const float FuelLowWarningLevel = 20f;
    private Image _thrustFuelImage;
    private float _thrustFuelImageMaxWidth;
    private Rect _thrustFuelImageDefaultRect;
    private Color _thrustFuelImageStartingColor;

    public ThrustFuel(Image thrustFuelImage)
    {
        _thrustFuelImage = thrustFuelImage;
        _thrustFuelImageMaxWidth = _thrustFuelImage.rectTransform.rect.width;
        _thrustFuelImageDefaultRect = _thrustFuelImage.rectTransform.rect;
        _thrustFuelImageStartingColor = _thrustFuelImage.color;
    }

    public void UpdateThrustFuel(float thrustFuel)
    {
        var fuelLeftAsWidth = (thrustFuel / _thrustFuelImageMaxWidth) * 100f;
        _thrustFuelImage.rectTransform.sizeDelta = new Vector2(fuelLeftAsWidth, _thrustFuelImageDefaultRect.height);
        if (thrustFuel < FuelLowWarningLevel)
        {
            _thrustFuelImage.color = Color.red;
        }
        else
        {
            _thrustFuelImage.color = _thrustFuelImageStartingColor;
        }
    }
}
