using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    /**
     * Vypíše všechny komponenty, které má objekt
     */
    public static void ListComponents (GameObject gameObject)
    {
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            Debug.Log(component.GetType().Name);
        }
    }
}
