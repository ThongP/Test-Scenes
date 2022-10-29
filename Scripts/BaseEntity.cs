using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using Random=System.Random;

public class BaseEntity : MonoBehaviour
{
    protected int maxHealth;
    //public int currentHealth;
    public HealthbarPreFab barPrefab;
    protected HealthbarPreFab healthBar;

    public int Damage;
    public float attackSpeed = 1f;
    public float movementSpeed = 0.5f;
    public int range = 1;

    [SerializeField] demoFigure _birdFigure;
    public Team myTeam;
    protected Node currentNode;

    protected BaseEntity currentTarget = null;
    protected bool moving;
    protected Node destinaion;

    protected bool canAttack = true;
    protected bool dead = false;
    protected float waitBetweenAttack;

    protected bool HasEnemy => currentTarget != null;
    protected bool IsInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= range;

    public void Setup(Team team, Node spawnNode)
    {
        myTeam = team;
        if(myTeam == Team.TeamDEF)
        {
            StartCoroutine(GetAxiesGenes("2724598", _birdFigure));
            maxHealth = 32;
            Damage = DamageCal();
        }
        else
        {
            StartCoroutine(GetAxiesGenes("4191804", _birdFigure));
            maxHealth = 16;
            Damage = DamageCal();
        }

        this.currentNode = spawnNode;
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);
        healthBar = Instantiate(barPrefab, this.transform);
        healthBar.Setup(this.transform, maxHealth);
    }

    protected void FindTarget()
    {
        var allEnemies = GameManager.Instance.GetEntitiesEnemy(myTeam);

        float minDistance = Mathf.Infinity;
        BaseEntity enemyTarget = null;
        foreach (BaseEntity e in allEnemies)
        {
            if (Vector3.Distance(e.transform.position, this.transform.position) <= minDistance)
            {
                minDistance = Vector3.Distance(e.transform.position, this.transform.position);
                enemyTarget = e;
            }
        }

        currentTarget = enemyTarget;
    }

    protected void GetInRange()
    {
        if (currentTarget == null)
            return;

        if (!moving)
        {
            Node targetDestination = null;
            List<Node> targets = GridManager.Instance.GetNodesCloseTo(currentTarget.currentNode);
            targets = targets.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                if (!targets[i].IsOccupied)
                {
                    targetDestination = targets[i];
                    break;
                }
            }

            if (targetDestination == null)
                return;

            //Find path
            var path = GridManager.Instance.GetPath(currentNode, targetDestination);
            if (path == null || path.Count <= 1)
                return;

            if (path[1].IsOccupied)
                return;

            path[1].SetOccupied(true);
            destinaion = path[1];
        }

        moving = !MoveToward();
        if (!moving)
        {
            currentNode.SetOccupied(false);
            currentNode = destinaion;
        }
    }

    protected bool MoveToward()
    {
        Vector3 direction = destinaion.worldPosition - this.transform.position;
        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = destinaion.worldPosition;
            return true;
        }

        this.transform.position += direction.normalized * movementSpeed * Time.deltaTime;
        return false;
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        healthBar.UpdateBar(maxHealth);

        if (maxHealth <= 0 && !dead)
        {
            dead = true;
            currentNode.SetOccupied(false);
            GameManager.Instance.UnitDead(this);
        }
    }

    protected virtual void Attack()
    {
        if (!canAttack)
            return;
        
        waitBetweenAttack = 1 / attackSpeed;
        StartCoroutine(WaitAttackCoroutine());
    }

    private int DamageCal()
    {
        int rndATK = 0;
        int rndDEF = 0;
        int damage = 0;
        if (myTeam == Team.TeamDEF)
        {
            Random rnd = new Random();
            rndDEF = rnd.Next(3);
        }
        else
        {
            Random rnd = new Random();
            rndATK = rnd.Next(3);
        }
        
        int temp = 3 + rndATK - rndDEF;
        if (temp % 3 == 0)
        {
            damage = 4;
        }
        else if (temp % 3 == 1)
        {
            damage = 5;
        }
        else if (temp % 3 == 2)
        {
            damage = 3;
        }

        return damage;
    }

    IEnumerator WaitAttackCoroutine()
    {
        canAttack = false;
        yield return null;
        yield return new WaitForSeconds(waitBetweenAttack);
        canAttack = true;

    }

    //Get model
        public IEnumerator GetAxiesGenes(string axieId, demoFigure _birdFigure)
        {
            string searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
            JObject jPayload = new JObject();
            jPayload.Add(new JProperty("query", searchString));

            var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
            wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            wr.SetRequestHeader("Content-Type", "application/json");
            wr.timeout = 10;
            yield return wr.SendWebRequest();
            if (wr.error == null)
            {
                var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
                if (!string.IsNullOrEmpty(result))
                {
                    JObject jResult = JObject.Parse(result);
                    string genesStr = (string)jResult["data"]["axie"]["newGenes"];
                    PlayerPrefs.SetString("selectingId", axieId);
                    PlayerPrefs.SetString("selectingGenes", genesStr);
                    _birdFigure.SetGenes(axieId, genesStr);
                }
            }
        }
}
