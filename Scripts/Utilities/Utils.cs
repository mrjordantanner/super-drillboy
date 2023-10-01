using UnityEngine;
using System.Reflection;
using Random = UnityEngine.Random;
using UnityEditor;
using System;

public static class Utils
{
    //public static Vector3 GetPlayerToMouseDirection(Vector2 mousePosition)
    //{
    //    return Utils.GetDirection(mousePosition, ObjRef.Instance.player.transform.position);
    //}

    //public static float GetPlayerToMouseDistance(Vector2 mousePosition)
    //{
    //    return Utils.GetDistance(mousePosition, ObjRef.Instance.player.transform.position);
    //}

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

    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector2 GetMouseViewportPosition()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    public static bool ArrayIsValid(object[] objectArray)
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

    public static void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
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

