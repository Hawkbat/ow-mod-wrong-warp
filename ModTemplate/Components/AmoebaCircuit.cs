using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaCircuit : WrongWarpBehaviour
    {
        public GameObject SwarmPrefab;

        public AmoebaWaypoint SpawnWaypoint;

        public List<AmoebaSwarm> Swarms { get; protected set; } = new List<AmoebaSwarm>();

        public List<AmoebaWaypoint> Waypoints { get; protected set; } = new List<AmoebaWaypoint>();

        public float SpawnRate;

        float spawnTimer;

        public override void WireUp()
        {

        }

        void Update()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > SpawnRate)
            {
                spawnTimer -= SpawnRate;
                var swarm = Instantiate(SwarmPrefab, transform, false).GetComponent<AmoebaSwarm>();
                swarm.Circuit = this;
                swarm.transform.position = SpawnWaypoint.transform.position;
                swarm.transform.rotation = SpawnWaypoint.transform.rotation;
                swarm.gameObject.SetActive(true);
                swarm.SetUp(SpawnWaypoint);
                Swarms.Add(swarm);
            }
        }

        public void Reclaim(AmoebaSwarm swarm)
        {
            swarm.gameObject.SetActive(false);
            Swarms.Remove(swarm);
            Destroy(swarm.gameObject);
        }
    }
}
