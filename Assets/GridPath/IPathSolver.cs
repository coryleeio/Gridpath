using GridPath;

namespace Assets.GridPath
{
    interface IPathSolver
    {
        Path FindPath(int startX, int startY, int endX, int endY, GridGraph grid);
    }
}
