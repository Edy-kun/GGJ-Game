using System.Collections.Generic;

public class Inventory
{
    public List<Element> items = new List<Element>();

    public void TrySubstract(Element cost)
    {
        var element = items.Find(e => e.type == cost.type);
        if (element.Amount > cost.Amount)
        {
            element -= cost;
        }


    }

    public void AddElement(Element element)
    {
        var found =  items.Find(e => e.type == element.type);
        if (found == null)
        {
            items.Add(element);
        }
        else
        {
            found += element;
        }
        
    }
    
}