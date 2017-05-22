using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using GridPath.DotNetConcurrency;
using Debug = UnityEngine.Debug;

namespace Gridpath
{
    [AddComponentMenu("GridPath/PathFinder")]
    public class PathFinder : Singleton<PathFinder>
    {
        public enum LogLevel
        {
            On,
            Off
        }

        public enum GizmoLevel
        {
            ShowGrid,
            Off
        }

        private bool _threadsRunning;
        private List<Thread> _threads = new List<Thread>();
        public LogLevel PathLogging;
        public GizmoLevel DebugMode;

        private int _numberOfPathsToDrawInDebug = 5;
        private ConcurrentQueue<PathRequest> _incompletePaths = new ConcurrentQueue<PathRequest>();
        private ConcurrentQueue<PathRequest> _completePaths = new ConcurrentQueue<PathRequest>();
        private Queue<PathRequest> _previouslyCompletedPaths = new Queue<PathRequest>();

        private GridGraph _grid;
        public GridGraph Grid
        {
            get
            {
                return _grid;
            }
            private set
            {
                _grid = value;
            }
        }

        private void Log(String log)
        {
            if (PathLogging == LogLevel.On)
            {
                Debug.Log(log);
            }
        }

        private void LogCompletedRequest(PathRequest request)
        {
            if (PathLogging == LogLevel.On)
            {
                var min = request.TimeToFind.Minutes;
                var sec = request.TimeToFind.Seconds;
                var milli = request.TimeToFind.Milliseconds;
                Debug.Log(string.Format("Thread {0} Completed path {1},{2} -> {3},{4} in: {5}m:{6}s.{7}", request.ThreadId, request.StartX, request.StartY, request.EndX, request.EndY, min, sec, milli));
            }
        }

        void Update()
        {
            while (_completePaths.Count > 0)
            {
                // call completed path handlers in the main unity thread
                PathRequest completedRequest;
                _completePaths.TryDequeue(out completedRequest);
                if (completedRequest != null)
                {
                    LogCompletedRequest(completedRequest);
                    completedRequest.Handler(completedRequest.Path);
                    if (_previouslyCompletedPaths.Count > _numberOfPathsToDrawInDebug)
                    {
                        _previouslyCompletedPaths.Dequeue();
                    }
                    _previouslyCompletedPaths.Enqueue(completedRequest);
                }
            }
        }

        private void PathingWorker()
        {
            PathSolver solver = new PathSolver();
            Stopwatch watch = new Stopwatch();
            _threadsRunning = true;
            bool workDone = false;

            // This pattern lets us interrupt the work at a safe point if neeeded.
            while (_threadsRunning && !workDone)
            {
                PathRequest incompletePath;
                _incompletePaths.TryDequeue(out incompletePath);
                if (incompletePath != null)
                {
                    watch.Reset(); ;
                    watch.Start();
                    incompletePath.Path = solver.FindPath(incompletePath.StartX, incompletePath.StartY, incompletePath.EndX, incompletePath.EndY, Grid);
                    watch.Stop();
                    incompletePath.TimeToFind = watch.Elapsed;
                    incompletePath.ThreadId = Thread.CurrentThread.ManagedThreadId;
                    _completePaths.Enqueue(incompletePath);
                }
            }
            _threadsRunning = false;
        }

        void OnDisable()
        {
            // If the thread is still running, we should shut it down,
            // otherwise it can prevent the game from exiting correctly.
            if (_threadsRunning)
            {
                // This forces the while loop in the ThreadedWork function to abort.
                _threadsRunning = false;

                foreach (var thread in _threads)
                {
                    thread.Join();
                }
                // This waits until the thread exits,
                // ensuring any cleanup we do after this is safe. 
            }

            // Thread is guaranteed no longer running. 
        }

        public void Init(int gridSizeX, int gridSizeY, GridGraph.DiagonalOptions diagonalSetting, int numberOfThreads)
        {
            Grid = new GridGraph(gridSizeX, gridSizeY, diagonalSetting);
            if (numberOfThreads < 1)
            {
                Debug.LogError("Pathfinding will not work without atleast one thread!");
            }
            else
            {
                Log("Started with " + numberOfThreads + " threads.");
                for (var i = 0; i < numberOfThreads; i++)
                {
                    _threads.Add(new Thread(PathingWorker));
                }

                foreach (var thread in _threads)
                {
                    thread.Start();
                }
            }
        }

        private Vector3 FlipForDrawing(float x, float y)
        {
            return new Vector3(x, -y, 0.0f);
        }

        public delegate void OnPathComplete(Path path);

        public void StartPath(int startX, int startY, int endX, int endY, OnPathComplete handler)
        {
            Log("started path!");
            _incompletePaths.Enqueue(new PathRequest()
            {
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                Handler = handler
            });
        }

        private void DrawSquare(float x, float y)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(FlipForDrawing(x - 0.5f, y + 0.5f), FlipForDrawing(x + 0.5f, y + 0.5f));
            Gizmos.DrawLine(FlipForDrawing(x + 0.5f, y + 0.5f), FlipForDrawing(x + 0.5f, y - 0.5f));
            Gizmos.DrawLine(FlipForDrawing(x + 0.5f, y - 0.5f), FlipForDrawing(x - 0.5f, y - 0.5f));
            Gizmos.DrawLine(FlipForDrawing(x - 0.5f, y - 0.5f), FlipForDrawing(x - 0.5f, y + 0.5f));
        }

        private void DrawX(float x, float y)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(FlipForDrawing(x - 0.5f, y + 0.5f), FlipForDrawing(x + 0.5f, y - 0.5f));
            Gizmos.DrawLine(FlipForDrawing(x + 0.5f, y + 0.5f), FlipForDrawing(x - 0.5f, y - 0.5f));
        }

        void OnDrawGizmos()
        {
            if (Grid != null && DebugMode == GizmoLevel.ShowGrid)
            {
                var sizeX = Grid.SizeX;
                var sizeY = Grid.SizeY;
                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
                    {
                        DrawSquare(x, y);
                        if (!Grid.NodeAt(x, y).Walkable)
                        {
                            DrawX(x, y);
                        }
                    }
                }

                if (_previouslyCompletedPaths != null && _previouslyCompletedPaths.Count > 0)
                {
                    foreach (var pathRequest in _previouslyCompletedPaths)
                    {
                        var path = pathRequest.Path;
                        if (path.Nodes.Count > 0)
                        {
                            var nextNode = path.Nodes[0];
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(FlipForDrawing(path.StartNode.X, path.StartNode.Y), FlipForDrawing(nextNode.X, nextNode.Y));
                        }
                        for (var i = 0; i < path.Nodes.Count; i++)
                        {
                            var node = path.Nodes[i];
                            PathNode nextNode = null;
                            if (i + 1 < path.Nodes.Count)
                            {
                                nextNode = path.Nodes[i + 1];
                            }
                            if (nextNode != null)
                            {
                                Gizmos.color = Color.green;
                                Gizmos.DrawLine(FlipForDrawing(node.X, node.Y), FlipForDrawing(nextNode.X, nextNode.Y));
                            }
                        }
                    }
                }
            }
        }

        private class PathRequest
        {
            public int StartX;
            public int StartY;
            public int EndX;
            public int EndY;
            public int ThreadId;
            public Path Path = null;
            public OnPathComplete Handler;
            public TimeSpan TimeToFind;
        }
    }

}
