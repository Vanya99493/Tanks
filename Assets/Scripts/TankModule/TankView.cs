using System;
using ShellModule;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TankModule
{
    public class TankView : MonoBehaviour
    {
        public event Action<int> TakeDamageAction;
        
        [Header("Components")]
        [SerializeField] private TankCanvas tankCanvas;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private ParticleSystem explosionParticles;

        [Header("Fire settings")] 
        [SerializeField] private ShellView shellPrefab;
        [SerializeField] private Transform fireTransform;
        [SerializeField] private float minLaunchForce = 15f;
        [SerializeField] private float maxLaunchForce = 30f;
        [SerializeField] private float maxChargeTime = 0.75f;
        
        [Header("Speed settings")]
        [SerializeField] private float movementSpeed = 12f;
        [SerializeField] private float turnSpeed = 180f;

        [Header("Audio settings")] 
        [SerializeField] private AudioSource engineAudioSource;
        [SerializeField] private AudioSource shootingAudioSource;
        [SerializeField] private AudioClip engineIdlingAudio;
        [SerializeField] private AudioClip engineDrivingAudio;
        [SerializeField] private AudioClip chargingAudio;
        [SerializeField] private AudioClip fireAudio;
        [SerializeField] private AudioClip explosionAudio;
        [SerializeField] private float pitchRange = 0.2f;

        private string _movementAxisName;
        private string _turnAxisName;
        private string _fireButton;
        private float _movementInputValue;
        private float _turnInputValue;
        private float _originalPitch;
        private float _currentLaunchForce;
        private float _chargeSpeed;
        private bool _isFired;
        
        public void Initialize(int tankId, int health, Color color)
        {
            _movementAxisName = $"Vertical{tankId}";
            _turnAxisName = $"Horizontal{tankId}";
            _originalPitch = engineAudioSource.pitch;

            _fireButton = $"Fire{tankId}";
            _currentLaunchForce = minLaunchForce;
            tankCanvas.SetAimSlider(minLaunchForce);
            
            SetHealth(health, health);
            PaintColor(color);
        }

        public void EnableCanvas()
        {
            tankCanvas.gameObject.SetActive(true);
        }

        public void DisableCanvas()
        {
            tankCanvas.gameObject.SetActive(false);
        }

        public void TakeDamage(int damage)
        {
            TakeDamageAction?.Invoke(damage);
        }

        public void SetHealth(int maxHealth, int currentHealth)
        {
            tankCanvas.SetHealth(maxHealth, currentHealth);
        }

        public void Destroy()
        {
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();
            engineAudioSource.clip = explosionAudio;
            engineAudioSource.Play();
            gameObject.SetActive(false);
        }

        private void PaintColor(Color color)
        {
            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = color;
            }
        }

        private void Awake()
        {
            explosionParticles.gameObject.SetActive(false);
            _chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        }

        private void OnEnable()
        {
            rigidbody.isKinematic = false;
            _movementInputValue = 0f;
            _turnInputValue = 0f;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
        }

        private void OnDisable()
        {
            rigidbody.isKinematic = true;
            
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Stop();
            }
        }

        private void Update()
        {
            CalculateMovement();
            CalculateFire();
        }

        private void CalculateMovement()
        {
            if (_movementAxisName.Length <= 0)
                return;
            _movementInputValue = Input.GetAxis(_movementAxisName);
            _turnInputValue = Input.GetAxis(_turnAxisName);
            
            EngineAudio();
        }

        private void CalculateFire()
        {
            tankCanvas.SetAimSlider(minLaunchForce);
            if (_currentLaunchForce >= maxLaunchForce && !_isFired)
            {
                _currentLaunchForce = maxLaunchForce;
                Fire();
            }
            else if (Input.GetButtonDown (_fireButton))
            {
                _isFired = false;
                _currentLaunchForce = minLaunchForce;

                shootingAudioSource.clip = chargingAudio;
                shootingAudioSource.Play ();
            }
            else if (Input.GetButton (_fireButton) && !_isFired)
            {
                _currentLaunchForce += _chargeSpeed * Time.deltaTime;

                tankCanvas.SetAimSlider(_currentLaunchForce);
            }
            else if (Input.GetButtonUp (_fireButton) && !_isFired)
            {
                Fire();
            }
        }

        private void FixedUpdate()
        {
            Move();
            Turn();
        }

        private void EngineAudio()
        {
            if (Mathf.Abs (_movementInputValue) < 0.1f && Mathf.Abs (_turnInputValue) < 0.1f)
            {
                if (engineAudioSource.clip == engineDrivingAudio)
                {
                    engineAudioSource.clip = engineIdlingAudio;
                    engineAudioSource.pitch = Random.Range (_originalPitch - pitchRange, _originalPitch + pitchRange);
                    engineAudioSource.Play ();
                }
            }
            else
            {
                if (engineAudioSource.clip == engineIdlingAudio)
                {
                    engineAudioSource.clip = engineDrivingAudio;
                    engineAudioSource.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                    engineAudioSource.Play();
                }
            }
        }

        private void Move()
        {
            Vector3 movement = transform.forward * _movementInputValue * movementSpeed * Time.fixedDeltaTime;
            rigidbody.MovePosition(rigidbody.position + movement);
        }

        private void Turn()
        {
            float turn = _turnInputValue * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
            rigidbody.MoveRotation (rigidbody.rotation * turnRotation);
        }
        
        private void Fire ()
        {
            _isFired = true;
            ShellView shellInstance =
                Instantiate (shellPrefab, fireTransform.position, fireTransform.rotation);

            shellInstance.GetComponent<Rigidbody>().velocity = _currentLaunchForce * fireTransform.forward; 

            shootingAudioSource.clip = fireAudio;
            shootingAudioSource.Play ();

            _currentLaunchForce = minLaunchForce;
        }
    }
}