using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GGJGameSettingsInstaller", menuName = "Installers/GGJGameSettingsInstaller")]
public class GGJGameSettingsInstaller : ScriptableObjectInstaller<GGJGameSettingsInstaller>
{
   

    public GameInstaller.Settings Settings;
    public EnemeySettings EnemySettings;
    
    
    [Serializable]
    public class EnemeySettings
    {
        public RandomEnemyPlacement.Settings spawnerSettigns;
        public TurretAI.Settings turretSettigns;
        
    }

    public override void InstallBindings()
    {
        Container.BindInstance(EnemySettings.spawnerSettigns);
        Container.BindInstance(EnemySettings.turretSettigns);
        Container.BindInstance(Settings);
    }

}