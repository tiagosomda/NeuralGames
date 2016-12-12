using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharData : MonoBehaviour {

    public GameObject DataItemPrefab;
    public GameObject[] DataItemParent;

    public static GameObject _DataItemPrefab;
    public static GameObject[] _DataItemParent;

    public static List<string> data_names;
    public static List<Text> data_values;


    public void Start()
    {
        data_names = new List<string>();
        data_values = new List<Text>();

        _DataItemPrefab = DataItemPrefab;
        _DataItemParent = DataItemParent;
    }

    public static void PanelLeft(string name, string value, Color color)
    {
        SetDataItem(0, name, value, color);
    }

    public static void PanelRight(string name, string value, Color color)
    {
        SetDataItem(1, name, value, color);
    }

    private static void SetDataItem(int side, string name, string value, Color color)
    {
        if (!data_names.Contains(name))
        {
            if (data_names == null)
            {
                data_names = new List<string>();
                data_values = new List<Text>();
            }

            data_names.Add(name);

            var item = Instantiate(_DataItemPrefab) as GameObject;

            var data_n = item.transform.FindChild("Name").GetComponent<Text>();
            var data_v = item.transform.FindChild("Value").GetComponent<Text>();

            data_n.color = color;
            data_v.color = color;

            data_n.text = name;

            data_values.Add(data_v);

            item.transform.SetParent(_DataItemParent[side].transform);
        }

        var index = data_names.IndexOf(name);
        data_values[index].text = value;
    }
}
