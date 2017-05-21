using GridPath;
using UnityEngine;

namespace Assets.GridPath.Example
{
    public class PlayerController : MonoBehaviour
    {
        private Vector3 _mousePositionInScreenCoordinates = Vector3.zero;
        private Vector3 _mousePositionInWorldCoordinates = Vector3.zero;
        private Point _cellCoordinates = new Point(0, 0);
        private Vector3 _centerOfCellPosition = Vector3.zero;
        private Camera _camera;
        private GameObject _target;

        public void Update()
        {
            if(_camera == null)
            {
                _camera = Camera.current;
                if (_camera == null)
                {
                    return;
                }
            }

            if (_target == null)
            {
                var gameObjects = GameObject.FindGameObjectsWithTag("Target");
                if (gameObjects.Length > 0)
                {
                    _target = gameObjects[0];
                };
            }

            _mousePositionInScreenCoordinates = IsometricMath.GetMousePositionInScreenCoordinates();
            _mousePositionInWorldCoordinates = IsometricMath.GetMousePositionInWorldCoordinates(_camera);
            _cellCoordinates = IsometricMath.GetMousePositionInCartesianCoordinates(_camera);
            _centerOfCellPosition = IsometricMath.CartesianToIso(_cellCoordinates.x, _cellCoordinates.y, IsometricMath.DrawType.FLOOR);

            if (Input.GetButton("Fire1"))
            {
                MoveTargetToMouse();
            }
        }

        private void MoveTargetToMouse()
        {
            if (PathFinder.Instance.Grid.NodeInGrid(_cellCoordinates.x, _cellCoordinates.y))
            {
                var cartesianPosition = _target.GetComponent<CartesianPosition>();
                cartesianPosition.X = _cellCoordinates.x;
                cartesianPosition.Y = _cellCoordinates.y;
            }
        }

        public void OnGUI()
        {
            GUI.Label(new UnityEngine.Rect(10, 160, 200, 40), "Click the mouse to place the target at the location.");
            GUI.Label(new UnityEngine.Rect(10, 200, 200, 40), "_mousePosition: " + _mousePositionInScreenCoordinates);
            GUI.Label(new UnityEngine.Rect(10, 240, 200, 40), "WorldPosition: " + _mousePositionInWorldCoordinates);
            GUI.Label(new UnityEngine.Rect(10, 280, 200, 40), "CellCoordinates: " + _cellCoordinates);
            GUI.Label(new UnityEngine.Rect(10, 320, 200, 40), "CenterOfCellPosition: " + _centerOfCellPosition);
        }
    }
}
