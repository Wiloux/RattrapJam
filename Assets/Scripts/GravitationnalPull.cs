using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationnalPull : MonoBehaviour
{

    public bool isDebris;
    public GameObject Debris;
    [SerializeField] private Transform pulledTarget;
    [SerializeField] public Transform pullingObject;
    [SerializeField] public PlayerController player;
    private Rigidbody targetBody;
    [SerializeField] public float influenceRange;
    [SerializeField] private float intensity;
    private float distanceToPlayer;
    private Vector3 pullForce;
    public float playerStrengthPercentage;
    private Vector3 initialScale;

    public Material currentMat;
    public GameObject rot;

    public GameObject planetExplosionFX;

    public ParticleSystem suckParticles;
    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    public float rotationSpeed = 50;
    private Vector3 rotationAxis = new Vector3(0, 0, 1);
    int count;

    private Enemy myEnemy;

    public float maxDistance;

    private Spawner spawner;


    // Start is called before the first frame update
    void Start()
    {
        pulledTarget = this.transform;
        targetBody = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
        myEnemy = GetComponent<Enemy>();
        pullingObject = player.transform;
        initialScale = transform.localScale;

        spawner = FindObjectOfType<Spawner>();

        if (!isDebris && rot != null)
        {
            currentMat = rot.GetComponentInChildren<MeshRenderer>().material;
            currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strength);
        }
    }

    private float t = 0;
    // Update is called once per frame
    void Update()
    {


        if (myEnemy.strength <= 0 && !myEnemy.dead)
        {
            StartCoroutine(Death(true));
        }

        suckParticlesFunc();

        distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);


        if (distanceToPlayer > maxDistance * player.transform.lossyScale.magnitude)
        {
            Destroy(gameObject);
            spawner.currentUnits--;
        }

        if (distanceToPlayer < influenceRange * transform.localScale.magnitude)
        {
            transform.RotateAround(pullingObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
            pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPlayer * intensity;
            targetBody.AddForce(pullForce);

            if (!isDebris && currentMat != null)
                currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strengthMax);


            if (transform.localScale.magnitude > pullingObject.localScale.magnitude) //If Im bigger than the player
            {
                if (pullingObject.gameObject.GetComponent<PlayerController>() != null)
                {
                    myEnemy.strength -= (player.strength * playerStrengthPercentage) * Time.deltaTime;
                    if (!suckParticles.isPlaying)
                        suckParticles.Play();
                }
            }
            else //If I'm smaller
            {
                if (suckParticles.isPlaying)
                    suckParticles.Stop();
                StartCoroutine(Death());
            }

        }
        else if (distanceToPlayer > influenceRange + influenceRange / 2)
        {
            if (suckParticles.isPlaying)
                suckParticles.Stop();
            //targetBody.velocity = Vector3.zero;
        }
    }


    void suckParticlesFunc()
    {
        count = suckParticles.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            ParticleSystem.Particle particle = particles[i];

            Vector3 v1 = suckParticles.transform.TransformPoint(particle.position);
            Vector3 v2 = player.transform.position;

            Vector3 tarPosi = (v2 - v1) * (particle.lifetime / particle.startLifetime);
            particle.position = suckParticles.transform.InverseTransformPoint(v2 - tarPosi);
            particles[i] = particle;
        }

        suckParticles.SetParticles(particles, count);
    }
    IEnumerator Death(bool spawnDebris = false)
    {
        myEnemy.dead = true;
        Vector3 initscale = transform.lossyScale;
        Vector3 initPosition = transform.position;
        float elapsedTime = 0;
        float waitTime = 0.8f;

        if (!spawnDebris)
        {

            while (elapsedTime < waitTime)
            {
                transform.localScale = Vector3.Lerp(initscale, Vector3.zero, (elapsedTime / waitTime));
                transform.position = Vector3.Lerp(initPosition, player.transform.position, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                // Yield here
                yield return null;
            }
            if (pullingObject == player.gameObject.transform)
            {
                Debug.Log("Sussy");
                player.UpdateScaleAndStrength(myEnemy);
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
            List<Debris> debris = new List<Debris>();
            int debrisAmount = Random.Range(2, 6);
            for (int i = 0; i < debrisAmount; i++)
            {
                Vector2 explosionDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                Debris newenemy = Instantiate(Debris, transform.position, Quaternion.identity).GetComponent<Debris>();
                debris.Add(newenemy);
                newenemy.GetComponent<Rigidbody>().AddForce(Random.Range(15, 20) * explosionDir, ForceMode.Impulse);
            }

            float debrisStrength = myEnemy.strengthMax / debrisAmount;

            for (int i = 0; i < debrisAmount; i++)
            {
                debris[i].strength = debrisStrength;
                debris[i].transform.localScale = initscale;
            }

            GameObject exFX = Instantiate(planetExplosionFX, transform.position, Quaternion.identity);
            exFX.transform.localScale = initscale;
            Destroy(exFX, 2);

        }

        spawner.currentUnits--;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (transform.localScale.magnitude < player.transform.localScale.magnitude)
        {
            StartCoroutine(Death());
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
            Gizmos.DrawWireSphere(transform.position, maxDistance * player.transform.lossyScale.magnitude);


        Gizmos.DrawWireSphere(transform.position, influenceRange * transform.lossyScale.magnitude);
    }
}

