using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxieMixer.Unity;

public class GameManager : Manager<GameManager>
{
    public List<BaseEntity> allEntitiesPrefab;

    Dictionary<Team, List<BaseEntity>> entitiesByTeam = new Dictionary<Team, List<BaseEntity>>();

    int unitsPerTeam = 6;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        Mixer.Init();
        InstantiateUnits();
    }

    private void InstantiateUnits()
    {
        entitiesByTeam.Add(Team.TeamATK, new List<BaseEntity>());
        entitiesByTeam.Add(Team.TeamDEF, new List<BaseEntity>());
        for (int i = 0; i < unitsPerTeam; i++)
        {
            //TeamATK
            int randomIndex = UnityEngine.Random.Range(0, allEntitiesPrefab.Count - 1);
            BaseEntity newEntity = Instantiate(allEntitiesPrefab[randomIndex]);
            entitiesByTeam[Team.TeamATK].Add(newEntity);

            newEntity.Setup(Team.TeamATK, GridManager.Instance.GetFreeNode(Team.TeamATK));

            //TeamDEF
            randomIndex = UnityEngine.Random.Range(0, allEntitiesPrefab.Count - 1);
            newEntity = Instantiate(allEntitiesPrefab[randomIndex]);
            entitiesByTeam[Team.TeamDEF].Add(newEntity);

            newEntity.Setup(Team.TeamDEF, GridManager.Instance.GetFreeNode(Team.TeamDEF));
        }
    }

    public List<BaseEntity> GetEntitiesEnemy(Team enemyTeam)
    {
        if (enemyTeam == Team.TeamDEF)
        {
            return entitiesByTeam[Team.TeamATK];
        }
        else
        {
            return entitiesByTeam[Team.TeamDEF];
        }
    }
    
    public void UnitDead(BaseEntity e)
    {
        entitiesByTeam[e.myTeam].Remove(e);

        Destroy(e.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum Team
{
    TeamATK, 
    TeamDEF
}
