using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Umožní přidávat více tagů jednomu objektu
 * 
 */
public class MultiTag : MonoBehaviour
{
    public List<string> tags = new List<string>();
    
    /**
     * Zkontroluje, zda má objekt daný tag
     */
    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    /**
     * Přidá tag do seznamu tagů
     */
    public void AddTag(string tag)
    {
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
        }
    }
}
