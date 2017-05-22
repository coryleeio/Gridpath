using Gridpath;
using UnityEngine;

namespace GridPath.Example
{
    public static class IsometricMath
    {
        public enum DrawType
        {
            FLOOR, // Draw this on the floor, slightly shifted down from the tile position(also affects Z axis)
            TILE   // this is a tile, draw it at the cartesian position of the 64x64 sprite
        }

        public static float TileHeight = 0.5f;
        public static float TileWidth = 1.0f;

        public static float HalfTileWidth = TileWidth / 2.0f;
        public static float HalfTileHeight = TileHeight / 2.0f;

        public static Vector3 MapToWorld(int x, int y, DrawType mode)
        {
            return MapToWorld(x * 1.0f, y * 1.0f, mode);
        }

        public static Vector3 MapToWorld(float x, float y, DrawType mode)
        {
            var newX = (x - y) * HalfTileWidth;
            var newY = (-x - y) * HalfTileHeight;

            // Set Z based on x and Y, this is like a poor man's painters algorithm, causes the stuff further back in the scene to
            // appear behind the stuff thats closer to the bottom of the screen
            var newZ = 1.0f * newY;
            if (mode == DrawType.FLOOR)
            {
                // Shift downward a bit for sprites that are standing on the tile
                // usually we would do this by just drawing the characters after the tiles, but this works for our purposes here
                // since we are just using sprite renderers
                newZ -= 0.01f;
            }

            return new Vector3(newX, newY, newZ);
        }

        public static Point WorldToMap(Vector3 v)
        {
            var isoX = v.x;
            var isoY = v.y;

            var cartX = (isoX / HalfTileWidth - isoY / HalfTileHeight) / 2.0f;
            var cartY = (-isoY / HalfTileHeight - isoX / HalfTileWidth) / 2.0f;

            return new Point(Mathf.FloorToInt(cartX), Mathf.FloorToInt(cartY));
        }

        public static Vector3 GetMousePositionInScreenCoordinates()
        {
            return Input.mousePosition;
        }

        private static Vector3 ScreenToWorld(Camera camera, Vector3 screenCoordinates)
        {
            var ray = camera.ScreenPointToRay(screenCoordinates);
            // create a plane at 0,0,0 whose normal points to +Y:
            var hPlane = new Plane(Vector3.back, Vector3.zero);
            float distance;
            // if the ray hits the plane...
            if (hPlane.Raycast(ray, out distance))
            {
                // get the hit point:
                return ray.GetPoint(distance);
            }
            return Vector3.zero;
        }

        public static Vector3 GetMousePositionInWorldCoordinates(Camera camera)
        {
            return ScreenToWorld(camera, GetMousePositionInScreenCoordinates());
        }

        public static Point GetMousePositionOnMap(Camera camera)
        {
            var v = GetMousePositionInWorldCoordinates(camera);
            return WorldToMap(v);
        }
    }
}
