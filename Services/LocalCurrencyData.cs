using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Currency Data")]
public class LocalCurrencyData : ScriptableObject
{
    [ReadOnly] public float Gems;
    [ReadOnly] public int PlayCurrency;
    [ReadOnly] public int MaxPlayCurrency;
    [ReadOnly] public int Material_1;
    [ReadOnly] public int MaxMaterial_1;
    [ReadOnly] public int Material_2;
}
