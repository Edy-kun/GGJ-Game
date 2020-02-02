using System;
using System.Collections.Generic;

public class Inventory
{
    public List<Element> items = new List<Element>();
    public Dictionary<ElementType, int> elements = new Dictionary<ElementType, int>();

    public Inventory()
    {
        elements.Add(ElementType.Tape, 2);
        elements.Add(ElementType.Cog, 3);
        elements.Add(ElementType.Prop, 2);
        elements.Add(ElementType.Ammo, 100);
    }

    public event Action<Inventory> OnElementsChanged;
    public bool TrySubstract(Element cost)
    {
        //var element = items.Find(e => e.type == cost.type);
        if (elements[cost.type] > cost.Amount)
        {
           
            elements[cost.type] -= cost.Amount; 
            OnElementsChanged?.Invoke(this);
            return true;
        }

        return false;


    }

    public bool TrySubstract(List<Element> cost)
    {
        if (!cost.TrueForAll(item => (elements[item.type] > item.Amount)))
            return false;

        foreach (var item in cost)
        {
            TrySubstract(item);
        }

        return true;

    }

    public void AddElement(Element element)
    {
        elements[element.type] += element.Amount;
        OnElementsChanged?.Invoke(this);
    }


    public void Init()
    {
     OnElementsChanged?.Invoke(this);
    }
}