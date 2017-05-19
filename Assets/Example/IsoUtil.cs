using UnityEngine;

namespace Assets
{
    public static class IsoUtil
    {
        private static float _tileSizeInUnitsX = 1.0f;
        private static float _tileSizeInUnitsY = 0.5f;

        public enum IsoType
        {
            FLOOR,
            TILE
        }

        public static Vector3 CartesianToIso(int x, int y, IsoType mode)
        {
            return CartesianToIso(x * 1.0f, y * 1.0f, mode);
        }

        public static Vector3 CartesianToIso(float x, float y, IsoType mode)
        {
            var newX = (x - y) * 0.5f;
            var newY = (-x - y) * 0.25f;

            newX += x * -0.02f;
            newY += x * 0.01f;
            newX += y * 0.02f;
            newY += y * 0.01f;

            if(mode == IsoType.FLOOR)
            {
                newY -= 0.15f;
            }

            var newZ = CalculateIsoZBasedOnPosition(newX, newY);
            return new Vector3(newX, newY, newZ);
        }

        public static Vector3 IsoSnapToGridPosition(Vector3 position)
        {
            // Calculate ratios for simple grid snap
            float ratioX = Mathf.Round(position.y / _tileSizeInUnitsY - position.x / _tileSizeInUnitsX);
            float ratioY = Mathf.Round(position.y / _tileSizeInUnitsY + position.x / _tileSizeInUnitsX);

            // Calculate grid aligned position from current position
            float x = (ratioY - ratioX) * 0.5f * _tileSizeInUnitsX;
            float y = (ratioY + ratioX) * 0.5f * _tileSizeInUnitsY;
            var newX = x;
            var newY = y;
            newX += x * -0.02f;
            newY += x * 0.01f;
            newX += y * 0.02f;
            newY += y * 0.01f;

            return new Vector3(newX, newY, CalculateIsoZBasedOnPosition(newX, newY));
        }

        public static float CalculateIsoZBasedOnPosition(float x, float y)
        {
            return 1.0f * y - 0.1f * x;
        }
    }
}
