using UnityEngine;

[System.Serializable]
public class Element
{
    public ElementType type;
    public int Amount = 1;
   


    public static Element operator+ ( Element b, Element c)
    {
        Debug.Assert(b.type == c.type);
         b.Amount += c.Amount;
         return b;
    }
    public static Element operator- ( Element b, Element c)
    {
        Debug.Assert(b.type == c.type);
        b.Amount -= c.Amount;
        return b;
    }
}