using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to include this if you're using TextMeshPro for your dropdown

public class DifficultyDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Assign this in the inspector

    void Start()
    {
        // Clear existing options (if any)
        dropdown.ClearOptions();

        // Define the options
        var options = new TMP_Dropdown.OptionDataList();
        options.options.Add(new TMP_Dropdown.OptionData("Easy"));
        options.options.Add(new TMP_Dropdown.OptionData("Normal"));
        options.options.Add(new TMP_Dropdown.OptionData("Expert"));

        // Add options to the dropdown
        dropdown.options = options.options;

        // Optionally set a default value
        dropdown.value = 1; // 0 for Easy, 1 for Normal, 2 for Expert
    }
}
