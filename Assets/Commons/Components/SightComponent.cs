using UnityEngine;

namespace Evolution.Commons.Components
{
    public class SightComponent : MonoBehaviour
    {
        [field:SerializeField]
        public float SightRadius { get; private set; } = 1.0f;


        public void SetSightRadius(float radius)
        {
            if (radius < 0.0f)
            {
                radius = 0.0f;
            }
            SightRadius = radius;
        }

        public Collider2D[] FindAllInLayer(LayerMask layer)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, SightRadius, layer);
            return hits;
        }
        void OnDrawGizmosSelected()
        {
            if (SightRadius == 0) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, SightRadius);
        }

    }
}
