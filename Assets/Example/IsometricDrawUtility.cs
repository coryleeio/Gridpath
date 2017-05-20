using UnityEngine;

namespace Assets
{
    public static class IsometricDrawUtility
    {
        public enum DrawType
        {
            FLOOR, // Draw this on the floor, slightly shifted down from the tile position(also affects Z axis)
            TILE   // this is a tile, draw it at the cartesian position of the 64x64 sprite
        }

        public static Vector3 CartesianToIsometricDraw(int x, int y, DrawType mode)
        {
            return CartesianToIsometricDraw(x * 1.0f, y * 1.0f, mode);
        }

        public static Vector3 CartesianToIsometricDraw(float x, float y, DrawType mode)
        {
            var newX = (x - y) * 0.5f;
            var newY = (-x - y) * 0.25f;

            // Shift up and to the left about 1px so our tiles create 1px lines instead of doubling up.
            newX += x * -0.02f;
            newY += x * 0.01f;
            newX += y * 0.02f;
            newY += y * 0.01f;



            // Set Z based on x and Y, this is like a poor man's painters algorithm, causes the stuff further back in the scene to
            // appear behind the stuff thats closer to the bottom of the screen
            var newZ = 1.0f * newY - 0.1f * newX;
            if (mode == DrawType.FLOOR)
            {
                // Shift downward a bit for sprites that are standing on the tile
                newZ -= 0.01f;
            }
            return new Vector3(newX, newY, newZ);
        }
    }
}
