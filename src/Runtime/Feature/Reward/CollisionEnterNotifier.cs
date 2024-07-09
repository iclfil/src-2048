using UnityEngine;

namespace Markins.Runtime.Game
{

    public delegate void OnCollision(CollisionEnterNotifier cur, string collisionTag);

    public class CollisionEnterNotifier : MonoBehaviour
    {
        public Rigidbody Rigidbody;
        public bool EnableCollision;

        public OnCollision OnCollision;

        public void OnCollisionEnter(Collision other)
        {
            if (EnableCollision == false)
                return;

            OnCollision?.Invoke(this, other.collider.tag);
        }
    }
}

