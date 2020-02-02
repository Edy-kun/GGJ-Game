using System.Collections.Generic;

public interface IRepairable : IDamageable
{
    
    List<Element> GetRequiredItem();
    void Repair();
    void Break();

    bool NeedsRepair();


}