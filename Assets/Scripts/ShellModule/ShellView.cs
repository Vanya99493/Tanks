using TankModule;
using UnityEngine;

namespace ShellModule
{
    public class ShellView : MonoBehaviour
    {
        [SerializeField] private LayerMask tankMask;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private AudioSource explosionAudio;

        [Header("Settings")] 
        [SerializeField] private float maxDamage = 100f;
        [SerializeField] private float explosionForce = 1000f;
        [SerializeField] private float maxLiveTime = 2f;
        [SerializeField] private float explosionRadius = 5f;

        private void Start()
        {
            Destroy(gameObject, maxLiveTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Collider[] colliders = Physics.OverlapSphere (transform.position, explosionRadius, tankMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                if (!targetRigidbody)
                    continue;

                targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                if (targetRigidbody.TryGetComponent(out TankView tankView))
                {
                    int damage = (int)CalculateDamage(targetRigidbody.position);
                    tankView.TakeDamage(damage);
                }
            }

            explosionParticles.transform.parent = null;
            explosionParticles.Play();
            explosionAudio.Play();

            ParticleSystem.MainModule mainModule = explosionParticles.main;
            Destroy (explosionParticles.gameObject, mainModule.duration);

            Destroy (gameObject);
        }
        
        private float CalculateDamage (Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;
            
            float explosionDistance = explosionToTarget.magnitude;
            float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;
            float damage = relativeDistance * maxDamage;
            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}