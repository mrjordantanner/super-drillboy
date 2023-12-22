using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public static class Utils
{
    //public static IEnumerator WaitFor(object target, float timeout)
    //{
    //    var interval = 0.1f;
    //    var waitTime = timeout * interval;
    //    float initTimer = 0;
    //    while (target == null)
    //    {
    //        initTimer += interval;
    //        yield return new WaitForSecondsRealtime(interval);
    //        if (initTimer >= waitTime)
    //        {
    //            yield break;
    //        }
    //    }
    //}

    public static IEnumerator WaitFor(bool value, float timeout)
    {
        var interval = 0.1f;
        var waitTime = timeout * interval;
        float initTimer = 0;
        while (!value)
        {
            initTimer += interval;
            yield return new WaitForSecondsRealtime(interval);
            if (initTimer >= waitTime)
            {
                yield break;
            }
        }
    }

    public static float CalculateSliderValue(float current, float max)
    {
        return (float)current / max;
    }

    public static float ConvertPercentageToDecibels(float percentage)
    {
        return percentage < 0.01 ? -80f : Mathf.Log10(percentage) * 20;
    }

    public static string FormatPercent(float value, int decimalPlaces = 0)
    {
        var percent = value * 100;
        var valueToRound = decimalPlaces > 0 ? (percent * Mathf.Pow(10.0f, decimalPlaces) / Mathf.Pow(10.0f, decimalPlaces)) : percent;
        float roundedValue = Mathf.Round(valueToRound);
        return $"{roundedValue}%";
    }

    // NOTE: Sprite must be marked as read/write enabled in import settings, and this affects performance
    public static Sprite CreateInvertedColorSprite(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Color32[] pixels = texture.GetPixels32();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(1f - pixels[i].r, 1f - pixels[i].g, 1f - pixels[i].b, pixels[i].a);
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static bool ClickOrTap()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        return false;
    }

    public static bool CoinFlip()
    {
        return Random.value <= 0.5f;
    }

    public static KeyValuePair<TKey, TValue> GetRandomFromDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        System.Random random = new();
        int randomIndex = random.Next(0, dictionary.Count);
        return dictionary.ElementAt(randomIndex);
    }

    public static T GetRandomFromArray<T>(T[] array)
    {
        if (array.Any())
        {
            return array[Random.Range(0, array.Length)];
        }

        Debug.LogError("Couldn't return random item from array because it was null.");
        return default;
    }

    public static T GetRandomEnumValue<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }

    public static string FormatNumberWithCommas(long number)
    {
        string formattedNumber = string.Format("{0:n0}", number);
        return formattedNumber;
    }

    public static object ArrayIsValid(object[] objectArray)
    {
        var arrayIsValid = objectArray.Length > 0 && objectArray[0] != null;

        if (arrayIsValid)
        {
            return arrayIsValid;
        }

        return false;
    }

    public static string TimeFormat(float clock)
    {
        return string.Format("{0:0}:{1:00}", Mathf.FloorToInt(clock / 60F), Mathf.FloorToInt(clock - (Mathf.FloorToInt(clock / 60F)) * 60));
    }

    public static float CalculatePercentage(float current, float max)
    {
        return current / max * 100;
    }

    public static float RandomizeValueWithinRange(float value, float valueRange)
    {
        return Random.Range(value * (1 + valueRange), value * (1 - valueRange));
    }

    public static Vector2 GetRandomDirection()
    {
        var x = Random.insideUnitCircle;
        if (x == Vector2.zero) x = GetRandomDirection();
        return x.normalized;
    }

    public static float GetDistance(Vector3 position1, Vector3 position2)
    {
        var heading = position1 - position2;

        var distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        var distance = Mathf.Sqrt(distanceSquared);

        return distance;
    }

    public static Vector2 GetDirection(Vector3 position1, Vector3 position2)
    {
        Vector3 direction;
        var heading = position1 - position2;

        var distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        var distance = Mathf.Sqrt(distanceSquared);

        direction.x = heading.x / distance;
        direction.y = heading.y / distance;
        direction.z = heading.z / distance;

        return direction.normalized;
    }

    public static string ShortName(string name)
    {
        if (name.Contains("Clone"))
        {
            return name.Remove(name.Length - 7);
        }
        // Remove "(#)" from name
        else if (name.Contains("("))
        {
            return name.Remove(name.Length - 4);
        }

        return name;
    }

    public static float GetRandomRoundedValue(float min, float max)
    {
        var rand = Random.Range(min, max);
        var roundedValue = Math.Round(rand * 2) / 2.0; // Rounds to the nearest 0.5
        return (float)roundedValue;
    }

    public static bool ValueIsBetween(float value, float min, float max)
    {
        if (value <= max && value >= min)
        {
            return true;
        }

        return false;
    }

}

