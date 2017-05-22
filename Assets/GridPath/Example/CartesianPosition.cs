using UnityEngine;

namespace GridPath.Example
{
    [AddComponentMenu("GridPath/Example/CartesianPosition")]
    public class CartesianPosition : MonoBehaviour
    {
        public IsometricMath.DrawType DrawPosition;

        private int _x;
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                UpdateDrawPosition(X, Y);
            }
        }

        private int _y;
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                UpdateDrawPosition(X, Y);
            }
        }

        private void UpdateDrawPosition(int newX, int newY)
        {
            transform.position = IsometricMath.MapToWorld(newX, newY, DrawPosition);
        }
    }
}
