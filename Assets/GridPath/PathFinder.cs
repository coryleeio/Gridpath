using Assets.Game;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using GridPath.DotNetConcurrency;

namespace GridPath
{
    public class PathFinder : Singleton<PathFinder>
    {
        private class PathRequest
        {
            public int StartX;
            public int StartY;
            public int EndX;
            public int EndY;
            public Path Path = null;
            public OnPathComplete Handler;
            public TimeSpan TimeToFind;
        }

        public bool ShowGraph;
        private ConcurrentQueue<PathRequest> _incompletePaths = new ConcurrentQueue<PathRequest>();
        private ConcurrentQueue<PathRequest> _completePaths = new ConcurrentQueue<PathRequest>();
        private List<PathRequest> _previouslyCompletedPaths = new List<PathRequest>();
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

        bool _threadRunning;
        Thread _thread;

        void Start()
        {
            _thread = new Thread(PathingWorker);
            _thread.Start();
        }

        void Update()
        {
            while(_completePaths.Count > 0)
            {
                // call completed path handlers in the main unity thread
                PathRequest completedRequest;
                _completePaths.TryDequeue(out completedRequest);
                if(completedRequest != null)
                {
                    completedRequest.Handler(completedRequest.Path);
                    _previouslyCompletedPaths.Add(completedRequest);
                }
            }
        }

        void PathingWorker()
        {
            IPathSolver solver = new PathSolver();
            Stopwatch watch = new Stopwatch();
            _threadRunning = true;
            bool workDone = false;

            // This pattern lets us interrupt the work at a safe point if neeeded.
            while (_threadRunning && !workDone)
            {
                PathRequest incompletePath;
                _incompletePaths.TryDequeue(out incompletePath);
                if(incompletePath != null)
                {
                    watch.Reset(); ;
                    watch.Start();
                    incompletePath.Path = solver.FindPath(incompletePath.StartX, incompletePath.StartY, incompletePath.EndX, incompletePath.EndY, Grid);
                    watch.Stop();

                    incompletePath.TimeToFind = watch.Elapsed;

                    var min = incompletePath.TimeToFind.Minutes;
                    var sec = incompletePath.TimeToFind.Seconds;
                    var milli = incompletePath.TimeToFind.Milliseconds;
                    UnityEngine.Debug.Log(string.Format("Completed path {0},{1} -> {2},{3} in: {4}m:{5}s.{6}", incompletePath.StartX, incompletePath.StartY, incompletePath.EndX, incompletePath.EndY, min,sec,milli));
                    _completePaths.Enqueue(incompletePath);
                }
            }
            _threadRunning = false;
        }

        void OnDisable()
        {
            // If the thread is still running, we should shut it down,
            // otherwise it can prevent the game from exiting correctly.
            if (_threadRunning)
            {
                // This forces the while loop in the ThreadedWork function to abort.
                _threadRunning = false;

                // This waits until the thread exits,
                // ensuring any cleanup we do after this is safe. 
                _thread.Join();
            }

            // Thread is guaranteed no longer running. 
        }

        public void BuildGrid(int gridSizeX, int gridSizeY)
        {
            Grid = new GridGraph(gridSizeX, gridSizeY);
        }

        private Vector3 FlipForDrawing(float x, float y)
        {
            return new Vector3(x, -y, 0.0f);
        }

        public delegate void OnPathComplete(Path path);

        public void StartPath(int startX, int startY, int endX, int endY, OnPathComplete handler)
        {
            UnityEngine.Debug.Log("started path!");
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

        void OnDrawGizmosSelected()
        {
            if (Grid != null && ShowGraph)
            {
                var sizeX = Grid.SizeX;
                var sizeY = Grid.SizeY;
                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
                    {
                        DrawSquare(x, y);
                        if (!Grid.NodeAt(x, y).walkable)
                        {
                            DrawX(x, y);
                        }
                    }
                }

                if (_previouslyCompletedPaths != null && _previouslyCompletedPaths.Count > 0)
                {
                    foreach(var pathRequest in _previouslyCompletedPaths)
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
                                if(pathRequest.TimeToFind.TotalSeconds < 1.0d)
                                {
                                    Gizmos.color = Color.green;
                                }
                                else if (pathRequest.TimeToFind.TotalSeconds < 3.0d)
                                {
                                    Gizmos.color = Color.yellow;
                                }
                                else
                                {
                                    Gizmos.color = Color.red;
                                }
                                Gizmos.DrawLine(FlipForDrawing(node.X, node.Y), FlipForDrawing(nextNode.X, nextNode.Y));
                            }
                        }
                    }
                }
            }
        }
    }
}
