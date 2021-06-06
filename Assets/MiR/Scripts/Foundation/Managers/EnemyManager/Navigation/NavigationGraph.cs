using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class NavigationGraph : MonoBehaviour
    {
        struct Connection
        {
            public NavigationPoint Point1;
            public NavigationPoint Point2;
            public IEdge Edge1;
            public IEdge Edge2;
        }

        PathFinder pathFinder;
        Dictionary<NavigationPoint, Node> nodes;
        Dictionary<INode, NavigationPoint> nodePoints;
        Dictionary<NavigationPoint, bool> canSeePlayer;
        List<Connection> connections;

        [Inject] IPlayerManager playerManager = default;

        void Start()
        {
            var points = GetComponentsInChildren<NavigationPoint>();

            nodes = new Dictionary<NavigationPoint, Node>();
            nodePoints = new Dictionary<INode, NavigationPoint>();
            canSeePlayer = new Dictionary<NavigationPoint, bool>();
            foreach (var point in points) {
                var pos = point.transform.position;
                var node = new Node(new Position(pos.x, pos.z));
                nodes[point] = node;
                nodePoints[node] = point;
                canSeePlayer[point] = false;
            }

            connections = new List<Connection>();
            var edges = new HashSet<(NavigationPoint, NavigationPoint)>();
            foreach (var point1 in points) {
                foreach (var point2 in points) {
                    if (!edges.Add((point1, point2)) || !edges.Add((point2, point1)))
                        continue;

                    var node1 = nodes[point1];
                    var node2 = nodes[point2];

                    var connection = new Connection();
                    connection.Point1 = point1;
                    connection.Point2 = point2;
                    connection.Edge1 = node1.Connect(node2, Velocity.FromMetersPerSecond(0.0f));
                    connection.Edge2 = node2.Connect(node1, Velocity.FromMetersPerSecond(0.0f));

                    connections.Add(connection);
                }
            }
        }

        Node FindClosestNode(Vector3 position)
        {
            float nearestDistance = 0.0f;
            Node nearestNode = null;

            foreach (var it in nodes) {
                float sqrDistance = (it.Key.transform.position - position).sqrMagnitude;
                if (nearestNode == null || sqrDistance < nearestDistance) {
                    nearestNode = it.Value;
                    nearestDistance = sqrDistance;
                }
            }

            return nearestNode;
        }

        bool CanSeePlayer(NavigationPoint point)
        {
            foreach (var player in playerManager.EnumeratePlayers()) {
                var origin = point.transform.position;

                var direction = player.Position - origin;
                float distance = direction.magnitude;
                direction /= distance;

                if (Physics.Raycast(origin, direction, out var hitInfo, distance)) {
                    var context = hitInfo.transform.GetComponentInParent<Context>();
                    if (context != null) {
                        var hitPlayer = context.Container.TryResolve<IPlayer>();
                        if (hitPlayer != null)
                            return true;
                    }
                }
            }

            return false;
        }

        void FindPath(Vector3 from, NavigationPoint to, List<Vector3> result)
        {
            if (pathFinder == null)
                pathFinder = new PathFinder();

            foreach (var it in nodes)
                canSeePlayer[it.Key] = CanSeePlayer(it.Key);

            foreach (var conn in connections) {
                float speed1 = (canSeePlayer[conn.Point1] ? 1.0f : 2.0f);
                float speed2 = (canSeePlayer[conn.Point2] ? 1.0f : 2.0f);
                var velocity = Velocity.FromMetersPerSecond(speed1 + speed2);
                conn.Edge1.TraversalVelocity = velocity;
                conn.Edge2.TraversalVelocity = velocity;
            }

            Node start = FindClosestNode(from);
            Node end = nodes[to];
            var path = pathFinder.FindPath(start, end, Velocity.FromMetersPerSecond(4.0f));

            result.Clear();
            foreach (var point in path.Edges)
                result.Add(nodePoints[point.Start].transform.position);

            result.Add(nodePoints[path.Edges.Last().End].transform.position);
        }
    }
}
