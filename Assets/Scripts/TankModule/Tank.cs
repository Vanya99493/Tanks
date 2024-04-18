using System;
using UnityEngine;

namespace TankModule
{
    public class Tank
    {
        public event Action<Tank> Death;
        
        private readonly TankView _view;
        
        public int Id { get; private set; }
        public int Health { get; private set; }
        public Color Color { get; private set; }
        public int Wins { get; private set; }

        private int _maxHealth;

        public Tank(TankView tankView, int id, int health, Color color)
        {
            _view = tankView;
            Id = id;
            _maxHealth = health;
            Health = _maxHealth;
            Color = color;
            _view.Initialize(Id, Health, Color);
            _view.TakeDamageAction += TakeDamage;
        }

        public Transform GetTransform()
        {
            return _view.transform;
        }

        public void AddWin()
        {
            Wins++;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Health = 0;
                Death?.Invoke(this);
                _view.Destroy();
            }
            
            _view.SetHealth(_maxHealth, Health);
        }

        public void EnableControl()
        {
            _view.EnableCanvas();
            _view.enabled = true;
        }

        public void DisableControl()
        {
            _view.DisableCanvas();
            _view.enabled = false;
        }

        public void Reset(Vector3 position)
        {
            _view.transform.position = position;
            _view.gameObject.SetActive(false);
            _view.gameObject.SetActive(true);
            Health = _maxHealth;
            _view.SetHealth(_maxHealth, Health);
        }
    }
}